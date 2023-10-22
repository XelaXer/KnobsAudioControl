using System.Diagnostics;
using System.Text;

namespace Knobs.Controller
{
	class Controller
	{
		Dictionary<int, object> mActuators = new Dictionary<int, object>();

		public Controller()
		{

		}

		public void ProcessHIDEvent(byte[] receivedData)
		{
			string type = Encoding.ASCII.GetString(receivedData, 0, 10).TrimEnd('\0');
			// Console.WriteLine($"Received HID Event: {type}");
			if (type == "event")
			{
				int value1 = BitConverter.ToInt32(receivedData, 10);
				int value2 = BitConverter.ToInt32(receivedData, 14);
				int value3 = BitConverter.ToInt32(receivedData, 18);
				ControllerEvent hidevent = new ControllerEvent(type, value1, value2, value3);
				ProcessControllerEvent(hidevent);
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

		}
	};
}
