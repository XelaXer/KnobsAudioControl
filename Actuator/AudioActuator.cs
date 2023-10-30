using Knobs.WindowsAudio;
namespace Knobs.Actuator
{
	public abstract class AudioActuator : Actuator
	{
		List<string> processNames;
		WindowsAudioHandler WindowsAudioHandler;

		public AudioActuator(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler) 
			: base(id, value, minValue, maxValue, physicalType, actuatorType)
		{
			this.processNames = processNames;
			this.WindowsAudioHandler = windowsAudioHandler;
		}
	}
}