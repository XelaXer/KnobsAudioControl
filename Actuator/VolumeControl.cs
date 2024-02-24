using Knobs.WindowsAudio;
using Knobs.Controller;
using IHID.HIDDevice;
// using NAudio.MediaFoundation;
namespace Knobs.Actuators
{
	public class VolumeControl : AudioActuator
	{
		float Volume { get; set; }
		public VolumeControl(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler) 
			: base(id, value, minValue, maxValue, physicalType, actuatorType, processNames, windowsAudioHandler)
		{
		}

		public override void ProcessEvent(ControllerEvent cEvent)
		{
			SetValue(cEvent.Value2);
			float volume = NormalizeFloat(cEvent.Value2, minValue, maxValue, 0, 1);
			Console.WriteLine($"Volume: {volume}");
			SetVolume(volume);
		}

		public override void UpdateActuatorState(IHIDDevice device)
		{
			return;
		}

		void SetVolume(float volume)
		{
			Volume = volume;
			// TODO: change this to use ActiveProcessIds so it doesn't loop over all processes
			//	ex. the "game" process group might have 1000 process names, but only 1 is active
			foreach (var processName in ProcessNames)
            {
                WindowsAudioHandler.SetVolumeByProcessName(processName, volume);
            }
		}
	}
}