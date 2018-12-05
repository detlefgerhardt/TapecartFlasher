using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TapecartFlasher
{

	class SketchList
	{

		private static SketchList instance;

		public static SketchList Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SketchList();
				}
				return instance;
			}
		}

		public List<SketchVersion> SketchVersions;

		private SketchList()
		{
			ReadSketchList();
		}

		public void ReadSketchList()
		{
			string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			//string exePath = Application.StartupPath;

			SketchVersions = new List<SketchVersion>();

			DirectoryInfo dirInfo = new DirectoryInfo(exePath);
			FileInfo[] files = dirInfo.GetFiles("TapecartFlasher*.hex");

			foreach (FileInfo file in files) {
				string[] list = Path.GetFileNameWithoutExtension(file.Name).Split('_');
				if ( list.Length != 5 )
					continue;

				SketchVersion.ArduinoType arduinoType = SketchVersion.ArduinoNameToType(list[1]);
				if ( arduinoType == SketchVersion.ArduinoType.None )
					continue; // invalid arduino type

				int majorVersion;
				if ( !int.TryParse(list[2], out majorVersion) )
					continue; // invalid major version

				int minorVersion;
				if ( !int.TryParse(list[3], out minorVersion) )
					continue; // invalid major version

				int apiVersion;
				if ( !int.TryParse(list[4], out apiVersion) )
					continue; // invalid major version

				SketchVersion version = new SketchVersion(majorVersion, minorVersion, apiVersion, arduinoType);
				version.Filename = file.Name;

				SketchVersions.Add(version);

				SketchVersions.Sort(delegate(SketchVersion x, SketchVersion y)
				{
					return x.SortCompareTo(y);
				});
			}
		}

		public SketchVersion GetLatestVersion(SketchVersion version, bool noVersionCheck=false)
		{
			if ( version == null )
				return null;

			// get the version for the same arduino type
			SketchVersion latest = ( from f in SketchVersions where f.Type == version.Type select f ).FirstOrDefault();
			if ( latest == null )
				return null;
			if ( noVersionCheck || latest.IsHigherAs(version) )
				return latest;
			return null;
		}
	}
}
