using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TapecartFlasher
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (!DotnetOk())
			{
				MessageBox.Show(
					$"This application requires Dotnet Version 4.5 or greater.",
					Helper.GetVersion());
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainView());
		}

		static bool DotnetOk()
		{
			string keyPath = @"Software\Microsoft\NET Framework Setup\NDP\v4\Full";
			string key = "Release";

			try
			{
				RegistryKey rk = Registry.LocalMachine.OpenSubKey(keyPath, false);
				int value = (int)rk.GetValue(key);

				return value >= 378389; // at least Dotnet 4.5
			}
			catch(Exception)
			{
				return false;
			}
		}
	}
}
