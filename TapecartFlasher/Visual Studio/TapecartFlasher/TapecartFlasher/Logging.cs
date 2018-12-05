using System;
using System.IO;

namespace TapecartFlasher
{
	public delegate void RecvLogEvent(object sender, LogArgs e);

	public enum LogTypes { None = 0, Fatal = 1, Error = 2, Warn = 3, Info = 4, Debug = 5 };

	[Serializable]
	class Logging
	{
		public event RecvLogEvent RecvLog;

		public LogTypes LogLevel { get; set; }

		private static Logging instance;

		public static Logging Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Logging();
				}
				return instance;
			}
		}

		private Logging()
		{
			LogLevel = LogTypes.Debug;
		}

		public void Debug(string section, string method, string text, bool show = true)
		{
			Log(LogTypes.Debug, section, method, text, show);
		}

		public void Info(string section, string method, string text, bool show = true)
		{
			Log(LogTypes.Info, section, method, text, show);
		}

		public void Warn(string section, string method, string text, bool show = true)
		{
			Log(LogTypes.Warn, section, method, text, show);
		}

		public void Error(string section, string method, string text, bool show = true)
		{
			Log(LogTypes.Error, section, method, text, show);
		}

		public void Fatal(string section, string method, string text, bool show = true)
		{
			Log(LogTypes.Fatal, section, method, text, show);
		}

		public void Log(LogTypes logType, string section, string method, string text, bool show = true)
		{
			if (IsActiveLevel(logType))
			{
				AppendLog(logType, section, method, text);
				OnLog(new LogArgs(logType, section, method, text, show));
			}
		}

		public void OnLog(LogArgs e)
		{
			//Debug.WriteLine("OnLog");
			RecvLog?.Invoke(this, e);
		}

		private void AppendLog(LogTypes logType, string section, string method, string text)
		{
			try
			{
				//string fullName = Path.Combine(Global.DATA_PATH, Global.LOG_NAME);
				string fullName = "TapecartFlasher.log";
				File.AppendAllText(fullName, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} {logType.ToString().PadRight(5)} [{section}] [{method}] {text}\r\n");
			}
			catch(Exception)
			{
			}
		}

		private bool IsActiveLevel(LogTypes current)
		{
			return (int)current <= (int)LogLevel;
		}
	}

	public class LogArgs : EventArgs
	{
		public LogTypes LogType { get; set; }

		public string Section { get; set; }

		public string Method { get; set; }

		public string Message { get; set; }

		public bool Show { get; set; }

		public LogArgs(LogTypes logType, string section, string method, string msg, bool show)
		{
			LogType = logType;
			Section = section;
			Method = method;
			Message = msg;
			Show = show;
		}
	}

}
