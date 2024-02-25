using System.Diagnostics;
using System.Text;
using Knobs.WindowsAudio;
using Knobs.Actuators;
using Knobs.ActuatorGroups;

namespace Knobs.Controller
{
	class Controller : IDisposable
	{
		private Dictionary<int, Actuator> MActuators;
		private Dictionary<int, ActuatorGroup> MActuatorGroups;
		private WindowsAudioHandler WAudioHandler;
		private bool disposed = false;

		// Long term TODO: create thread that updates mute state of ToggleMute actuators
		// SyncActuatorsWithOSState()
		// currently not needed, do event driven for state changes

		public Controller(ControllerConfiguration ctrlCfg, WindowsAudioHandler audioHandler)
		{
			WAudioHandler = audioHandler;
			MActuators = new Dictionary<int, Actuator>();

			foreach (ActuatorGroupConfig actuatorGroupCfg in ctrlCfg.ActuatorGroups)
			{
				Console.WriteLine($"[CONTROLLER] [INFO] Creating actuator group with ID {actuatorGroupCfg.Id} and process group {actuatorGroupCfg.ProcessGroup}");

				ActuatorGroup actuatorGroup = new ActuatorGroup(
					actuatorGroupCfg.Id,
					actuatorGroupCfg.ProcessGroup,
					new List<Actuator>()
				);

				// Get array of process names from process group for the actuators in the group
				List<string> processNames = new();
				foreach (ProcessGroup processGroup in ctrlCfg.ProcessGroups)
				{
					if (processGroup.GroupName != actuatorGroupCfg.ProcessGroup) continue;
					foreach (Process process in processGroup.Processes)
					{
						processNames.Add(process.ProcessName);
					}
				}

				// Create actuators
				foreach (ActuatorConfig actuatorCfg in actuatorGroupCfg.Actuators)
				{
					Console.WriteLine($"[CONTROLLER] [INFO] Creating actuator with ID {actuatorCfg.Id} and type {actuatorCfg.ActuatorType}");
					Actuator actuator = null;
					switch (actuatorCfg.ActuatorType)
					{
						case "volume_knob":
							actuator = new VolumeControl(
								actuatorCfg.Id,
								actuatorCfg.MaxValue,
								actuatorCfg.MinValue,
								actuatorCfg.MaxValue,
								actuatorCfg.PhysicalType,
								actuatorCfg.ActuatorType,
								processNames,
								WAudioHandler
							);
							break;
						case "toggle_mute":
							actuator = new ToggleMute(
								actuatorCfg.Id,
								actuatorCfg.MaxValue,
								actuatorCfg.MinValue,
								actuatorCfg.MaxValue,
								actuatorCfg.PhysicalType,
								actuatorCfg.ActuatorType,
								processNames,
								WAudioHandler
							);
							break;
						case "led":
							/*
							actuator = new Led(
								actuatorCfg.Id,
								actuatorCfg.MaxValue,
								actuatorCfg.MinValue,
								actuatorCfg.MaxValue,
								actuatorCfg.PhysicalType,
								actuatorCfg.ActuatorType,
								processNames,
								WAudioHandler
							);
							*/
							break;
						default:
							Console.WriteLine($"[CONTROLLER] [ERROR] Actuator type {actuatorCfg.ActuatorType} is not supported.");
							break;
					}
					if (actuator != null)
					{
						actuatorGroup.AddActuator(actuator);
						MActuators.Add(actuatorCfg.Id, actuator);
					}
				}
			}

			Thread updateThread = new Thread(new ThreadStart(BackgroundUpdateLoop))
			{
				IsBackground = true
			};
			updateThread.Start();
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

		public void UpdateControllerState()
		{
			foreach (var actuatorPair in MActuators)
			{
				var actuator = actuatorPair.Value;
				// bool isMuted = actuator.UpdateActuatorState();
				// Console.WriteLine($"Actuator ID: {actuatorPair.Key}, Muted: {isMuted}");
			}
		}

		private void BackgroundUpdateLoop()
		{
			while (!disposed)
			{
				UpdateControllerState();
				Thread.Sleep(150);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (WAudioHandler != null)
					{
						WAudioHandler.Dispose();
						WAudioHandler = null;
					}

					foreach (var actuator in MActuators.Values)
					{
						if (actuator is IDisposable disposableActuator)
						{
							disposableActuator.Dispose();
						}
					}
					MActuators.Clear();

					foreach (var actuatorGroup in MActuatorGroups.Values)
					{
						if (actuatorGroup is IDisposable disposableActuator)
						{
							disposableActuator.Dispose();
						}
					}
					MActuatorGroups.Clear();
				}
				MActuators = null;
				MActuatorGroups = null;
				disposed = true;
			}
		}
		~Controller()
		{
			Dispose(false);
		}
	};
}