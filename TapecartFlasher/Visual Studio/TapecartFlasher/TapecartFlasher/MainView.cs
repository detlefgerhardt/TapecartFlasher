using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DgCommon;

namespace TapecartFlasher
{
	partial class MainView : Form
	{
		private const string MODUL_NAME = "TapecartFlasherView";

		private Logging _logging = Logging.Instance;

		/// <summary>
		/// Include bootloader into TCRT file, when reading it from the Tapecart modul
		/// </summary>
 #warning TODO implement checkbox for this option
		private const bool INCL_LOADER_IN_TCRT = false;

		private ArduinoCommunication _arduinoComm;

		private List<ComPortSelectionItem> _comPorts;

		private ComPortSelectionItem _selectedComPort;

		private TapecartInfo _tapecartInfo;

		private SketchVersion _currentSketchVersion;

		private SketchList _sketchList;

		private bool _writeActive = false;
		private bool _readActive = false;
		private bool _readWriteCanceled;

		public MainView()
		{
			InitializeComponent();

			this.Text = Helper.GetVersion();

			_logging.Info(MODUL_NAME, "MainView", $"Start {this.Text}");

			_arduinoComm = new ArduinoCommunication();
			InitComportSelection();
			SetButtonStatus();
			ReadWritePercentLbl.Text = "";
			ReadWriteStatusLbl.Text = "";

			this.MinimumSize = new Size(470, 320);

			TestBtn.Visible = false;

			// read the available ardunio sketches (hex files)
			_sketchList = SketchList.Instance;
		}

		private void MainView_Load(object sender, EventArgs e)
		{
			// limit form to current screen with
			int screenNr = CommonUtils.GetScreenNr(this.Bounds);
			Rectangle scrnBounds = Screen.AllScreens[screenNr].WorkingArea;
			if (this.Width > scrnBounds.Width - 20)
				this.Width = scrnBounds.Width - 20;

			// center form on screen
			Point pos = Helper.CenterForm(this, scrnBounds);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void MainView_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_writeActive || _readActive) {
				_writeActive = false;
				_readActive = false;
				// wait until read/write has stopped
				while (!_readWriteCanceled) {
					Thread.Sleep(100);
				};
			}

			_arduinoComm.DeInit();
		}

		private void InitComportSelection()
		{
			_comPorts = _arduinoComm.GetPorts();

			ComPortCb.Items.Clear();
			foreach (ComPortSelectionItem port in _comPorts)
				ComPortCb.Items.Add(port);

			ComPortCb.DisplayMember = "Description";
			ComPortCb.SelectedItem = null;
		}

		private void SetButtonStatus()
		{
			if (_arduinoComm.Connected)
			{
				ConnectStateLbl.Text = "Connected";
				ConnectStateLbl.ForeColor = Color.Green;
				ConnectBtn.Text = "Disconnect";
			}
			else
			{
				ConnectStateLbl.Text = "Disconnected";
				ConnectStateLbl.ForeColor = Color.Black;
				ConnectBtn.Text = "Connect";
			}

			DetectBtn.Enabled = !_arduinoComm.Connected;

			ConnectBtn.Enabled = ComPortCb.SelectedItem != null;
			ConnectBtn.Refresh();

			UpdateSketchBtn.Enabled = ComPortCb.SelectedItem != null /*&& _arduinoComm.Connected*/ && !_writeActive && !_readActive;
			UpdateSketchBtn.Refresh();

			WriteTcrtBtn.Enabled = _arduinoComm.Connected && !_readActive;
			WriteTcrtBtn.Refresh();

			ReadTcrtBtn.Enabled = _arduinoComm.Connected && !_writeActive;
			ReadTcrtBtn.Refresh();
		}

		private void DetectBtn_Click(object sender, EventArgs e)
		{
			DetectBtn.Enabled = false;
			DetectBtn.Text = "Detecting...";
			DetectBtn.Refresh();

			_logging.Info(MODUL_NAME, "DetectBtn_Click", "Detecting...");

			InitComportSelection();

			ComPortCb.SelectedItem = null;
#warning TODO check why detection always works only on the second pass
			// repeat 2 times
			for (int i = 0; i < 2; i++)
			{
				foreach (ComPortSelectionItem port in _comPorts)
				{
					Debug.WriteLine(port);
					if (_arduinoComm.CheckPort(port))
					{
						ComPortCb.SelectedItem = port;
						break;
					}
				}
				if (ComPortCb.SelectedItem != null)
					break;
			}

			if (ComPortCb.SelectedItem==null)
				_logging.Info(MODUL_NAME, "DetectBtn_Click", "No COM port detected");
			else
				_logging.Info(MODUL_NAME, "DetectBtn_Click", $"{((ComPortSelectionItem)ComPortCb.SelectedItem).ComPort} detected");

			DetectBtn.Enabled = true;
			DetectBtn.Text = "Detect";
		}

		private void ComPortCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			_selectedComPort = (ComPortSelectionItem)ComPortCb.SelectedItem;
			ConnectBtn.Enabled = _selectedComPort != null;
			_logging.Info(MODUL_NAME, "ComPortCb_SelectedIndexChanged", $"{_selectedComPort?.ComPort} selected");
			SetButtonStatus();
		}

		private async void ConnectBtn_Click(object sender, EventArgs e) {
			if (_selectedComPort == null)
				return;

			_logging.Info(MODUL_NAME, "ConnectBtn_Click", $"Connecting to {_arduinoComm.CurrentParameter}");

			FlasherVersionTb.Text = "";
			TapecartVersionTb.Text = "";

			if (_arduinoComm.Connected)
			{
				_arduinoComm.DeInit();
				ConnectBtn.Text = "Connect";
				ConnectBtn.ForeColor = Color.Black;
				SetButtonStatus();
				DetectBtn.Enabled = true;
				ConnectBtn.Enabled = true;
				return;
			}

			DetectBtn.Enabled = false;
			ConnectBtn.Enabled = false;

			string btnText = ConnectBtn.Text;
			ConnectBtn.Text = "Connecting...";
			ConnectStateLbl.Text = "";
			ConnectBtn.Refresh();

			bool success = false;
			await Task.Run(() => {
				success = Connect();
			});

			if (success)
				_logging.Info(MODUL_NAME, "ConnectBtn_Click", $"Connected");
			else
				_arduinoComm.DeInit();

			SetButtonStatus();

			SketchVersion latest = _sketchList.GetLatestVersion(_currentSketchVersion);
			if (latest != null)
				AskForSketchUpdate(latest);
		}

		private bool Connect()
		{
			if (!_arduinoComm.Init(_selectedComPort.ComPort, ArduinoCommunication.DEFAULT_BAUDRATE, ArduinoCommunication.DEFAULT_TIMEOUT, true))
			{
				_logging.Error(MODUL_NAME, "ConnectBtn_Click", $"error connecting to Arduino");
				Helper.ControlInvokeRequired(FlasherVersionTb, () =>
				{
					FlasherVersionTb.Text = "Error connecting to arduino";
				});
				return false;
			}

			int majorVersion = 0;
			int minorVersion = 0;
			int apiVersion = 0;
			SketchVersion.ArduinoType arduinoType;
			_currentSketchVersion = null;

			Debug.WriteLine("GetArduinoSketchVersion");
			if (!_arduinoComm.GetArduinoSketchVersion(out majorVersion, out minorVersion, out apiVersion, out arduinoType))
			{
				_logging.Error(MODUL_NAME, "ConnectBtn_Click", $"error reading Arduino sketch version");
				Helper.ControlInvokeRequired(FlasherVersionTb, () =>
				{
					FlasherVersionTb.Text = "Error reading Arduino sketch version";
				});
				return false;
			}

			_currentSketchVersion = new SketchVersion(majorVersion, minorVersion, apiVersion, arduinoType);

			Helper.ControlInvokeRequired(FlasherVersionTb, () =>
			{
				FlasherVersionTb.Text =
					$"Arduino: Version={majorVersion}.{minorVersion}, " +
					$"API-Version={apiVersion}, " +
					$"Type={SketchVersion.ArduinoTypeToName(arduinoType)}";
			});

			Debug.WriteLine("GetTapecartInfo");
			_tapecartInfo = _arduinoComm.GetTapecartInfo();
			if (_tapecartInfo == null)
			{
				_logging.Error(MODUL_NAME, "ConnectBtn_Click", $"error reading tapecart info");
				Helper.ControlInvokeRequired(TapecartVersionTb, () =>
				{
					TapecartVersionTb.Text = "Error reading Tapecart info";
				});
				return true;
			}

			Helper.ControlInvokeRequired(TapecartVersionTb, () =>
			{
				TapecartVersionTb.Text =
					$"Name={_tapecartInfo.TCrtName}, TotalSize={_tapecartInfo.TotalSize:X6}h, " +
					$"PageSize={_tapecartInfo.PageSize:X}h, ErasePages={_tapecartInfo.ErasePages:X}h";
			});
			return true;
		}

		private void Disconnect()
		{
			_logging.Info(MODUL_NAME, "Disconnect", $"Disconnect from {_arduinoComm.CurrentComPortName}");

			_arduinoComm.DeInit();
			ConnectBtn.Text = "Connect";
			ConnectBtn.ForeColor = Color.Black;
			ConnectBtn.Refresh();
			SetButtonStatus();
		}

		private void CloseBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void TestBtn_Click(object sender, EventArgs e)
		{
			//ReadWriteTest();
		}

#if false
		/// <summary>
		/// Debugging/Flash-Test
		/// </summary>
		private void ReadWriteTest()
		{
			byte[] buffer = new byte[256];

			int offset = 0x0000;
			int cnt = 1;
			while ( offset < 0x200000 )
			//while (offset < 0x1000)
			{
				Debug.WriteLine($"write {offset:X6}");
				for (int i = 0; i < 256; i++)
					buffer[i] = (byte)((cnt + i) % 256);
				buffer[0] = (byte)((offset >> 8) & 0xFF);
				buffer[1] = (byte)((offset >> 16) & 0xFF);
				if (offset % (Tapecart.ERASE_SIZE) == 0)
					_arduinoComm.EraseFlashBlock(offset);
				if (!_arduinoComm.WriteFlashPage(buffer, 0, offset))
				{
					Debug.WriteLine($"write error {offset:X6}");
					break;
				}
				offset += 256;
				cnt++;
			}

			offset = 0x0000;
			cnt = 0;
			while (offset < 0x200000)
			//while (offset < 0x1000)
			{
				Debug.WriteLine($"read {offset:X6}");
				buffer = _arduinoComm.ReadFlashPage(offset);
				int addr = ((int)buffer[0] << 8) + ((int)buffer[1] << 16);
				if (addr != offset)
					Debug.Write("");
				offset += 256;
				cnt++;
			}
		}
#endif

		private async void WriteTcrtBtn_Click(object sender, EventArgs e)
		{
			if (_tapecartInfo == null)
				return;

			if (_writeActive)
			{
				_writeActive = false; // stop
				return;
			}

			// load tcrt file

			OpenFileDialog openFileDialog = new OpenFileDialog();
			byte[] tcrtImage;
			try {
				openFileDialog.Filter = "TCRT files|*.tcrt";
				openFileDialog.Title = "Select a TCRT file";

				if (openFileDialog.ShowDialog() != DialogResult.OK)
					return;

				tcrtImage = File.ReadAllBytes(openFileDialog.FileName);
			}
			catch (Exception ex)
			{
				_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Error reading '{Path.GetFileName(openFileDialog.FileName)}'");
				return;
			}

			TcrtFilenameTb.Text = openFileDialog.FileName;
			string logFilename = Path.GetFileName(openFileDialog.FileName);

			_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"Write file: Filename='{logFilename}' {_arduinoComm.CurrentParameter}");

			// check tcrt header

			if (ByteArray.ReadAsciiString(tcrtImage, 0x00, 16) != Tapecart.TAPECART_SIGNATURE)
			{
				_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Invalid TCRT signature, tcrt file='{logFilename}'");
				TcrtInfoTb.Text = "Invalid TCRT header";
				return;
			}

			int tcrtVersion = BitConverter.ToUInt16(tcrtImage, Tapecart.OFFSET_VERSION);
			if (tcrtVersion != 1) {
				_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Invalid TCRT version, tcrt file='{logFilename}'");
				TcrtInfoTb.Text = $"Invalid TCRT version {tcrtVersion}";
				return;
			}

			int tcrtSize = (int)BitConverter.ToUInt32(tcrtImage, Tapecart.OFFSET_FLASHSIZE);
			if (tcrtSize > _tapecartInfo.TotalSize || tcrtSize < 0x1000) {
				_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"TCRT size exceeds cartridge size, tcrt file='{logFilename}'");
				TcrtInfoTb.Text = $"Invalid TCRT size {tcrtSize / 1024:X}h kB";
				return;
			}

			BrowserInfoTb.Text = TapecartBrowser.GetInfo(tcrtImage, Tapecart.HEADER_SIZE);

			// write loader

			byte[] loader;
			int flags = tcrtImage[Tapecart.OFFSET_FLAGS];
			if ((flags & 0x01) == 0) {
				// loader not present, use standard loader
				_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"TCRT file has no loader, using standard loader");
				loader = Helper.GetEmbeddedRessource("TapecartFlasher.Resources.tapecart_loader.bin");
			}
			else {
				// use loader from tcrt image
				_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"Using TCRT file loader");
				loader = new byte[Tapecart.LOADER_LENGTH];
				Buffer.BlockCopy(tcrtImage, Tapecart.OFFSET_LOADER, loader, 0, Tapecart.LOADER_LENGTH);
			}

			byte[] installedLoader = _arduinoComm.ReadLoader();
			if (!ByteArray.CompareBytes(loader, 0, installedLoader, 0, Tapecart.LOADER_LENGTH))
			{
				// installed loader is different, write loader
				if (!_arduinoComm.WriteLoader(loader))
				{
					_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Error writing loader, tcrt file='{logFilename}'");
					TcrtInfoTb.Text = $"Error writing loader";
					return;
				}
			}
			else
			{
				_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"Identical loader is already installed");
			}

			// write loadinfo

			int loaderOffset = BitConverter.ToUInt16(tcrtImage, Tapecart.OFFSET_LOADER_OFFSET);
			int loaderLength = BitConverter.ToUInt16(tcrtImage, Tapecart.OFFSET_LOADER_LENGTH);
			int loaderCalladdr = BitConverter.ToUInt16(tcrtImage, Tapecart.OFFSET_LOADER_CALLADDR);
			string filename = ByteArray.ReadAsciiString(tcrtImage, Tapecart.OFFSET_LOADER_NAME, Tapecart.LOADER_NAME_LENGTH);
			if (!_arduinoComm.WriteLoadInfo(loaderOffset, loaderLength, loaderCalladdr, filename)) {
				_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Error writing loader info, tcrt file='{logFilename}'");
				TcrtInfoTb.Text = $"Error writing loader info";
				return;
			}

			TcrtInfoTb.Text = $"Size={tcrtSize:X6}h Loader info={loaderOffset:X4}h,{loaderLength:X4}h,{loaderCalladdr:X4}h,{filename.Trim()}";

			// write flash data

			string btnText = WriteTcrtBtn.Text;
			WriteTcrtBtn.Text = "Cancel write";
			ReadTcrtBtn.Enabled = false;

			ReadWritePb.Minimum = 0;
			ReadWritePb.Maximum = 100;
			ReadWritePb.Step = 1;
			ReadWritePb.Value = 0;

			_writeActive = true;
			_readWriteCanceled = false;
			SetButtonStatus();

			await Task.Run(() =>
			{
				int flashOffset = 0;
				Crc32 calcCrc = new Crc32();
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (flashOffset < tcrtSize)
				{
					if (!_writeActive)
					{   // cancel
						_readWriteCanceled = true; // <- must be set before invoke !!!
						UpdateReadWriteStatus("Write canceled", Color.Red);
						_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Write canceled at {flashOffset:X6}");
						UpdateReadWriteBar(0);
						return;
					}

					if (flashOffset % Tapecart.ERASE_SIZE == 0)
						_arduinoComm.EraseFlashBlock(flashOffset);

					if (!_arduinoComm.WriteFlashPage(tcrtImage, flashOffset + Tapecart.HEADER_SIZE, flashOffset))
					{
						UpdateReadWriteStatus($"Error at offset={flashOffset:X6}", Color.Red);
						_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"Error at offset={flashOffset:X6}");
						_readWriteCanceled = true;
						return;
					}

					calcCrc.Update(tcrtImage, flashOffset + Tapecart.HEADER_SIZE, Tapecart.BLOCK_SIZE);

					flashOffset += Tapecart.BLOCK_SIZE;

					if (flashOffset % 0x1000 == 0)
					{
						UpdateReadWriteBar((int)(flashOffset * 100 / tcrtSize));
						UpdateReadWriteStatus($"Writing: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}");
					}
				}

				if (ChecksumCb.Checked)
				{
					_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", "Checksum test...");
					UpdateReadWriteStatus($"Checksum-Test...");
					UInt32 crc;
					Stopwatch sw2 = new Stopwatch();
					sw2.Start();
					_arduinoComm.Crc32Flash(0, (int)tcrtSize, out crc);
					Debug.WriteLine($"crc-time={Minutes(sw2.ElapsedMilliseconds)}");
					if (crc != calcCrc.Crc)
					{
						Debug.WriteLine($"checksum error calc={calcCrc.Crc:X8} flash={crc:X8}");
						_logging.Warn(MODUL_NAME, "WriteTcrtBtn_Click", $"checksum error calc={calcCrc.Crc:X8} flash={crc:X8}");
						UpdateReadWriteStatus($"Checksum error", Color.Red);
						return;
					}
					else
						_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"checksum ok {crc:X8}");

				}

				_logging.Info(MODUL_NAME, "WriteTcrtBtn_Click", $"Write finished: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}");
				UpdateReadWriteStatus($"Write finished: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}", Color.Green);
				UpdateReadWriteBar(100);
			});


			_readWriteCanceled = true;
			_writeActive = false;
			WriteTcrtBtn.Text = btnText;
			SetButtonStatus();
		}


		private async void ReadTcrtBtn_Click(object sender, EventArgs e)
		{
			if (_tapecartInfo == null)
				return;

			if (_readActive)
			{
				_readActive = false; // stop
				return;
			}

			// 2 mbyte tcrt file
			byte[] tcrtImage = new byte[0x200000 + 216];

			// select tcrt file to write

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			try {
				saveFileDialog.Filter = "TCRT files|*.tcrt";
				saveFileDialog.Title = "Select a TCRT file";

				if (saveFileDialog.ShowDialog() != DialogResult.OK)
					return;

				// test if we can write this file
				File.WriteAllBytes(saveFileDialog.FileName, tcrtImage);
				File.Delete(saveFileDialog.FileName);
			}
			catch (Exception)
			{
				_logging.Warn(MODUL_NAME, "ReadTcrtBtn_Click", $"Error opening tcrt file for write '{Path.GetFileName(saveFileDialog.FileName)}'");
				TcrtInfoTb.Text = $"Error opening TCRT file for write";
				return;
			}

			TcrtFilenameTb.Text = saveFileDialog.FileName;
			string logFilename = Path.GetFileName(saveFileDialog.FileName);

			_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", $"Read file, Filename='{logFilename}' {_arduinoComm.CurrentParameter}");

			// fill header with 0xFF
			ByteArray.Fill(tcrtImage, Tapecart.HEADER_SIZE, 0xFF, Tapecart.MAX_FLASH_SIZE);

			ByteArray.WriteASCIIString(tcrtImage, 0, Tapecart.TAPECART_SIGNATURE);
			ByteArray.WriteUInt16(tcrtImage, Tapecart.OFFSET_VERSION, Tapecart.VERSION);
			// we can not determine the real tcrt size, so we assume maximum flash size
			ByteArray.WriteInt32(tcrtImage, Tapecart.OFFSET_FLASHSIZE, Tapecart.MAX_FLASH_SIZE);

			// handle loader

			if ( INCL_LOADER_IN_TCRT ) {
				tcrtImage[Tapecart.OFFSET_FLAGS] = 0x01;

				byte[] loader = _arduinoComm.ReadLoader();
				if ( loader == null || loader.Length != Tapecart.LOADER_LENGTH ) {
					_logging.Warn(MODUL_NAME, "ReadTcrtBtn_Click", $"Error reading loader");
					TcrtInfoTb.Text = $"Error reading loader";
					return;
				}
				Buffer.BlockCopy(loader, 0, tcrtImage, Tapecart.OFFSET_LOADER, Tapecart.LOADER_LENGTH);
			}
			else {
				tcrtImage[Tapecart.OFFSET_FLAGS] = 0x00;
				// filling with 0x00 is not really neccessary but makes the TCRT file equal if you compare it to existing files
				ByteArray.Fill(tcrtImage, Tapecart.OFFSET_LOADER, 0x00, Tapecart.LOADER_LENGTH);
			}

			// read loader info

			int loaderOffset;
			int loaderLength;
			int loaderCallAddr;
			string filename;
			if (!_arduinoComm.ReadLoadInfo(out loaderOffset, out loaderLength, out loaderCallAddr, out filename)) {
				_logging.Warn(MODUL_NAME, "ReadTcrtBtn_Click", $"Error reading loader info");
				TcrtInfoTb.Text = $"Error reading loader info";
				return;
			}
			ByteArray.WriteUInt16(tcrtImage, Tapecart.OFFSET_LOADER_OFFSET, (UInt16)loaderOffset);
			ByteArray.WriteUInt16(tcrtImage, Tapecart.OFFSET_LOADER_LENGTH, (UInt16)loaderLength);
			ByteArray.WriteUInt16(tcrtImage, Tapecart.OFFSET_LOADER_CALLADDR, (UInt16)loaderCallAddr);
			ByteArray.WriteASCIIString(tcrtImage, Tapecart.OFFSET_LOADER_NAME, filename, 0x20);

			int tcrtSize = Tapecart.MAX_FLASH_SIZE;

			TcrtInfoTb.Text = $"Loader info={loaderOffset:X4}h,{loaderLength:X4}h,{loaderCallAddr:X4}h,{filename.Trim()}";
			_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", $"Loader info={loaderOffset:X4}h,{loaderLength:X4}h,{loaderCallAddr:X4}h,{filename.Trim()}");


			// read flash data

			string btnText = ReadTcrtBtn.Text;
			ReadTcrtBtn.Text = "Cancel read";
			WriteTcrtBtn.Enabled = false;

			ReadWritePb.Minimum = 0;
			ReadWritePb.Maximum = 100;
			ReadWritePb.Step = 1;
			ReadWritePb.Value = 0;

			_readActive = true;
			_readWriteCanceled = false;
			SetButtonStatus();

			await Task.Run(() =>
			{
				int flashOffset = 0;
				Crc32 calcCrc = new Crc32();
				Stopwatch sw = new Stopwatch();
				sw.Start();
				while (flashOffset < tcrtSize)
				{
					if (!_readActive)
					{   // cancel
						_readWriteCanceled = true; // <- must be set before invoke !!!
						UpdateReadWriteStatus("Read canceled", Color.Red);
						_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", $"Read canceled at {flashOffset:X6}");
						UpdateReadWriteBar(0);
						return;
					}

					byte[] page = _arduinoComm.ReadFlashPage(flashOffset);
					if (page == null)
					{
						UpdateReadWriteStatus($"Error at offset={flashOffset:X6}", Color.Red);
						_readWriteCanceled = true;
						return;
					}

					calcCrc.Update(page, 0, Tapecart.BLOCK_SIZE);

					Buffer.BlockCopy(page, 0, tcrtImage, flashOffset + Tapecart.HEADER_SIZE, Tapecart.BLOCK_SIZE);

					flashOffset += Tapecart.BLOCK_SIZE;

					if (flashOffset % 0x1000 == 0)
					{
						if (flashOffset == 0x1000)
						{
							Helper.ControlInvokeRequired(BrowserInfoTb, () =>
							{
								BrowserInfoTb.Text = TapecartBrowser.GetInfo(tcrtImage, Tapecart.HEADER_SIZE);
							});
						}
						UpdateReadWriteStatus($"Reading: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}");
						UpdateReadWriteBar(flashOffset * 100 / tcrtSize);
					}
				}

				if (ChecksumCb.Checked)
				{
					_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", "Checksum test...");
					UpdateReadWriteStatus($"Checksum-Test...");
					UInt32 crc;
					Stopwatch sw2 = new Stopwatch();
					sw2.Start();
					_arduinoComm.Crc32Flash(0, (int)tcrtSize, out crc);
					Debug.WriteLine($"crc-time={Minutes(sw2.ElapsedMilliseconds)}");
					if (crc != calcCrc.Crc)
					{
						Debug.WriteLine($"checksum error calc={calcCrc.Crc:X8} flash={crc:X8}");
						_logging.Warn(MODUL_NAME, "ReadTcrtBtn_Click", $"checksum error calc={calcCrc.Crc:X8} flash={crc:X8}");
						UpdateReadWriteStatus($"Checksum error", Color.Red);
						return;
					}
					else
						_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", $"checksum ok {crc:X8}");

				}

				_logging.Info(MODUL_NAME, "ReadTcrtBtn_Click", $"Read finished: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}");
				UpdateReadWriteStatus($"Read finished: Offset={flashOffset:X6}h Time={Minutes(sw.ElapsedMilliseconds)}", Color.Green);
				UpdateReadWriteBar(100);
			});

			if (!_readWriteCanceled)
			{
				// write tcrt file, should write without error because we tested it before
				try {
					File.WriteAllBytes(saveFileDialog.FileName, tcrtImage);
				}
				catch (Exception ex) {
					_logging.Warn(MODUL_NAME, "ReadTcrtBtn_Click", $"Error writing tcrt file '{logFilename}'");
					TcrtInfoTb.Text = $"Error writing TCRT file";
				}
			}

			_readActive = false;
			_readWriteCanceled = true;
			ReadTcrtBtn.Text = btnText;
			SetButtonStatus();
		}

		private void AskForSketchUpdate(SketchVersion latest) {

			_logging.Info(MODUL_NAME, "AskForSketchUpdate", $"current={_currentSketchVersion} latest={latest}");

			DialogResult terms = MessageBox.Show(
				"The Arduiono Tapecart Flasher Version does not match the current Version.\r\n" +
				$"Do you want to update from version {_currentSketchVersion.VerStr} to {latest.VerStr}?",
				"Arduino Flasher Update?",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Warning,
				MessageBoxDefaultButton.Button2);

			if (terms == DialogResult.Yes)
			{
				UpdateSketch();
			}
		}

		private void UpdateSketchBtn_Click(object sender, EventArgs e)
		{
			if (_selectedComPort == null)
				return;

			UpdateSketch();
		}

		private void UpdateSketch() {

			Disconnect();

			UpdateView updateView = new UpdateView(this.Bounds, "Update Sketch", _selectedComPort.ComPort, _currentSketchVersion);
			updateView.ShowDialog();

			// because we are disconnected
			FlasherVersionTb.Text = "";
			TapecartVersionTb.Text = "";
		}

		private void MainView_ResizeEnd(object sender, EventArgs e)
		{
			//Debug.WriteLine($"{Width} / {Height}");
		}

		private void UpdateReadWriteStatus(string text, Color? color = null)
		{
			if (color == null)
				color = Color.Black;

			Helper.ControlInvokeRequired(ReadWriteStatusLbl, () => {
				ReadWriteStatusLbl.Text = text;
				ReadWriteStatusLbl.ForeColor = color.Value;
			});
		}

		/// <summary>
		/// Updates the read write bar.
		/// </summary>
		/// <param name="value">The value (0..100)</param>
		private void UpdateReadWriteBar(int value)
		{
			Helper.ControlInvokeRequired(ReadWritePb, () =>
			{
				ReadWritePb.Value = value;
			});
			Helper.ControlInvokeRequired(ReadWritePercentLbl, () =>
			{
				ReadWritePercentLbl.Text = $"{value}%";
				ReadWritePercentLbl.Refresh();
			});
		}

		/// <summary>
		/// Minimums the specified time.
		/// </summary>
		/// <param name="time">The time in milliseconds</param>
		/// <returns></returns>
		private string Minutes(long time)
		{
			time /= 1000;
			return $"{time / 60}:{time % 60:D2}";
		}

		private void AboutBtn_Click(object sender, EventArgs e)
		{
			string text =
				$"{Helper.GetVersion()}\r\r" +
				"by *dg* Detlef Gerhardt\r\r" +
				"Send feedback to\r" +
				"feedback@dgerhardt.de";
			MessageBox.Show(
				text,
				"About Tapecart Flasher",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button1);
		}

	}
}
