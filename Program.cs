using System.Text.Json;
using IHID.HIDManager;
using IHID.HIDDevice;
using Knobs.Controller;
using Knobs.WindowsAudio;
using System.Text;
using Knobs.Actuators;

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

		//, test_controller_config_v2.json
		string fileName = "controller_config_v3.json";
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

		InitializeHIDDevice();

		// Start a new thread to poll the HID device
		Thread pollingThread = new Thread(new ThreadStart(PollHIDDevice));
		pollingThread.IsBackground = true; // Set as background thread
		pollingThread.Start();

		WindowsNamedPipeHandler _pipeHandler;
		_pipeHandler = new WindowsNamedPipeHandler("MyNamedPipe");
		_pipeHandler.AddFunction("get-current-audio-session-programs", PrintMessageFunction);
		_pipeHandler.StartReadLoop();

		

		while (true)
		{
			// Update Controller
			Controller.UpdateControllerState(HDevice);

			// Console.WriteLine("[NEW NEW NEW]");
			// foreach (var actuatorGroup in Controller.GetActuatorGroups())
			// {
			// 	string actuatorGroupType = actuatorGroup.GetGroupType();

			// 	// Run update group


			// 	// Console.WriteLine($"[MAIN] Actuator Group Type: {actuatorGroupType}");
			// 	if (actuatorGroupType == "audio_control_knob_v1")
			// 	{
			// 		ToggleMute tm = (ToggleMute)actuatorGroup.MActuatorListByType["toggle_mute"][0];
			// 		bool liveState = tm.GetLiveMuteState();
			// 		bool savedState = tm.GetSavedMuteState();

			// 		// Console.WriteLine($"[MAIN] [UPDATE STATE] Live state: {liveState}, Saved state: {savedState}");
			// 		if (liveState != savedState)
			// 		{
			// 			tm.UpdateMuteState(liveState);
			// 			Console.WriteLine("!!!!!!!!!!!!!!! MISMATCH !!!!!!!!!!!!!!!!!");
			// 			// Update State of LED actuator

			// 			if (liveState == false)
			// 			{
			// 				// Clear LED
			// 				string[] arrHIDEvent = new string[5];
			// 				arrHIDEvent[0] = " event";
			// 				arrHIDEvent[1] = "300";
			// 				arrHIDEvent[2] = "0";
			// 				arrHIDEvent[3] = "0";
			// 				arrHIDEvent[4] = "0";
			// 				SendHIDEvent(arrHIDEvent);
			// 			}
			// 			else
			// 			{
			// 				// Send white color to LED 300
			// 				string[] arrHIDEvent = new string[5];
			// 				arrHIDEvent[0] = " event";
			// 				arrHIDEvent[1] = "300";
			// 				arrHIDEvent[2] = "255";
			// 				arrHIDEvent[3] = "255";
			// 				arrHIDEvent[4] = "255";
			// 				SendHIDEvent(arrHIDEvent);
			// 			}
			// 		}
			// 	}
				
			// 	// Console.WriteLine(actuatorGroup.GetId());
			// 	foreach (var actuator in actuatorGroup.GetActuators())
			// 	{
			// 		string actuatorType = actuator.GetActuatorType();
			// 		// Console.WriteLine(actuatorType);
			// 		if (actuatorType == "toggle_mute")
			// 		{
			// 			//
			// 		}
			// 		// actuator.UpdateActuatorState(HDevice);
			// 	}
			
			// }
			


			// byte[] bytes = Encoding.ASCII.GetBytes(" Hello from C#");
			/*
				0 -> type
					event
					message
				1 -> value1
				2 -> value2
				3 -> value3
				4 -> value4

				type = "event"
				value1 = LED ID
					LED ID

			*/
			/*
			var allMuteStates = WindowsAudioHandler.GetAllMuteStates();
			foreach (var state in allMuteStates)
			{
				Console.WriteLine($"Process: {state.Item1}, Muted: {state.Item2}");
			}
			*/


			
			System.Threading.Thread.Sleep(100);
		}
	}

	public void SendHIDEvent(string[] arrHIDEvent)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < arrHIDEvent.Length; i++)
		{
			sb.Append(arrHIDEvent[i]);
			if (i < arrHIDEvent.Length - 1) // Don't add the delimiter after the last string
			{
				sb.Append(",");
			}
		}

		byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

		HDevice?.WriteEvent(bytes);
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

			if (configuration?.ActuatorGroups != null)
			{
				foreach (var actuatorGroup in configuration.ActuatorGroups)
				{
					Console.WriteLine($"[CTRL CFG] =======================================");
					Console.WriteLine($"[CTRL CFG] Actuator Group ID: {actuatorGroup.Id}");
					Console.WriteLine($"[CTRL CFG] Process Group: {actuatorGroup.ProcessGroup}");
					foreach (var actuator in actuatorGroup.Actuators)
					{
						Console.WriteLine($"[CTRL CFG] Actuator ID: {actuator.Id}");
						Console.WriteLine($"[CTRL CFG] Actuator Type: {actuator.ActuatorType}");
						Console.WriteLine($"[CTRL CFG] Actuator Physical Type: {actuator.PhysicalType}");
						Console.WriteLine($"[CTRL CFG] Actuator Min Value: {actuator.MinValue}");
						Console.WriteLine($"[CTRL CFG] Actuator Max Value: {actuator.MaxValue}");
					}
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
		// Controller = null;
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

	private void PollHIDDevice()
	{
		while (true)
		{
			Console.WriteLine("Polling HID Device...");
			if (!IsDeviceConnected())
			{
				Console.WriteLine("HID Device is not connected or has been disconnected.");
				TryReconnectDevice();
			}
			Thread.Sleep(1000);
		}
	}

	private bool IsDeviceConnected()
	{
		return HDevice != null && HDevice.IsConnected() && HDevice.IsDeviceOpen();
	}

	private void HandleDeviceDisconnection()
	{
		if (HDevice != null)
		{
			Console.WriteLine("[MAIN] Closing HID Device...");
			HDevice.StopReading();
			HDevice.CloseDevice();
			HDevice = null;
		}
	}

	private void InitializeHIDDevice()
	{
		HDevice = HManager.LoadDefaultDevice();
		if (HDevice != null)
		{
			HDevice.OpenDevice();
			if (HDevice.IsDeviceOpen())
			{
				HDevice.StartReading(Controller.ProcessHIDEvent);
			}
			else
			{
				Console.WriteLine("Failed to open initial HID Device. Waiting for connection...");
				HDevice = null; // Ensure HDevice is null if not open
			}
		}
		else
		{
			Console.WriteLine("Initial HID Device not found. Waiting for connection...");
			TryReconnectDevice();
		}
	}

	private void TryReconnectDevice()
	{
		while (!IsDeviceConnected())
		{
			Console.WriteLine("Attempting to reconnect HID Device...");
			HDevice = HManager.LoadDefaultDevice();

			if (HDevice != null)
			{
				HDevice.OpenDevice();
				if (HDevice.IsDeviceOpen())
				{
					Console.WriteLine("HID Device reconnected.");
					HDevice.StartReading(Controller.ProcessHIDEvent);
					break;
				}
			}

			Thread.Sleep(2000);
			HDevice = null;
		}
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

	Task<string> PrintMessageFunction(string message)
	{
		Console.WriteLine(message);

		var jsonElement = JsonDocument.Parse(message).RootElement;
		var requestid = jsonElement.GetProperty("id").GetString();
		JsonElement requestParams = jsonElement.GetProperty("requestParams");
		Console.WriteLine($"Request ID: {requestid}");
		Console.WriteLine(requestParams.GetProperty("test").GetString());
		Console.WriteLine(requestParams);

		Dictionary<string, List<WindowsAudioSession>> audioSessions = WindowsAudioHandler.GetAudioSessions();
		// string jsonStrAudioSessions = JsonSerializer.Serialize(audioSessions);

		var responseData = new Dictionary<string, object>
		{
			{ "test", "test" }
		};
		var response = new Dictionary<string, object>
		{
			{ "id", requestid },
			{ "response", audioSessions }
		};
		string responseString = JsonSerializer.Serialize(response);

		// await Task.CompletedTask;
		return Task.FromResult(responseString);
	}
}