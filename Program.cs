using System.Collections;
using NAudio.CoreAudioApi;

using IHID.HIDManager;
using IHID.HIDDevice;
using Knobs.Controller;

static void LoadEnvironmentVariables()
{
	var lines = File.ReadAllLines(".env");
	foreach (var line in lines)
	{
		var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 2)
		{
			Environment.SetEnvironmentVariable(parts[0], parts[1], EnvironmentVariableTarget.Process);
		}
	}
}

LoadEnvironmentVariables();

Controller Controller = new Controller();
HIDManager HManager = new HIDManager();
IHIDDevice HDevice = HManager.LoadDefaultDevice();
HDevice.OpenDevice();
HDevice.StartReading(Controller.ProcessHIDEvent);


while (true)
{
	// System.Threading.Thread.Sleep(100);
}
























/*
while (true)
{
	var report = device.ReadReport();
	if (report.Exists)
	{

		byte[] receivedData = report.Data;
		int val1 = BitConverter.ToInt32(receivedData, 0);
		int val2 = BitConverter.ToInt32(receivedData, 4);
		int val3 = BitConverter.ToInt32(receivedData, 8);

		string receivedString = Encoding.ASCII.GetString(receivedData, 12, 10).TrimEnd('\0');

		Console.WriteLine($"Received Integers: {val1}, {val2}, {val3}");
		Console.WriteLine("Received String: " + receivedString);
	}
	System.Threading.Thread.Sleep(100);
}
*/

/*

if (device == null)
{
	Console.WriteLine("Device not found.");
	return;
}
device.OpenDevice();
while (true)
{
	var report = device.ReadReport();
	if (report.Exists)
	{
		Console.WriteLine("Received: " + report.Data.Skip(1));
		// Skip the first byte and take bytes until the first 0x00 byte (end of string)
		var stringData = Encoding.ASCII.GetString(report.Data.Skip(1).TakeWhile(b => b != 0x00).ToArray());
		Console.WriteLine("Received String: " + stringData);
		byte[] receivedData = report.Data;
        int val1 = BitConverter.ToInt32(receivedData, 0);
        int val2 = BitConverter.ToInt32(receivedData, 4);
        int val3 = BitConverter.ToInt32(receivedData, 8);

        string receivedString = Encoding.ASCII.GetString(receivedData, 12, 10).TrimEnd('\0');

        Console.WriteLine($"Received Integers: {val1}, {val2}, {val3}");
        Console.WriteLine("Received String: " + receivedString);
	}
	System.Threading.Thread.Sleep(100);
}
device.CloseDevice();

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
		// foreach (var property in session.GetType().GetProperties())
		// {
		// 	Console.WriteLine($"Property: {property.Name}, Type: {property.PropertyType}");
		// }
		Console.WriteLine($"Session {i + 1}:");
		Console.WriteLine($"  Display Name: {session.DisplayName}");
		Console.WriteLine($"  State: {session.State}");
		
		foreach (var property in session.SimpleAudioVolume.GetType().GetProperties())
		{
			Console.WriteLine($"SimpleAudioVolume Property: {property.Name}, Type: {property.PropertyType}");
		}
		foreach (var property in session.AudioMeterInformation.GetType().GetProperties())
		{
			Console.WriteLine($"AudioMeterInformation Property: {property.Name}, Type: {property.PropertyType}");
		}

		// SimpleAudioVolume.Volume
		// SimpleAudioVolume.Mute

		
		Console.WriteLine("------------------------------");
	}
}

void printPropertyInfo(string name, object obj)
{
	Console.WriteLine($"------------ {name} Info ------------------");
	foreach (var property in obj.GetType().GetProperties())
	{
		Console.WriteLine($"Property: {property.Name}, Type: {property.PropertyType}");
	}
	Console.WriteLine("-------------------------------------------");
}
*/



/*
Property: AudioMeterInformation, Type: NAudio.CoreAudioApi.AudioMeterInformation
Property: SimpleAudioVolume, Type: NAudio.CoreAudioApi.SimpleAudioVolume
Property: State, Type: NAudio.CoreAudioApi.Interfaces.AudioSessionState
Property: DisplayName, Type: System.String
Property: IconPath, Type: System.String
Property: GetSessionIdentifier, Type: System.String
Property: GetSessionInstanceIdentifier, Type: System.String
Property: GetProcessID, Type: System.UInt32
Property: IsSystemSoundsSession, Type: System.Boolean
*/

/*
SimpleAudioVolume Property: Volume, Type: System.Single
SimpleAudioVolume Property: Mute, Type: System.Boolean
AudioMeterInformation Property: PeakValues, Type: NAudio.CoreAudioApi.AudioMeterInformationChannels
AudioMeterInformation Property: HardwareSupport, Type: NAudio.CoreAudioApi.EEndpointHardwareSupport
AudioMeterInformation Property: MasterPeakValue, Type: System.Single
*/