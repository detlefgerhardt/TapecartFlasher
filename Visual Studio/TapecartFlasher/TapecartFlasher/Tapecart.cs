using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TapecartFlasher
{
	static class Tapecart
	{
		public const int BLOCK_SIZE = 256;
		public const int ERASE_SIZE = 4096;
		public const int ERASE_64K = 0x10000;

		public const int MAX_FLASH_SIZE = 0x200000; // 2 MByte
		public const int HEADER_SIZE = 216;

		public const int OFFSET_VERSION = 0x10;
		public const int OFFSET_LOADER_OFFSET = 0x12;
		public const int OFFSET_LOADER_LENGTH = 0x14;
		public const int OFFSET_LOADER_CALLADDR = 0x16;
		public const int OFFSET_LOADER_NAME = 0x18;
		public const int OFFSET_FLAGS = 0x28;
		public const int OFFSET_LOADER = 0x29;
		public const int OFFSET_FLASHSIZE = 0xD4;

		public const int LOADER_LENGTH = 171;
		public const int LOADER_OFFSET = 0x0000;
		public const int LOADER_CALLADDR = 0x2800;
		public const int LOADER_NAME_LENGTH = 16;
		public const int VERSION = 0x0001;

		public const string TAPECART_SIGNATURE = "tapecartImage\x0d\x0a\x1a";

		public const string BROWSER_SIGNATURE = "TapcrtFileSys";

		private const string BROWSER_NAME = "Browser-P1X3Lnet";
		private const string BROWSER_FILENAME = "browser.bin";
		private const int BROWSER_FILESYS_SIZE = 0x1000;
		private const int BROWSER_SIZE = 0x4000;
	}
}
