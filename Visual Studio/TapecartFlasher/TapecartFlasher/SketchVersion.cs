using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapecartFlasher
{

	class SketchVersion
	{
		public enum ArduinoType
		{
			None = 0x00,
			ARDUINO_UNO = 0x01,
			ARDUINO_NANO = 0x02,
			ARDUINO_MEGA2560 = 0x03,
			//ARDUINO_PRO_MINI = 0x04
		}

		public int Major { get; set; }

		public int Minor { get; set; }

		public int Api { get; set; }

		public ArduinoType Type { get; set; }

		public string Filename { get; set; }

		public SketchVersion(int major, int minor, int api, ArduinoType type)
		{
			Major = major;
			Minor = minor;
			Api = api;
			Type = type;
		}

		public bool Equals(SketchVersion version)
		{
			return Major == version.Major && Minor == version.Minor && Api == version.Api;
		}

		public bool IsHigherAs(SketchVersion version)
		{
			if (version.Type != Type)
				return false; // different type, can not be compared
			if (Major > version.Major)
				return true;
			if (Api > version.Api)
				return true;
			if (Major == version.Major && Minor > version.Minor)
				return true;

			return false;
		}

		public int SortCompareTo(SketchVersion version)
		{
			if (Type == version.Type)
				return 0;
			if ((int)Type > (int)version.Type)
				return 1;
			else
				return 0;
		}

		public string VerStr
		{
			get
			{
				return $"{Major}.{Minor}";
			}
		}

		public string TypeStr
		{
			get
			{
				return ArduinoTypeToName(Type);
			}
		}

		public override string ToString()
		{
			return $"{Major}.{Minor} {Api} {Type} {Filename}";
		}

		public static string ArduinoTypeToName(ArduinoType arduinoType)
		{
			switch (arduinoType)
			{
				case ArduinoType.ARDUINO_UNO:
					return "UNO R3";
				case ArduinoType.ARDUINO_NANO:
					return "NANO R3";
				case ArduinoType.ARDUINO_MEGA2560:
					return "MEGA2560";
					//case ArduinoType.ARDUINO_PRO_MINI:
					//	return "PROMICRO";
			}
			return "unknown";
		}

		public static ArduinoType ArduinoNameToType(string name)
		{
			switch (name.ToUpper())
			{
				case "UNO":
					return ArduinoType.ARDUINO_UNO;
				case "NANO":
					return ArduinoType.ARDUINO_NANO;
				case "MEGA2560":
					return ArduinoType.ARDUINO_MEGA2560;
					//case "PROMINI":
					//	return ArduinoType.ARDUINO_PRO_MINI;
			}
			return ArduinoType.None;
		}

	}
}
