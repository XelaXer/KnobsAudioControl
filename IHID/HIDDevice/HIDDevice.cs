using HidLibrary;
using System.Text;

namespace IHID.HIDDevice
{
	class IHIDDevice
	{
		HidDevice? Device;
		int VendorID;
		int ProductID;

		public IHIDDevice(int vendorid, int productid)
		{
			VendorID = vendorid;
			ProductID = productid;
		}

		public void OpenDevice()
		{
			var device = HidDevices.Enumerate(VendorID, ProductID).FirstOrDefault();
			if (device == null)
			{
				Console.WriteLine($"[HID DEVICE] Vendor ID: {VendorID}, Product ID: {ProductID} not found.");
				return;
			}
			Device = device;
			Device.OpenDevice();
		}

		public void CloseDevice()
		{
			Device?.CloseDevice();
		}

		public void StartReading(Action<byte[]> callback)
		{
			Task.Run(() => ReadLoop(callback));
		}

		private void ReadLoop(Action<byte[]> callback)
		{
			while (true)
			{
				if (Device == null) continue;
				var report = Device.ReadReport();
				if (report.Exists)
				{
					byte[] receivedData = report.Data;
					callback(receivedData);
				}
				// Thread.Sleep(5);
			}
		}
	}
}



















/*
	while (true)
	{
		if (Device == null) continue;
		var report = Device.ReadReport();
		if (report.Exists)
		{
			byte[] receivedData = report.Data;
			string type = Encoding.ASCII.GetString(receivedData, 0, 10).TrimEnd('\0');
			int value1 = BitConverter.ToInt32(receivedData, 10);
			int value2 = BitConverter.ToInt32(receivedData, 14);
			int value3 = BitConverter.ToInt32(receivedData, 18);

			SHIDEvent hidevent = new SHIDEvent(type, value1, value2, value3);
			callback(hidevent);
		}
		// Thread.Sleep(5);
	}

	struct SHIDEvent
	{
		readonly string type;
		readonly int value1;
		readonly int value2;
		readonly int value3;

		public SHIDEvent(string type, int value1, int value2, int value3)
		{
			this.type = type;
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}
	};
*/