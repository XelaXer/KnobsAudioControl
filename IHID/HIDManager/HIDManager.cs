using HidLibrary;
using IHID.HIDDevice;

namespace IHID.HIDManager 
{
	class HIDManager
	{
		int defaultVendorId = 0x2341;
		int defaultProductId = 0x8036;

		public HIDManager()
		{
			string? envDefaultVendorId = Environment.GetEnvironmentVariable("VENDORIDDEFAULT");
			string? envDefaultProductId = Environment.GetEnvironmentVariable("PRODUCTIDDEFAULT");

			if (envDefaultVendorId != null)
			{
				Console.WriteLine("Using VENDORIDDEFAULT env variable for Default Vendor ID.");
				try
				{
					int defaultVendorId = Convert.ToInt32(envDefaultVendorId, 16);
				}
				catch (FormatException e)
				{
					Console.WriteLine($"VENDORIDDEFAULT Hex Conversion failed: {e.Message}");
				}
			}
			if (envDefaultProductId != null)
			{
				Console.WriteLine("Using PRODUCTIDDEFAULT env variable for Default Product ID.");
				try
				{
					int defaultProductId = Convert.ToInt32(envDefaultProductId, 16);
				}
				catch (FormatException e)
				{
					Console.WriteLine($"PRODUCTIDDEFAULT Hex Conversion failed: {e.Message}");
				}
			}

			LoadDefaultDevice();
		}

		// Return custom class for HIDDevice
		public IHIDDevice LoadDefaultDevice()
		{
			return new IHIDDevice(defaultVendorId, defaultProductId);
		}
	}
}