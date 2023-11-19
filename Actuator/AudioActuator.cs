using Knobs.WindowsAudio;
namespace Knobs.Actuators
{
	public abstract class AudioActuator : Actuator
	{
		protected List<string> ProcessNames { get; set; }
		protected List<int>? ActiveProcessIds;
		protected WindowsAudioHandler WindowsAudioHandler { get; private set; }

		public AudioActuator(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler) 
			: base(id, value, minValue, maxValue, physicalType, actuatorType)
		{
			ProcessNames = processNames;
			WindowsAudioHandler = windowsAudioHandler;
		}
	}
}