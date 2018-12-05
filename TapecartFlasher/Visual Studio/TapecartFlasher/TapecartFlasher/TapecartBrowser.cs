using DgCommon;

namespace TapecartFlasher
{
	class TapecartBrowser
	{
		public static string GetInfo(byte[] image, int offset)
		{
			string signature = ByteArray.ReadAsciiString(image, offset + 0x02, 13);
			if (signature != "TapcrtFileSys")
				return "no Tapecart filesystem";

			string name = ByteArray.ReadAsciiString(image, offset + 0x0F, 16);

			int count = 0;
			while (count < 128)
			{
				int addr = offset + 0x0F + 32 * count;
				if (image[addr + 16] == 0xFF)
					break;
				count += 32;
			}
			return $"{name}, {count} entries";
		}
	}
}
