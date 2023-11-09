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
    private readonly ConcurrentDictionary<string, Func<string, Task>> _functionMap;
    private CancellationTokenSource _cancellationTokenSource;
    private Task? _readTask;

    public WindowsNamedPipeHandler(string pipeName)
    {
        _functionMap = new ConcurrentDictionary<string, Func<string, Task>>();
        _cancellationTokenSource = new CancellationTokenSource();
        _pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _readTask?.Wait(); // Ensure the read loop completes before disposing.
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
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                int bytesRead = await _pipeServer.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                await ConvertMessageToFunctionCallAsync(message).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation
        }
        catch (IOException e)
        {
            // Handle the IO exception
        }
    }

    // private async Task ConvertMessageToFunctionCallAsync(string message)
    // {
    //     try
    //     {
    //         var jsonElement = JsonDocument.Parse(message).RootElement;
    //         var functionName = jsonElement.GetProperty("requestName").GetString();
    //         if (_functionMap.TryGetValue(functionName, out var function))
    //         {
    //             await function(message).ConfigureAwait(false);
    //         }
    //     }
    //     catch (JsonException e)
    //     {
    //         // Handle JSON parsing error
    //     }
    // }

    // public void AddFunction(string functionName, Action<string> function)
    // {
    //     _functionMap[functionName] = (message) => Task.Run(() => function(message));
    // }

    // private async Task ExecuteFunctionAsync(string functionName, string message)
    // {
    //     if (_functionMap.TryGetValue(functionName, out var function))
    //     {
    //         await function(message); // This assumes the stored delegate is of type Func<string, Task>
    //     }
    // }

    public void AddFunction(string functionName, Action<string> action)
    {
        Func<string, Task> functionWrapper = message =>
        {
            try
            {
                action(message);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.Error.WriteLine($"Error executing action for {functionName}: {ex}");
            }
            return Task.CompletedTask;
        };

        _functionMap[functionName] = functionWrapper;
    }

    private async Task ConvertMessageToFunctionCallAsync(string message)
    {
        string functionName = ExtractFunctionNameFromMessage(message);

        if (_functionMap.TryGetValue(functionName, out var function))
        {
            await function(message);
        }
        else
        {
            Console.Error.WriteLine($"No function registered under the name: {functionName}");
        }
    }

    private string ExtractFunctionNameFromMessage(string message)
    {
        var jsonDocument = JsonDocument.Parse(message);
        if (jsonDocument.RootElement.TryGetProperty("requestName", out var functionNameElement))
        {
            return functionNameElement.GetString();
        }

        throw new InvalidOperationException("The message does not contain a function name.");
    }
}
