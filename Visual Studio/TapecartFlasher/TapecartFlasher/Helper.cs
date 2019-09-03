using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace TapecartFlasher
{
	class Helper
	{
		public static byte[] GetEmbeddedRessource(string name)
		{
			Assembly assembly = assembly = Assembly.GetExecutingAssembly();

			try
			{
				assembly = Assembly.GetExecutingAssembly();
				Stream binStream = assembly.GetManifestResourceStream(name);
				byte[] data;
				using (BinaryReader reader = new BinaryReader(binStream))
				{
					data = reader.ReadBytes((int)binStream.Length);
				}
				return data;
			}
			catch
			{
				return null;
			}
		}

		public static string GetEmbeddedTextRessource(string name)
		{
			Assembly assembly = assembly = Assembly.GetExecutingAssembly();

			try
			{
				string result;
				assembly = Assembly.GetExecutingAssembly();
				using (Stream stream = assembly.GetManifestResourceStream(name))
					using (StreamReader reader = new StreamReader(stream))
					{
						result = reader.ReadToEnd();
					}
				return result;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static string CombineWithNull(string part1, string part2)
		{
			if (part1 == null)
				part1 = "";
			if (part2 == null)
				part2 = "";
			return Path.Combine(part1, part2);
		}

		public static string GetVersion()
		{
#if (DEBUG)
			// show date and time in debug version
			string buildTime = Properties.Resources.BuildDate.Trim(new char[] { '\n', '\r' }) + " Debug";
#else
			// show only date in release version
			string buildTime = Properties.Resources.BuildDate.Trim(new char[] { '\n', '\r' });
			buildTime = buildTime.Substring(0, 10);
#endif
			return $"Tapecart Flasher  V{Application.ProductVersion}  (Build={buildTime})";
		}

		public static Point CenterForm(Form form, Rectangle parentPos)
		{
			int screenNr = GetScreenNr(parentPos);
			Rectangle sc = Screen.AllScreens[screenNr].WorkingArea;

			int x = sc.Left + (sc.Width - form.Width) / 2;
			int y = sc.Top + (sc.Height - form.Height) / 2;

			return new Point(x, y);
		}

#warning TODO Fehler, wenn parentPos = Fullscreen
		public static int GetScreenNr(Rectangle parentPos)
		{
			Screen[] screens = Screen.AllScreens;
			int screenNr = -1;
			int lenMax = 0;
			for (int i = 0; i < screens.Length; i++)
			{
				//Rectangle scrnBounds = screens[i].Bounds;
				Rectangle scrnBounds = screens[i].WorkingArea;
				//Debug.WriteLine("Screen {0}, width={1}", i, scrnBounds.Right - parentPos.Left);
				int len = 0;
				if (parentPos.Left >= scrnBounds.Left && parentPos.Left <= scrnBounds.Right ||
					parentPos.Right >= scrnBounds.Left && parentPos.Right <= scrnBounds.Right)
				{
					len = scrnBounds.Right - parentPos.Left;
					//Debug.WriteLine("Screen {0}, len={1}", i, len);
				}
				if (screenNr == -1 || len > lenMax)
				{
					screenNr = i;
					lenMax = len;
				}
			}
			//Debug.WriteLine("Screen {0}, width={1}", screenNr, lenMax);
			return screenNr;
		}

		/// <summary>
		/// Helper method to determin if invoke required, if so will rerun method on correct thread.
		/// if not do nothing.
		/// </summary>
		/// <param name="c">Control that might require invoking</param>
		/// <param name="a">action to preform on control thread if so.</param>
		/// <returns>true if invoke required</returns>
		public static void ControlInvokeRequired(Control c, Action a)
		{
			if (c.InvokeRequired)
				c.Invoke(new MethodInvoker(delegate { a(); }));
			else
				a();
		}
	}
}
