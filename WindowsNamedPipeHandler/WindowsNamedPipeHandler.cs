using System;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class WindowsNamedPipeHandler : IDisposable
{
	private NamedPipeServerStream _pipeServer;
	private readonly ConcurrentDictionary<string, Func<string, Task<string>>> _functionMap;
	private CancellationTokenSource _cancellationTokenSource;
	private Task? _readTask;

	public WindowsNamedPipeHandler(string pipeName)
	{
		_functionMap = new ConcurrentDictionary<string, Func<string, Task<string>>>();
		_cancellationTokenSource = new CancellationTokenSource();
		_pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
	}

	public void Dispose()
	{
		_cancellationTokenSource.Cancel();
		_readTask?.Wait();
		_pipeServer?.Dispose();
		_cancellationTokenSource.Dispose();
	}

	public async Task<bool> WaitForConnectionAsync(CancellationToken cancellationToken)
	{
		try
		{
			await _pipeServer.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
			return true;
		}
		catch (OperationCanceledException)
		{
			return false;
		}
	}

	public async Task SendToNamedPipeAsync(string message, CancellationToken cancellationToken)
	{
		byte[] messageBytes = Encoding.UTF8.GetBytes(message);
		await _pipeServer.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken).ConfigureAwait(false);
		await _pipeServer.FlushAsync(cancellationToken).ConfigureAwait(false);
	}

	public void StartReadLoop()
	{
		_readTask = ReadPipeAsync(_cancellationTokenSource.Token);
	}

	private async Task ReadPipeAsync(CancellationToken cancellationToken)
	{
		var buffer = new byte[1024];
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				if (!_pipeServer.IsConnected) // Check if the pipe is connected
				{
					// If not connected, wait for a connection before proceeding
					Console.WriteLine("Waiting for pipe connection...");
					await _pipeServer.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
				}

				Console.WriteLine($"Performing Pipe Read, cancel token value is {cancellationToken.IsCancellationRequested}");
				int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);

				if (bytesRead > 0)
				{
					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					await ConvertMessageToFunctionCallAsync(message).ConfigureAwait(false);
				}
				else
				{
					// Handle the zero bytes read scenario, which means the other end has closed the pipe.
					Console.WriteLine("Zero bytes read, the client may have closed the connection.");
					_pipeServer.Disconnect(); // Disconnect the current instance to allow a new connection
					continue; // Continue the loop, which will wait for a new connection
				}
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine("Read loop cancelled by cancellation token.");
				break;
			}
			catch (IOException ioEx)
			{
				Console.WriteLine($"An IO exception occurred: {ioEx.Message}");
				if (!_pipeServer.IsConnected)
				{
					// If an IO exception occurred and the pipe isn't connected, wait for a new connection
					_pipeServer.Disconnect(); // Disconnect the current instance to allow a new connection
				}
				// You may also want to consider whether to continue the loop or not here
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An unexpected exception occurred: {ex.ToString()}");
				// Handle other exceptions and potentially reconnect the pipe if necessary
				_pipeServer.Disconnect(); // Ensures the pipe is reset and ready for a new connection
			}
		}
	}



	private async Task ConvertMessageToFunctionCallAsync(string message)
	{
		try
		{
			var jsonElement = JsonDocument.Parse(message).RootElement;
			var functionName = jsonElement.GetProperty("requestName").GetString();
			if (_functionMap.TryGetValue(functionName, out var function))
			{
				string response = await function(message);
				await SendResponseThroughPipeAsync(response);
			}
		}
		catch (JsonException e)
		{
			Console.WriteLine($"An error occurred during deserialization: {e.Message}");
		}
	}

	private async Task SendResponseThroughPipeAsync(string response)
	{
		// Convert the response to a byte array
		byte[] responseBytes = Encoding.UTF8.GetBytes(response);
		// Send the response bytes back through the pipe
		// You will need to implement the actual named pipe send logic based on your pipe handling code
		await _pipeServer.WriteAsync(responseBytes, 0, responseBytes.Length);
		await _pipeServer.FlushAsync(); // Ensure all data is sent immediately
	}

	public void AddFunction(string functionName, Func<string, Task<string>> function)
	{
		_functionMap[functionName] = function;
	}
}