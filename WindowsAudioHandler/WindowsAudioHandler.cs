using NAudio.CoreAudioApi;
using System.Collections.Generic;

namespace Knobs.WindowsAudio
{
	public struct WindowsAudioSession
	{
		public AudioSessionControl AudioSessionControlObject;
		public System.Diagnostics.Process SystemProcessObject;
		public int ProcessId;
		public string ProcessName;
		// Can access state via: AudioSessionObject.State
	}

	public class WindowsAudioHandler
	{
		MMDeviceEnumerator enumerator;
		Dictionary<string, WindowsAudioSession> CachedSessions;
		DateTime TimeLastUpdatedSessions;
		public WindowsAudioHandler()
		{
			enumerator = new ();
			CachedSessions = GetAudioSessions();
		}

		public Dictionary<string, WindowsAudioSession> GetAudioSessions()
		{
			Dictionary<string, WindowsAudioSession> sessions = new();

			MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
			AudioSessionManager sessionManager = device.AudioSessionManager;
			for (int i = 0; i < sessionManager.Sessions.Count; i++)
			{
				AudioSessionControl session = sessionManager.Sessions[i];
				var process = System.Diagnostics.Process.GetProcessById((int)session.GetProcessID);

				var wAudioSession = new WindowsAudioSession
				{
					AudioSessionControlObject = session,
					SystemProcessObject = process,
					ProcessId = (int)session.GetProcessID,
					ProcessName = process.ProcessName
				};
				sessions.Add(wAudioSession.ProcessName, wAudioSession);
			}
			TimeLastUpdatedSessions = DateTime.UtcNow;
			return sessions;
		}

		public void SetVolumeByProcessName(string processName, float volume)
		{
			if (DateTime.UtcNow - TimeLastUpdatedSessions > TimeSpan.FromSeconds(2))
			{
				Console.WriteLine("[WINDOWS AUDIO HANDLER] [CACHE] Updating process cache");
				CachedSessions = GetAudioSessions();
			}
			if (CachedSessions.ContainsKey(processName) == false) return;
			var session = CachedSessions[processName].AudioSessionControlObject;
			var simpleAudioVolume = session.SimpleAudioVolume;
			simpleAudioVolume.Volume = volume;
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