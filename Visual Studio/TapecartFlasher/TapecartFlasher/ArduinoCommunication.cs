using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using DgCommon;

namespace TapecartFlasher
{
	class ArduinoCommunication
	{
		private const string MODUL_NAME = "ArduinoCommunication";

		private Logging _logging = Logging.Instance;

		//public const int DEFAULT_BAUDRATE = 230000;
		public const int DEFAULT_BAUDRATE = 115000;
		//public const int BAUDRATE = 57600;

		public const int DEFAULT_TIMEOUT = 10000;

		public int CurrentBaudrate { get; set; }

		public int CurrentTimeout { get; set; }

		public string CurrentComPortName { get; set; }

		public string CurrentParameter
		{
			get { return $"Port={CurrentComPortName} CurrentBaudrate={CurrentBaudrate} Timeout={CurrentTimeout}"; }
		}

		private const byte CMD_SOH = 0x01; // SOH
		private const byte CMD_ENQ = 0x05; // ENQ

		private enum CmdGroups
		{
			CMDGROUP_ARDUINO = 0x01,
			CMDGROUP_TAPECART = 0x02
		};

		private enum ArduinoCmds
		{
			ARDUINOCMD_VERSION = 0x01,
			ARDUINOCMD_START_CMDMODE = 0x02
		};

		private enum TapecartCmds
		{
			CMD_EXIT = 0,
			CMD_READ_DEVICEINFO,
			CMD_READ_DEVICESIZES,
			CMD_READ_CAPABILITIES,

			CMD_READ_FLASH = 0x10,
			CMD_READ_FLASH_FAST,
			CMD_WRITE_FLASH,
			CMD_WRITE_FLASH_FAST, // FIXME: Not Yet Implemented
			CMD_ERASE_FLASH_64K,
			CMD_ERASE_FLASH_BLOCK,
			CMD_CRC32_FLASH,

			CMD_READ_LOADER = 0x20,
			CMD_READ_LOADINFO,
			CMD_WRITE_LOADER,
			CMD_WRITE_LOADINFO,

			CMD_LED_OFF = 0x30,
			CMD_LED_ON,
			CMD_READ_DEBUGFLAGS,
			CMD_WRITE_DEBUGFLAGS,

			CMD_DIR_SETPARAMS = 0x40,
			CMD_DIR_LOOKUP
		};

		private SerialPort _serialPort;

		public bool Connected
		{
			get { return _serialPort != null; }
		}

		public ArduinoCommunication()
		{
		}

		public bool Init(string comPortName, int baudrate, int timeout, bool reset = false)
		{
			if (_serialPort != null)
				_serialPort.Close();

			CurrentComPortName = comPortName;
			CurrentBaudrate = baudrate;
			CurrentTimeout = timeout;

			try
			{
				_serialPort = new SerialPort(comPortName, baudrate);
				//_serialPort.Handshake = Handshake.RequestToSend;
				//_serialPort.DtrEnable = true;   // reset arduino
				_serialPort.ReadTimeout = CurrentTimeout;
				_serialPort.Open();
				if (reset)
				{
					_serialPort.DtrEnable = true;   // reset arduino
					Thread.Sleep(2000);
				}
				return true;
			}
			catch (Exception ex)
			{
				_logging.Error(MODUL_NAME, "Init", $"error opening comport {comPortName}, ex={ex}");
				return false;
			}
		}

		public void DeInit()
		{
			_serialPort?.Close();
			_serialPort = null;
		}

		public bool GetArduinoSketchVersion(out int majorVersion, out int minorVersion, out int apiVersion, out SketchVersion.ArduinoType arduinoType)
		{
			majorVersion = 0;
			minorVersion = 0;
			apiVersion = 0;
			arduinoType = SketchVersion.ArduinoType.None;

			if (_serialPort == null)
				return false;

			byte[] recvData;
			_serialPort.ReadTimeout = 15000; // 15 seconds
			int result = SendCmd(CmdGroups.CMDGROUP_ARDUINO, (byte)ArduinoCmds.ARDUINOCMD_VERSION, null, 4, out recvData);
			_serialPort.ReadTimeout = CurrentTimeout;

			if (result != 0)
				return false;

			minorVersion = recvData[0];
			majorVersion = recvData[1];
			apiVersion = recvData[2];
			arduinoType = (SketchVersion.ArduinoType)recvData[3];

			return true;
		}

		//public bool SetCmdMode()
		//{
		//	byte[] cmd = new byte[] { CMD_PREFIX, (byte)CmdGroups.CMDGROUP_ARDUINO, (byte)ArduinoCmds.ARDUINOCMD_START_CMDMODE };
		//	return true;
		//}

		public TapecartInfo GetTapecartInfo()
		{
			if (_serialPort == null)
				return null;

			int result = SendCmd(CmdGroups.CMDGROUP_ARDUINO, (int)ArduinoCmds.ARDUINOCMD_START_CMDMODE);
			if (result != 0)
				return null;

			TapecartInfo info = new TapecartInfo();

			byte[] recvData;
			result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_READ_DEVICEINFO, null, null, out recvData);
			if (result != 0 || recvData?.Length == 0)
				return null;
			info.TCrtName = "";
			for (int i = 0; i < recvData.Length; i++)
				info.TCrtName += (char)recvData[i];
			info.TCrtName = info.TCrtName.Trim();

			result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_READ_DEVICESIZES, null, 7, out recvData);
			if (result != 0)
				return null;
			info.TotalSize = (int)FromBuffer_u24(recvData, 0);
			info.PageSize = FromBuffer_u16(recvData, 3);
			info.ErasePages = FromBuffer_u16(recvData, 2);

			return info;
		}

		/// <summary>
		/// Read a flash page
		/// Because of the RAM limitation in the arduino only a page site of 256 byte is allowed
		/// </summary>
		/// <returns></returns>
		public byte[] ReadFlashPage(int flashOffset)
		{
			if (_serialPort == null)
				return null;

			byte[] sendData = new byte[5];
			ToBuffer_u24(sendData, 0, flashOffset);
			ToBuffer_u16(sendData, 3, Tapecart.BLOCK_SIZE);

			byte[] recvData;
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_READ_FLASH, sendData, Tapecart.BLOCK_SIZE, out recvData);
			if (result != 0)
				return null;
			return recvData;
		}

		public bool WriteFlashPage(byte[] data, int offset, int flashOffset)
		{
			if (_serialPort == null)
				return false;

			byte[] sendData = new byte[Tapecart.BLOCK_SIZE + 5];
			ToBuffer_u24(sendData, 0, flashOffset);
			ToBuffer_u16(sendData, 3, Tapecart.BLOCK_SIZE);
			Buffer.BlockCopy(data, offset, sendData, 5, Tapecart.BLOCK_SIZE);

			byte[] recvData;
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_WRITE_FLASH, sendData, 0, out recvData);
			return result == 0;
		}

		public bool EraseFlash64K(int flashOffset)
		{
			if (_serialPort == null)
				return false;

			byte[] sendData = new byte[3];
			ToBuffer_u24(sendData, 0, flashOffset);
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_ERASE_FLASH_64K, sendData);
			return result == 0;
		}

		public bool EraseFlashBlock(int flashOffset)
		{
			if (_serialPort == null)
				return false;

			byte[] sendData = new byte[3];
			ToBuffer_u24(sendData, 0, flashOffset);
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_ERASE_FLASH_BLOCK, sendData);
			return result == 0;
		}

		/// <summary>
		/// Performs an internal crc32 check of the flash rom in the Tapecart module.
		/// Runs 80 seconds on the AVR version and 7 seconds on the ARM version.
		/// Therefore the timeout is temporary extendet.
		/// </summary>
		/// <param name="flashOffset">The flash offset.</param>
		/// <param name="length">The length.</param>
		/// <param name="crc">The CRC.</param>
		/// <returns></returns>
		public bool Crc32Flash(int flashOffset, int length, out UInt32 crc) {
			crc = 0;
			if (_serialPort == null)
				return false;

			// extend timeout
			int tempTimeout = CurrentTimeout;
			CurrentTimeout = 120 * 1000; // 120 seconds
			_serialPort.ReadTimeout = CurrentTimeout;

			byte[] sendData = new byte[6];
			ToBuffer_u24(sendData, 0, flashOffset);
			ToBuffer_u24(sendData, 3, length);
			byte[] recvData;
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_CRC32_FLASH, sendData, 4, out recvData);
			crc = BitConverter.ToUInt32(recvData, 0);

			// restore timeout
			CurrentTimeout = tempTimeout;
			_serialPort.ReadTimeout = CurrentTimeout;

			return result == 0;
		}

		public byte[] ReadLoader()
		{
			if (_serialPort == null)
				return null;

			byte[] recvData;
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_READ_LOADER, null, 171, out recvData);
			return recvData;
		}

		public bool ReadLoadInfo(out int offset, out int length, out int callAddr, out string filename)
		{
			offset = 0;
			length = 0;
			callAddr = 0;
			filename = "";

			if (_serialPort == null)
				return false;

			byte[] recvData;
			int result = SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_READ_LOADINFO, null, 6 + 16, out recvData);
			if (result != 0)
				return false;

			offset = BitConverter.ToUInt16(recvData, 0);
			length = BitConverter.ToUInt16(recvData, 2);
			callAddr = BitConverter.ToUInt16(recvData, 4);
			filename = ByteArray.ReadAsciiString(recvData, 6, 16);
			return true;
		}

		public bool WriteLoader(byte[] loader)
		{
			if (_serialPort == null)
				return false;

			return SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_WRITE_LOADER, loader) == 0;
		}

		public bool WriteLoadInfo(int offset, int length, int callAddr, string filename)
		{
			if (_serialPort == null)
				return false;

			byte[] sendData = new byte[6 + 16];
			ToBuffer_u16(sendData, 0, offset);
			ToBuffer_u16(sendData, 2, length);
			ToBuffer_u16(sendData, 4, callAddr);
			ByteArray.WriteASCIIString(sendData, 6, filename, 16, 0x20);
			return SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_WRITE_LOADINFO, sendData) == 0;
		}

		public bool LedOnOff(bool ledOn)
		{
			if (_serialPort == null)
				return false;

			if (ledOn)
				return SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_LED_ON) == 0;
			else
				return SendCmd(CmdGroups.CMDGROUP_TAPECART, (int)TapecartCmds.CMD_LED_OFF) == 0;
		}

		/// <summary>
		/// Send cmd without send and receive data
		/// </summary>
		/// <param name="sendGroup"></param>
		/// <param name="sendCmd"></param>
		/// <returns></returns>
		private int SendCmd(CmdGroups sendGroup, int sendCmd)
		{
			byte[] dummy;
			return SendCmd(sendGroup, sendCmd, null, 0, out dummy);
		}

		/// <summary>
		/// Send cmd with send data, not receive data
		/// </summary>
		/// <param name="sendGroup"></param>
		/// <param name="sendCmd"></param>
		/// <param name="sendData"></param>
		/// <param name="recvLen"></param>
		/// <param name="recvData"></param>
		/// <returns></returns>
		private int SendCmd(CmdGroups sendGroup, int sendCmd, byte[] sendData)
		{
			byte[] dummy;
			return SendCmd(sendGroup, sendCmd, sendData, 0, out dummy);
		}

		/// <summary>
		/// Send cmd with send and receive data
		/// </summary>
		/// <param name="sendGroup"></param>
		/// <param name="sendCmd"></param>
		/// <param name="sendData"></param>
		/// <param name="recvLen"></param>
		/// <param name="recvData"></param>
		/// <returns></returns>
		private int SendCmd(CmdGroups sendGroup, int sendCmd, byte[] sendData, int? recvLen, out byte[] recvData)
		{
			recvData = null;
			try
			{
				_serialPort.DiscardInBuffer();
				byte[] data = CreateCmd(sendGroup, sendCmd, sendData);

				if (data.Length <= 32)
				{
					data = CreateCmd(sendGroup, sendCmd, sendData);
					_serialPort.Write(data, 0, data.Length);
				}
				else
				{
					// if data>32 byte we use a special SendBlock routine to send the data part
					data = CreateCmd(sendGroup, sendCmd, sendData);
					_serialPort.Write(data, 0, 5); // header
					SendBlock(data, 5, data.Length - 6); // data
					_serialPort.Write(data, data.Length - 1, 1); // checksum
				}

				int status = RecvResult(sendGroup, sendCmd, recvLen, out recvData);
				string str = _serialPort.ReadExisting();
				return status;
			}
			catch(Exception ex)
			{
				_logging.Error(MODUL_NAME, "SendCmd", $"Error sending command, sendCmd={sendCmd}");
				return 0;
			}
		}

		private byte[] CreateCmd(CmdGroups sendGroup, int sendCmd, byte[] data = null)
		{
			int dataLen = (data == null) ? 0 : data.Length;

			// data + 5 byte header + 1 byte chksum
			int length = dataLen + 5;
			byte[] cmd = new byte[length + 1]; // + 1 byte for checksum
			cmd[0] = CMD_SOH;
			cmd[1] = (byte)sendGroup;
			cmd[2] = (byte)sendCmd;
			cmd[3] = (byte)(dataLen & 0xff);
			cmd[4] = (byte)(dataLen >> 8);
			if (dataLen > 0)
				Buffer.BlockCopy(data, 0, cmd, 5, dataLen);
			int chksum = 0;
			for (int i = 1; i < cmd.Length - 1; i++)
				chksum ^= cmd[i];
			cmd[cmd.Length - 1] = (byte)(chksum);
			return cmd;
		}

		/// <summary>
		/// Result with out data
		/// </summary>
		/// <param name="sendGroup"></param>
		/// <param name="sendCmd"></param>
		/// <returns></returns>
		private int RecvResult(CmdGroups sendGroup, int sendCmd)
		{
			byte[] dummy;
			return RecvResult(sendGroup, sendCmd, 0, out dummy);
		}

		/// <summary>
		/// Receive a result message from the Arduino, with timeout
		/// Debugging messegaes from the arduino a filtered an written to the log
		/// </summary>
		/// <param name="sendGroup"></param>
		/// <param name="sendCmd"></param>
		/// <param name="length"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private int RecvResult(CmdGroups sendGroup, int sendCmd, int? length, out byte[] data)
		{
			int cmdPrefix;
			data = null;

			try
			{
				// wait for CMD_PREFIX
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (true)
				{
					if (sw.ElapsedMilliseconds > CurrentTimeout)
					{
						_logging.Warn(MODUL_NAME, "RecvResult", $"timout waiting for {CMD_SOH:X2}");
						Debug.WriteLine($"timeout {(byte)sendGroup:X2} {sendCmd:X2}");
						return 99;
					}
					if (_serialPort.BytesToRead == 0)
						continue;
					cmdPrefix = _serialPort.ReadByte();
					if (cmdPrefix == CMD_SOH)
						break;
					if (cmdPrefix == (int)'*')
					{
						// debug output from arduino
						string debug = _serialPort.ReadLine().Trim(new char[] { '\r', '\n' });
						Debug.WriteLine("*" + debug);
						_logging.Debug("Arduino", "", "*" + debug);
						sw.Restart();
					}
				}

				int calcChksum = 0;

				int recvGroup = _serialPort.ReadByte();
				if (recvGroup != (int)sendGroup)
				{
					_logging.Error(MODUL_NAME, "RecvResult", $"invalid cmd group {recvGroup:X2} received, expected {(int)sendGroup:X2}");
					return 99; // invalid cmd group received
				}
				calcChksum ^= recvGroup;

				int recvCmd = _serialPort.ReadByte();
				if (recvCmd != sendCmd)
				{
					_logging.Error(MODUL_NAME, "RecvResult", $"invalid cmd {recvCmd:X2} received, expected {sendCmd:X2}");
					return 99; // invalid cmd group received
				}
				calcChksum ^= recvCmd;

				int recvResult = _serialPort.ReadByte();
				if (recvResult != 0)
				{
					_logging.Error(MODUL_NAME, "RecvResult", $"error result {recvResult:X2} received");
					return recvResult;
				}
				calcChksum ^= recvResult;

				// 16 bit length
				int recvLength = _serialPort.ReadByte();
				recvLength += (_serialPort.ReadByte() << 8);
				if (length != null && recvLength != length)
				{
					_logging.Error(MODUL_NAME, "RecvResult", $"invalid length {recvLength}, excpected {length}");
					return 99; // wrong length
				}
				calcChksum ^= recvLength & 0xFF;
				calcChksum ^= (recvLength >> 8) & 0xFF;

				if (recvLength != 0)
				{
					// receive data

					data = new byte[recvLength];
					int count = RecvBlock(data, recvLength);
					if (count != recvLength)
					{
						_logging.Error(MODUL_NAME, "RecvResult", $"received {count} bytes, excpected {recvLength} bytes");
						return 99; // wrong length
					}
					for (int i = 0; i < recvLength; i++)
						calcChksum ^= data[i];
				}

				int chkSum = _serialPort.ReadByte();
				if (chkSum != calcChksum)
				{
					_logging.Error(MODUL_NAME, "RecvResult", $"checksum error, receiced {chkSum:X2}, excpected {calcChksum:X2}");
					return 99; // wrong checksum
				}

				return 0; // ok
			}
			catch (Exception ex)
			{
				_logging.Error(MODUL_NAME, "RecvResult", $"ex={ex}");
				return 99;
			}
		}

		UInt16 FromBuffer_u16(byte[] buffer, int offset)
		{
			return (UInt16)(buffer[offset] + ((int)buffer[offset + 1] << 8));
		}

		UInt32 FromBuffer_u24(byte[] buffer, int offset)
		{
			return (UInt32)((int)buffer[offset] + ((int)buffer[offset + 1] << 8) + ((int)buffer[offset + 2] << 16));
		}

		/*
		UInt32 FromBuffer_u32(byte[] buffer, int offset)
		{
			return (UInt32)((int)buffer[offset] + ((int)buffer[offset + 1] << 8) + 
				((int)buffer[offset + 2] << 16) + ((int)buffer[offset + 3] << 24));
		}
		*/

		void ToBuffer_u16(byte[] buffer, int offset, int value)
		{
			buffer[offset] = (byte)(value & 0xFF);
			buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
		}

		public static void ToBuffer_u24(byte[] buffer, int offset, int value)
		{
			buffer[offset] = (byte)(value & 0xFF);
			buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
			buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
		}

		public static void ToBuffer_u32(byte[] buffer, int offset, int value)
		{
			buffer[offset] = (byte)(value & 0xFF);
			buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
			buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
			buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
		}

		public List<ComPortSelectionItem> GetPorts()
		{
			string[] ports = SerialPort.GetPortNames();
			List<ComPortSelectionItem> list = new List<ComPortSelectionItem>();
			for (int i = 0; i < ports.Length; i++)
			{
				int nr;
				if (int.TryParse(ports[i].Substring(3), out nr))
				{
					list.Add(new ComPortSelectionItem(nr, ports[i]));
				}
			}

			return list.OrderBy(o => o.ComNumber).ToList();

			// this more sophisticated COM port detection needs a higher DOTNET version
#if false
			List<ComPortSelectionItem> list = new List<ComPortSelectionItem>();
			//ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SerialPort");
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid = \"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
			foreach (ManagementObject queryObj in searcher.Get())
			{
				/*
				foreach (var property in queryObj.Properties)
				{
					foreach (QualifierData q in property.Qualifiers)
					{
							Debug.WriteLine(
								queryObj.GetPropertyQualifierValue(
								property.Name, q.Name));
							Debug.WriteLine(property.Value);
						}

					}
					Debug.WriteLine("");
				}
				*/

				string devId = (string)queryObj["DeviceID"];
				string name = (string)queryObj["Name"];
				int p1 = name.LastIndexOf("(");
				if (p1 == -1)
					continue;
				int p2 = name.LastIndexOf(")");
				if (p2 == -1 || p1 >= p2)
					continue;

				string comPort = name.Substring(p1 + 1, p2 - p1 - 1);
				if (comPort.Length <= 3 || comPort.Substring(0, 3).ToLower() != "com")
					continue;

				int comNumber = Convert.ToInt32(comPort.Substring(3));

				string desc = (string)queryObj["Description"];
				if (desc.IndexOf("Kommunikationsanschluss") != -1 || desc.IndexOf("Serial Port") != -1)
					desc = "";

				list.Add(new ComPortSelectionItem()
				{
					ComNumber = comNumber,
					ComPort = comPort,
					Description = $"{comPort}: {desc}"
				});
			}

			list = list.OrderBy(o => o.ComNumber).ToList();
			return list;

			/*
			Debug.WriteLine(queryObj["Caption"]);
			Debug.WriteLine(queryObj["Description"]);
			Debug.WriteLine(queryObj["DeviceID"]);
			Debug.WriteLine(queryObj["Name"]);
			Debug.WriteLine(queryObj["PNPDeviceID"]);
			*/


			// geht nicht: access denied
			/*
			ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSSerial_PortName");
			foreach (ManagementObject queryObj2 in searcher2.Get())
			{
				Debug.WriteLine(queryObj2["PortName"]);
				Debug.WriteLine(queryObj2["InstanceName"]);
			}
			*/
#endif
		}

		public bool CheckPort(ComPortSelectionItem portItem)
		{
			bool success = Init(portItem.ComPort, DEFAULT_BAUDRATE, 500, false);
			if (!success)
				return false;
			int majorVersion;
			int minorVersion;
			int apiVersion;
			SketchVersion.ArduinoType arduinoType;
			success = GetArduinoSketchVersion(out majorVersion, out minorVersion, out apiVersion, out arduinoType);
			DeInit();
			return success;
		}

		/// <summary>
		/// Because the Arduino input buffer is only 64 byte, we have to sends data part of a command in 32 byte blocks with handshake
		/// This is only used if the data part is >= 32 bytes
		/// Handshake character is CMD_ENQ
		/// Debugging messegaes from the arduino a filtered an written to the log
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length.</param>
		/// <returns></returns>
		private bool SendBlock(byte[] buffer, int offset, int length)
		{

			int len;
			int pos = 0;
			Stopwatch sw = new Stopwatch();
			while (pos < length)
			{
				len = length - pos;
				if (len > 32)
					len = 32;
				sw.Restart();
				while (true)
				{
					if (sw.ElapsedMilliseconds > 1000)
						return false;
					int chr = _serialPort.ReadByte();
					if (chr == CMD_ENQ)
						break;
					else if (chr == '*')
					{
						// debug output from arduino
						string debug = _serialPort.ReadLine().Trim(new char[] { '\r', '\n' });
						Debug.WriteLine("*" + debug);
						_logging.Debug("Arduino", "", "*" + debug);
					}
					else
						return false;
				}

				_serialPort.Write(buffer, offset + pos, len);
				pos += len;
			}
			return true;
		}

		private int RecvBlock(byte[] buffer, int len)
		{
			for (int i = 0; i < len; i++)
				buffer[i] = (byte)_serialPort.ReadByte();
			return len;
		}

	}

	class ComPortSelectionItem
	{
		public int ComNumber { get; set; }

		public string ComPort { get; set; }

		public string Description { get; set; }

		public ComPortSelectionItem(int comNumber, string comPort)
		{
			ComNumber = comNumber;
			ComPort = comPort;
			Description = comPort;
		}

		public override string ToString()
		{
			return $"{ComNumber} {ComPort}";
		}
	}
}
