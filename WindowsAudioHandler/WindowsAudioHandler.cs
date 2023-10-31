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
Process Cache:
            Dictionary<string, List<int>> processNameToProcessId = new Dictionary<string, List<int>>();
			Dictionary<int, object> processIdToSystemProcess = new Dictionary<int, object>();

			string processName = "exampleProcess";
            int processId = 12345;
            if (!processNameToProcessId.ContainsKey(processName))
            {
                processNameToProcessId[processName] = new List<int>();
            }
            processNameToProcessId[processName].Add(processId);
            
            // To iterate through items
            foreach (var entry in processNameToProcessId)
            {
                string key = entry.Key;
                foreach (var value in entry.Value)
                {
                    Console.WriteLine($"{key}: {value}");
                }
            }

*/


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
		foreach (var property in session.SimpleAudioVolume.GetType().GetProperties())
		{
			Console.WriteLine($"SimpleAudioVolume Property: {property.Name}, Type: {property.PropertyType}");
		}
		foreach (var property in session.AudioMeterInformation.GetType().GetProperties())
		{
			Console.WriteLine($"AudioMeterInformation Property: {property.Name}, Type: {property.PropertyType}");
		}

		Console.WriteLine("------------------------------");
	}
}
*/