using HidLibrary;

namespace IHID.HIDDevice
{
	class IHIDDevice
	{
		HidDevice? Device;
		int VendorID;
		int ProductID;

		private CancellationTokenSource? _cancellationTokenSource;

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
			_cancellationTokenSource = new CancellationTokenSource();
			var cancellationToken = _cancellationTokenSource.Token;

			Task.Run(() => ReadLoop(callback, cancellationToken), cancellationToken);
		}

		private void ReadLoop(Action<byte[]> callback, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				if (Device == null) continue;
				var report = Device.ReadReport();
				if (report.Exists)
				{
					byte[] receivedData = report.Data;
					callback(receivedData);
				}
				if (token.IsCancellationRequested) break;
				// Thread.Sleep(5);
			}
		}

		public void StopReading()
		{
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null; // reset the token source for future use if needed
			}
		}
	}
}