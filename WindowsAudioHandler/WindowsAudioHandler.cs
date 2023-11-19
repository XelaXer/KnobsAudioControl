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

		public void Dispose()
		{
			if (AudioSessionControlObject != null)
			{
				AudioSessionControlObject.Dispose();
			}
		}
	}

	public class WindowsAudioHandler : IDisposable
	{
		MMDeviceEnumerator enumerator;
		Dictionary<string, List<WindowsAudioSession>> CachedSessions;
		DateTime TimeLastUpdatedSessions;
		private bool disposed = false;

		public WindowsAudioHandler()
		{
			enumerator = new();
			CachedSessions = GetAudioSessions();
		}

		public Dictionary<string, List<WindowsAudioSession>> GetAudioSessions()
		{
			Dictionary<string, List<WindowsAudioSession>> sessions = new();

			using (var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console))
			{
				AudioSessionManager sessionManager = device.AudioSessionManager;

				for (int i = 0; i < sessionManager.Sessions.Count; i++)
				{
					AudioSessionControl session = sessionManager.Sessions[i];
					var process = System.Diagnostics.Process.GetProcessById((int)session.GetProcessID);
					Console.WriteLine($"Process Name: {process.ProcessName}");
					var wAudioSession = new WindowsAudioSession
					{
						AudioSessionControlObject = session,
						SystemProcessObject = process,
						ProcessId = (int)session.GetProcessID,
						ProcessName = process.ProcessName
					};
					if (sessions.ContainsKey(wAudioSession.ProcessName) == false) 
					{
						sessions.Add(wAudioSession.ProcessName, new List<WindowsAudioSession>());
					}
					sessions[wAudioSession.ProcessName].Add(wAudioSession);
				}
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
			for (int i = 0; i < CachedSessions[processName].Count; i++)
			{
				var session = CachedSessions[processName][i].AudioSessionControlObject;
				var simpleAudioVolume = session.SimpleAudioVolume;
				simpleAudioVolume.Volume = volume;
			}
			// var session = CachedSessions[processName].AudioSessionControlObject;
			// var simpleAudioVolume = session.SimpleAudioVolume;
			// simpleAudioVolume.Volume = volume;
		}

		// TODO: AN: Cleanup and pick method
		public void Dispose()
		{
			Console.WriteLine("[WINDOWS AUDIO HANDLER] [INFO] Starting Disposing WindowsAudioHandler");
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Console.WriteLine("[WINDOWS AUDIO HANDLER] [INFO] Disposing WindowsAudioHandler");
			if (!disposed)
			{
				if (disposing)
				{
					// Dispose managed state (managed objects)
					// Dispose all AudioSessionControlObjects in the cache
					foreach (var sessionList in CachedSessions.Values)
					{
						foreach (var session in sessionList)
						{
							session.Dispose(); // This will call Dispose on WindowsAudioSession struct, make sure all disposables in it are handled
						}
					}
					CachedSessions.Clear();
				}

				// Free unmanaged resources (unmanaged objects) and override finalizer
				// Set large fields to null

				disposed = true;
			}
		}

		~WindowsAudioHandler() // Finalizer
		{
			Console.WriteLine("[WINDOWS AUDIO HANDLER] [INFO] Destructing.");
			Dispose(false);
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