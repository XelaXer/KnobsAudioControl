using System.Text.Json;
using IHID.HIDManager;
using IHID.HIDDevice;
using Knobs.Controller;
using Knobs.WindowsAudio;

class Program
{
	public delegate void ConfigurationChangedCallback(ControllerConfiguration newConfig);

	static void Main()
	{
		LoadEnvironmentVariables();

		// TODO: Get JSON Config File
		string fileName = "C:/Users/alexa/Desktop/test_controller_config_v2.json";

		if (!File.Exists(fileName))
		{
			Console.WriteLine("The configuration file does not exist.");
			return;
		}

		ControllerConfiguration configuration = null;
		string jsonString = File.ReadAllText(fileName);
		try
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true // Use this option to ignore property name case
			};

			configuration = JsonSerializer.Deserialize<ControllerConfiguration>(jsonString, options);

			if (configuration?.Actuators != null)
			{
				foreach (var actuator in configuration.Actuators)
				{
					// Just for demonstration, print out the actuator type to verify it's being read
					Console.WriteLine($"Actuator ID: {actuator.Id}, Actuator Type: {actuator.ActuatorType}");
				}
			}

			// Use the configuration object as needed
			Console.WriteLine("Controller Configuration successfully deserialized.");
		}
		catch (JsonException ex)
		{
			Console.WriteLine($"An error occurred during deserialization: {ex.Message}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An unexpected error occurred: {ex.Message}");
		}

		WindowsAudioHandler WindowsAudioHandler = new WindowsAudioHandler();


		Controller Controller = new Controller(configuration, WindowsAudioHandler);
		HIDManager HManager = new HIDManager();
		IHIDDevice HDevice = HManager.LoadDefaultDevice();
		HDevice.OpenDevice();
		HDevice.StartReading(Controller.ProcessHIDEvent);
		Controller.ProcessControllerEvent(new ControllerEvent("event", 2, 75, 0));

		while (true)
		{
			// System.Threading.Thread.Sleep(100);
		}
	}

	// Poll for device on separate thread
	// StopReading()

	void CreateController(ControllerConfiguration ctrlCfg, WindowsAudioHandler wAudioHandler)
	{

	}

	static void CreateConfigurationFileWatcher(string path, ConfigurationChangedCallback callback)
	{
		FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(path))
		{
			Filter = Path.GetFileName(path),
			NotifyFilter = NotifyFilters.LastWrite // Watch for changes in last write time
		};

		// Event handler for the Changed event
		watcher.Changed += (sender, e) =>
		{
			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				Console.WriteLine($"Change detected in configuration file: {e.FullPath}");
				try
				{
					// Read the new JSON config file
					string newJsonString = File.ReadAllText(e.FullPath);
					var newConfig = JsonSerializer.Deserialize<ControllerConfiguration>(newJsonString, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

					// Invoke the callback method
					callback?.Invoke(newConfig);
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

	static void LoadController(ControllerConfiguration newConfig)
	{
		Console.WriteLine("New controller configuration loaded.");

		// UpdateController(newConfig); // This is a placeholder for the actual update logic
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
}