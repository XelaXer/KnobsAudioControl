namespace Knobs.Actuator
{
	public class Actuator
	{
		int id;
		int value;
		int minValue;
		int maxValue;
		string physicalType;
		string actuatorType;

		public Actuator(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType)
		{
			this.id = id;
			this.value = value;
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.physicalType = physicalType;
			this.actuatorType = actuatorType;
		}
	}

	
}