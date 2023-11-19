using System.Text.Json;
using IHID.HIDManager;
using IHID.HIDDevice;
using Knobs.Controller;
using Knobs.WindowsAudio;

class Program
{
	const string CONFIG_DIRECTORY = "C:/Users/alexa/Desktop/ControllerConfig";
	public delegate void ConfigurationChangedCallback(ControllerConfiguration newConfig);

	public WindowsAudioHandler? WindowsAudioHandler;
	public Controller? Controller;
	HIDManager HManager;
	IHIDDevice? HDevice;

	static void Main()
	{
		Program program = new Program();
		program.Run();
	}

	// Poll for device on separate thread
	// StopReading()
	public void Run()
	{
		LoadEnvironmentVariables();

		/*
		TODO HERE:
			Logic should be
				Check file
					If no file, sleep thread
				Parse file
					If JSON parse fails, sleep thread
				Return configuration object
		*/
		string fileName = "test_controller_config_v2.json";
		string filePath = Path.Combine(CONFIG_DIRECTORY, fileName);
		ControllerConfiguration? configuration = PollAndParseConfigurationFile(filePath);
		if (configuration == null)
		{
			Console.WriteLine("Failed to load controller configuration. Exiting...");
			return;
		}
		CreateConfigurationFileWatcher(filePath, ReloadController);

		WindowsAudioHandler = new WindowsAudioHandler();
		Controller = new Controller(configuration, WindowsAudioHandler);
		HManager = new HIDManager();
		HDevice = HManager.LoadDefaultDevice();
		HDevice.OpenDevice();
		HDevice.StartReading(Controller.ProcessHIDEvent);

		WindowsNamedPipeHandler _pipeHandler;
		_pipeHandler = new WindowsNamedPipeHandler("MyNamedPipe");
		_pipeHandler.AddFunction("get-current-audio-session-programs", PrintMessageFunction);
		_pipeHandler.StartReadLoop();

		while (true)
		{
			// System.Threading.Thread.Sleep(100);
		}
	}

	static public ControllerConfiguration? PollAndParseConfigurationFile(string filePath)
	{
		while (!File.Exists(filePath))
		{
			Console.WriteLine($"Configuration file not found at {filePath}. Waiting for file to be created...");
			Thread.Sleep(1000);
		}

		string fileContents = File.ReadAllText(filePath);
		ControllerConfiguration? configuration = ParseControllerConfiguration(fileContents);
		return configuration;
	}

	static ControllerConfiguration? ParseControllerConfiguration(string fileContents)
	{
		try
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			ControllerConfiguration? configuration = JsonSerializer.Deserialize<ControllerConfiguration>(fileContents, options);

			if (configuration?.Actuators != null)
			{
				foreach (var actuator in configuration.Actuators)
				{
					Console.WriteLine($"Actuator ID: {actuator.Id}, Actuator Type: {actuator.ActuatorType}");
				}
			}

			Console.WriteLine("Controller Configuration successfully deserialized.");
			return configuration;
		}
		catch (JsonException ex)
		{
			Console.WriteLine($"An error occurred during deserialization: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An unexpected error occurred: {ex.Message}");
		}
		return null;
	}

	static void CreateConfigurationFileWatcher(string path, ConfigurationChangedCallback callback)
	{
		FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(path))
		{
			Filter = Path.GetFileName(path),
			NotifyFilter = NotifyFilters.LastWrite
		};

		// Event handler for the Changed event
		watcher.Changed += (sender, e) =>
		{
			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				Console.WriteLine($"Change detected in configuration file: {e.FullPath}");
				try
				{
					string fileContents = File.ReadAllText(e.FullPath);
					ControllerConfiguration? configuration = ParseControllerConfiguration(fileContents);
					callback?.Invoke(configuration);
				}
				catch (JsonException jsonEx)
				{
					Console.WriteLine($"An error occurred during deserialization: {jsonEx.Message}");
				}
				catch (IOException ioEx)
				{
					Console.WriteLine($"An I/O error occurred: {ioEx.Message}");
				}
			}
		};

		// Begin watching the file
		watcher.EnableRaisingEvents = true;
	}

	void ReloadController(ControllerConfiguration cfg)
	{
		// Controller = null;\
		if (Controller != null)
		{
			Controller.Dispose();
		}
		Controller = new Controller(cfg, WindowsAudioHandler);
		HDevice.StopReading();
		// HDevice.OpenDevice();
		HDevice.StartReading(Controller.ProcessHIDEvent);
		// CloseDevice

		// UpdateController(newConfig);
	}

	static void LoadEnvironmentVariables()
	{
		var lines = File.ReadAllLines(".env");
		foreach (var line in lines)
		{
			var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length == 2)
			{
				Environment.SetEnvironmentVariable(parts[0], parts[1], EnvironmentVariableTarget.Process);
			}
		}
	}

	static Task<string> PrintMessageFunction(string message)
	{
		Console.WriteLine(message);
		// await Task.CompletedTask; // To keep it awaitable in case you need to do async operations in the future.
		string response = "Response to message received"; // Replace with actual response generation logic
		return Task.FromResult(response); // Wrap the response in a Task and return it
	}
}