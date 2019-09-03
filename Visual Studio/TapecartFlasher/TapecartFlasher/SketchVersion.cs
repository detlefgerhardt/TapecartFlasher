using ArduinoUploader.Hardware;

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
			ARDUINO_PROMICRO = 0x04
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
			// 1. board type
			if (Type != version.Type)
				return ((int)Type).CompareTo((int)version.Type);

			// 2. major version
			if (Major != version.Major)
				return Major.CompareTo(version.Major);

			// 3. minor version
			return Minor.CompareTo(version.Minor);
		}

		public string VerStr
		{
			get
			{
				return $"{Major}.{Minor}";
			}
		}

		public string DisplayName
		{
			get
			{
				return ArduinoTypeToDisplayName(Type) + " - V" + VerStr;
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
				case ArduinoType.ARDUINO_PROMICRO:
					return "PROMICRO";
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
				case "PROMICRO":
					return ArduinoType.ARDUINO_PROMICRO;
			}
			return ArduinoType.None;
		}

		public static string ArduinoTypeToDisplayName(ArduinoType arduinoType)
		{
			switch (arduinoType)
			{
				case ArduinoType.ARDUINO_UNO:
					return "UNO R3";
				case ArduinoType.ARDUINO_NANO:
					return "NANO R3";
				case ArduinoType.ARDUINO_MEGA2560:
					return "Mega2560";
				case ArduinoType.ARDUINO_PROMICRO:
					return "ProMicro";
			}
			return "unknown";
		}


		public static ArduinoModel? ArduinoTypeToModel(SketchVersion.ArduinoType arduinoType)
		{
			switch (arduinoType)
			{
				case SketchVersion.ArduinoType.ARDUINO_UNO:
					return ArduinoModel.UnoR3;
				case SketchVersion.ArduinoType.ARDUINO_NANO:
					return ArduinoModel.NanoR3;
				case SketchVersion.ArduinoType.ARDUINO_MEGA2560:
					return ArduinoModel.Mega2560;
				case SketchVersion.ArduinoType.ARDUINO_PROMICRO:
					return ArduinoModel.Micro;
			}
			return null;
		}

	}
}
