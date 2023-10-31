using Knobs.WindowsAudio;
namespace Knobs.Actuator
{
	public abstract class AudioActuator : Actuator
	{
		protected List<string> ProcessNames { get; set; }
		protected WindowsAudioHandler WindowsAudioHandler { get; private set; }

		public AudioActuator(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler) 
			: base(id, value, minValue, maxValue, physicalType, actuatorType)
		{
			this.ProcessNames = processNames;
			this.WindowsAudioHandler = windowsAudioHandler;
		}
	}
}