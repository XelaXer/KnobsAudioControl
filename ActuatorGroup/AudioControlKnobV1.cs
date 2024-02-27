using Knobs.Actuators;
using IHID.HIDDevice;
namespace Knobs.ActuatorGroups
{
	public class AudioControlKnobV1 : ActuatorGroup
	{
		ToggleMute toggleMute;
		VolumeControl volumeControl;
		RGBLED rgbLed;

		public AudioControlKnobV1(int id, string type, string? processGroup, List<Actuator> actuators) : base(id, type, processGroup, actuators)
		{
			// Loop through MActuatorListByType map and print out the Class name of the actuator
			foreach (var actuator in actuators)
			{
				Console.WriteLine($"[ACTUATOR_GROUP] [INFO] Adding actuator with ID {actuator.GetId()} and type {actuator.GetType().Name} to actuator group with ID");
				if (actuator.GetType().Name == "ToggleMute")
				{
					toggleMute = (ToggleMute)actuator;
				}
				else if (actuator.GetType().Name == "VolumeControl")
				{
					volumeControl = (VolumeControl)actuator;
				}
				else if (actuator.GetType().Name == "RGBLED")
				{
					rgbLed = (RGBLED)actuator;
				}
			}
			// this.MActuatorListByType
		}

		public override void UpdateState(IHIDDevice device)
		{
			bool liveMute = toggleMute.GetLiveMuteState();
			bool savedMute = toggleMute.GetSavedMuteState();

			if (liveMute != savedMute)
			{
				toggleMute.UpdateMuteState(liveMute);
				if (toggleMute.GetSavedMuteState() == true)
				{
					rgbLed.SetColor(255, 255, 255);
				}
				else
				{
					rgbLed.SetColor(0, 0, 0);
				}
				rgbLed.UpdateActuatorState(device);
			}
		}
	}
}