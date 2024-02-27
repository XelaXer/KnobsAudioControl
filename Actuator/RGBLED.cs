using IHID.HIDDevice;
using Knobs.Controller;
using System.Text;

namespace Knobs.Actuators
{
	public class RGBLED : Actuator
	{
		private int r = 0;
		private int g = 0;
		private int b = 0;

		public RGBLED(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType) : base(id, value, minValue, maxValue, physicalType, actuatorType)
		{
		}
		public void SetColor(int r, int g, int b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}
		public override void ProcessEvent(ControllerEvent cEvent)
		{
			throw new System.NotImplementedException();
		}
		public override void UpdateActuatorState(IHIDDevice? device)
		{
			string[] arrHIDEvent = new string[] { GetId().ToString() , r.ToString(), g.ToString(), b.ToString() };
			SendHIDEvent(arrHIDEvent, device);
		}
		static public void SendHIDEvent(string[] arrHIDEvent, IHIDDevice? device)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < arrHIDEvent.Length; i++)
			{
				sb.Append(arrHIDEvent[i]);
				if (i < arrHIDEvent.Length - 1) sb.Append(',');
			}
			byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
			device?.WriteEvent(bytes);
		}
	}
}