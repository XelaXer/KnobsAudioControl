using System.Diagnostics;
using System.Text;
using Knobs.WindowsAudio;
using Knobs.Actuators;

namespace Knobs.Controller
{
	class Controller
	{
		private Dictionary<int, Actuator> MActuators;
		private WindowsAudioHandler WAudioHandler;

		public Controller(ControllerConfiguration ctrlCfg, WindowsAudioHandler audioHandler)
		{
			WAudioHandler = audioHandler;
			MActuators = new Dictionary<int, Actuator>();

			// Create actuators
			foreach (ActuatorConfig actuatorCfg in ctrlCfg.Actuators)
			{
				Console.WriteLine($"[CONTROLLER] [INFO] Creating actuator with ID {actuatorCfg.Id} and type {actuatorCfg.ActuatorType}");

				Actuator actuator = null;
				switch (actuatorCfg.ActuatorType)
				{
					case "volume_knob":
						// Get process group
						List<string> processNames = new();
						foreach (ProcessGroup processGroup in ctrlCfg.ProcessGroups)
						{
							if (processGroup.GroupName == actuatorCfg.ActuatorTypeSettings.ProcessGroup)
							{
								foreach (Process process in processGroup.Processes)
								{
									processNames.Add(process.ProcessName);
								}
							}
						}
						// Create Actuator
						actuator = new VolumeControl(actuatorCfg.Id, actuatorCfg.MaxValue, actuatorCfg.MinValue, actuatorCfg.MaxValue,  actuatorCfg.PhysicalType, 
							actuatorCfg.ActuatorType, processNames, WAudioHandler);
						break;
					default:
						Console.WriteLine($"[CONTROLLER] [ERROR] Actuator type {actuatorCfg.ActuatorType} is not supported.");
						break;
				}
				if (actuator != null)
				{
					MActuators.Add(actuatorCfg.Id, actuator);
				}
			}
		}

		public void ProcessHIDEvent(byte[] receivedData)
		{
			string type = Encoding.ASCII.GetString(receivedData, 0, 10).TrimEnd('\0');
			Console.WriteLine($"[CONTROLLER] [HID] Received {type}");
			if (type == "event")
			{
				int value1 = BitConverter.ToInt32(receivedData, 10);
				int value2 = BitConverter.ToInt32(receivedData, 14);
				int value3 = BitConverter.ToInt32(receivedData, 18);
				ControllerEvent hidEvent = new ControllerEvent(type, value1, value2, value3);
				ProcessControllerEvent(hidEvent);
				return;
			}
			if (type == "message")
			{
				string message = Encoding.ASCII.GetString(receivedData, 10, 10).TrimEnd('\0');
				return;
			}
		}

		public void ProcessControllerEvent(ControllerEvent e)
		{
			Console.WriteLine($"[CONTROLLER] [EVENT] {e.Type}, {e.Value1}, {e.Value2}, {e.Value3}");
			if (e.Type == "event")
			{
				if (MActuators.ContainsKey(e.Value1) == false)
				{
					Console.WriteLine($"[CONTROLLER] [EVENT] Actuator with ID {e.Value1} does not exist.");
					return;
				}
				MActuators[e.Value1].ProcessEvent(e);
			}
		}
	};
}

/*
	public struct ControllerConfiguration
	{
		public List<object> Actuators;
		public List<object> ProcessGroups;
	};
*/