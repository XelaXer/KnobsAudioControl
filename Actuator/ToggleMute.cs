using Knobs.WindowsAudio;
using Knobs.Controller;
// using NAudio.MediaFoundation;
namespace Knobs.Actuators
{
	public class ToggleMute : AudioActuator
	{
		bool MuteState { get; set; }
		public ToggleMute(int id, int value, int minValue, int maxValue, string physicalType, string actuatorType, List<string> processNames, WindowsAudioHandler windowsAudioHandler)
			: base(id, value, minValue, maxValue, physicalType, actuatorType, processNames, windowsAudioHandler)
		{
		}

		public override void ProcessEvent(ControllerEvent cEvent)
		{
			bool newMuteState = ToggleMuteState();
			int newValue = newMuteState ? 1 : 0;
			SetValue(newValue);
			Console.WriteLine($"Mute: {newMuteState}");
		}
		bool ToggleMuteState()
		{
			bool muteState = false;
			foreach (var processName in ProcessNames)
			{
				muteState = WindowsAudioHandler.GetMuteByProcessName(processName);
				break;
			}
			MuteState = muteState;
			// TODO: change this to use ActiveProcessIds so it doesn't loop over all processes
			//	ex. the "game" process group might have 1000 process names, but only 1 is active
			foreach (var processName in ProcessNames)
			{
				WindowsAudioHandler.SetMuteByProcessName(processName, muteState);
			}
			return muteState;
		}
	}
}