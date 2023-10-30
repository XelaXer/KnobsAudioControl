namespace Knobs.WindowsAudio
{
	public class WindowsAudioHandler
	{
		public WindowsAudioHandler()
		{
		}
	}

	
}
/*

var enumerator = new MMDeviceEnumerator();

// Get the default playback device
var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);

// Now let's enumerate over the audio sessions
var sessionManager = device.AudioSessionManager;
for (int i = 0; i < sessionManager.Sessions.Count; i++)
{
	using (var session = sessionManager.Sessions[i])
	{
		Console.WriteLine($"Session {i + 1}:");
		Console.WriteLine($"Process ID: {session.GetProcessID}");
		Console.WriteLine($"Session ID: {session.GetSessionIdentifier}");
		Console.WriteLine($"Session Instance ID: {session.GetSessionInstanceIdentifier}");
		Console.WriteLine($"State: {session.State}");

		var process = System.Diagnostics.Process.GetProcessById((int)session.GetProcessID);
		Console.WriteLine($"Process Name: {process.ProcessName}");
		// foreach (var property in session.GetType().GetProperties())
		// {
		// 	Console.WriteLine($"SimpleAudioVolume Property: {property.Name}, Type: {property.PropertyType}");
		// }
		/*
		foreach (var property in session.SimpleAudioVolume.GetType().GetProperties())
		{
			Console.WriteLine($"SimpleAudioVolume Property: {property.Name}, Type: {property.PropertyType}");
		}
		foreach (var property in session.AudioMeterInformation.GetType().GetProperties())
		{
			Console.WriteLine($"AudioMeterInformation Property: {property.Name}, Type: {property.PropertyType}");
		}
		*/
		Console.WriteLine("------------------------------");
	}
}
*/