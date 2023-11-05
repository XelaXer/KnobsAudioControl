using System.Diagnostics;
using System.Text;
using Knobs.Actuators;

namespace Knobs.Controller
{
	class Controller
	{
		private Dictionary<int, Actuator> MActuators;

		public Controller(Dictionary<int, Actuator> actuators)
		{
			MActuators = actuators;
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