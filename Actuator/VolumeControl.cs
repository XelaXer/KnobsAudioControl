using Knobs.WindowsAudio;
using Knobs.Controller;
namespace Knobs.Actuator
{
	public class VolumeControl : AudioActuator
	{
		float Volume { get; set;}
		public VolumeControl(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler) 
			: base(id, value, minValue, maxValue, physicalType, actuatorType, processNames, windowsAudioHandler)
		{
		}

		public override void ProcessEvent(ControllerEvent cEvent)
		{
			SetValue(cEvent.Value1);
			
			// SetVolume()
			throw new System.NotImplementedException();
		}

		void SetVolume(float volume)
		{
			Volume = volume;
			foreach (var processName in ProcessNames)
            {
                // WindowsAudioHandler.SetVolumeByProcessId(processName, volume);
            }
		}
	}
}