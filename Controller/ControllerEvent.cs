	
namespace Knobs.Controller
{
	public class ControllerEvent
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
}