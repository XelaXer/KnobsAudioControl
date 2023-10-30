using Knobs.Controller;
namespace Knobs.Actuator
{
	public abstract class Actuator
	{
		private int id;
		private int value;
		private int minValue;
		private int maxValue;
		private string physicalType;
		private string actuatorType;

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

		public void SetValue(int value)
		{
			this.value = value;
		}
	}
	
}