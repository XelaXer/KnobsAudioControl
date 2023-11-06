using System.Collections;
using System.Text.Json;
using NAudio.CoreAudioApi;

using IHID.HIDManager;
using IHID.HIDDevice;
using Knobs.Controller;
using Knobs.Actuators;
using Knobs.WindowsAudio;

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

// WindowsAudioHandler.SetVolumeByProcessName("chrome", 0.5f);
// VolumeControl volumeControl = new(1, 100, 0, 100, "rotary pot", "volume knob", new List<string> { "chrome", "firefox" }, new WindowsAudioHandler());
// volumeControl.ProcessEvent(new ControllerEvent("event", 1, 50, 0));