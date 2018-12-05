using ArduinoUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapecartFlasher
{
	class ArduinoLogger: IArduinoUploaderLogger
	{
		private const string MODULE_NAME = "ArduinoUploader";

		Logging _logging = Logging.Instance;

		void IArduinoUploaderLogger.Error(string message, Exception exception)
		{
			_logging.Error(MODULE_NAME, "Upload", message, false);
		}

		void IArduinoUploaderLogger.Warn(string message)
		{
			_logging.Warn(MODULE_NAME, "Upload", message, false);
		}

		void IArduinoUploaderLogger.Info(string message)
		{
			_logging.Info(MODULE_NAME, "Upload", message, false);
		}

		void IArduinoUploaderLogger.Debug(string message)
		{
			//_logging.Debug(MODULE_NAME, "Upload", message, false);
		}

		void IArduinoUploaderLogger.Trace(string message)
		{
			//_logging.Debug(MODULE_NAME, "Upload", message, false);
		}
	}
}
