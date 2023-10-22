using System.Diagnostics;
using System.Text;

namespace KController
{
	class ControllerEvent
	{
		private readonly string type;
		private readonly int value1;
		private readonly int value2;
		private readonly int value3;

		public string Type => type;
		public int Value1 => value1;
		public int Value2 => value2;
		public int Value3 => value3;

		public ControllerEvent(string type, int value1, int value2, int value3)
		{
			this.type = type;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

	};

	class Controller
	{

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
			Console.WriteLine($"Received Controller Event: {e.Type}, {e.Value1}, {e.Value2}, {e.Value3}");
		}
	};
}
