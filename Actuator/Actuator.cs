using IHID.HIDDevice;
using Knobs.Controller;
namespace Knobs.Actuators
{
	public abstract class Actuator
	{
		private int id;
		private int value;
		protected int minValue;
		protected int maxValue;
		private string physicalType;
		private string actuatorType { get; }

		public Actuator(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType)
		{
			this.id = id;
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.physicalType = physicalType;
			this.actuatorType = actuatorType;
		}

		public abstract void ProcessEvent(ControllerEvent cEvent);

		public abstract void UpdateActuatorState(IHIDDevice? device);

		public void SetValue(int value)
		{
			this.value = value;
		}

		public int GetId()
		{
			return id;
		}

		public string GetActuatorType()
		{
			return actuatorType;
		}

		public static float NormalizeFloat(float value, float inMin, float inMax, float outMin, float outMax)
		{
			return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
		}
	}
	
}