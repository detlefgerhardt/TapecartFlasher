using System;
using System.Drawing;
using System.Windows.Forms;
using ArduinoUploader;
using ArduinoUploader.Hardware;

namespace TapecartFlasher
{

	partial class UpdateView : Form {

		private const string MODUL_NAME = "UpdateView";

		private Logging _logging = Logging.Instance;

		private string _comPortName;

		private Rectangle _parentWindowsPosition;

		private SketchList _sketchList = SketchList.Instance;

		private SketchVersion _latestVersion;

		private Timer startTimer;

		public static SketchVersion LatestSketchVersion { get; set; }  = new SketchVersion(0, 1, 1, SketchVersion.ArduinoType.ARDUINO_UNO);

		public UpdateView(Rectangle position, string msg, string comPortName, SketchVersion oldVersion) {

			_parentWindowsPosition = position;

			InitializeComponent();

			_sketchList.ReadSketchList();
			_latestVersion = _sketchList.GetLatestVersion(oldVersion);

			MessageLbl.Text = "";

			if ( oldVersion != null )
				OldVersion.Text = $"{oldVersion.DisplayName} Tapecart Flasher v{oldVersion.VerStr}";
			else
				OldVersion.Text = "---";

			if ( _latestVersion != null )
				NewVersion.Text = $"{_latestVersion.DisplayName} Tapecart Flasher v{_latestVersion.VerStr}";
			else {
				NewVersion.Text = "---";
				_latestVersion = _sketchList.GetLatestVersion(oldVersion, true);
			}

			UpdateProgress(0.0);

			// die Zuweisung an DataSource löst SelectedIndexChanged aus und überschreibt _latestVersion
			// work around:
			BoardTypeCb.Items.Clear();
			foreach ( SketchVersion f in _sketchList.SketchVersions )
				BoardTypeCb.Items.Add(f);

			BoardTypeCb.SelectedItem = _latestVersion;
			BoardTypeCb.DisplayMember = "DisplayName";
			if ( oldVersion != null )
				BoardTypeCb.Enabled = false;

			_comPortName = comPortName;
		}

		private void UpdateView_Load(object sender, EventArgs e) {
			// center form relative to parent windows
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void BoardTypeCb_SelectedIndexChanged(object sender, EventArgs e) {
			_latestVersion = (SketchVersion)BoardTypeCb.SelectedItem;
			NewVersion.Text = $"{_latestVersion.DisplayName} Tapecart Flasher v{_latestVersion.VerStr}";
		}

		private void UpdateBtn_Click(object sender, EventArgs e) {
			CloseBtn.Enabled = false;
			UploadSketch();
			CloseBtn.Enabled = true;
		}

		private void UploadSketch()
		{
			ArduinoProgress progress = new ArduinoProgress(UpdateProgress);

			ArduinoModel? model = SketchVersion.ArduinoTypeToModel(_latestVersion.Type);
			if ( model == null ) {
				_logging.Error(MODUL_NAME, "UploadSketch", $"invalid arduino type {_latestVersion.Type}");
				return;
			}

			var uploader = new ArduinoSketchUploader(
				new ArduinoSketchUploaderOptions() {
					FileName = _latestVersion.Filename,
					PortName = _comPortName,
					ArduinoModel = model.Value,
				},
				new ArduinoLogger(),
				progress
			);

			/*
			string hexStr = Helper.GetEmbeddedTextRessource(LatestSketchName);
			if (string.IsNullOrEmpty(hexStr))
			{
				_logging.Error(MODUL_NAME, "UploadSketch", $"Error reading embedded resource '{LatestSketchName}'");
				return;
			}
			string[] hexLines = hexStr.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			*/

			Helper.ControlInvokeRequired(StatusLbl, () => {
				StatusLbl.Text = "Uploading Sketch";
			});

			Helper.ControlInvokeRequired(UploadPb, () => {
				UploadPb.Minimum = 0;
				UploadPb.Maximum = 100;
				UploadPb.Step = 1;
				UploadPb.Value = 0;
			});

			try {
				//uploader.UploadSketch(hexLines);
				uploader.UploadSketch();
			}
			catch ( Exception ex ) {
				_logging.Error(MODUL_NAME, "UploadSketch", $"UploadSketch error ex={ex}");
				Helper.ControlInvokeRequired(StatusLbl, () => {
					StatusLbl.Text = "Sketch upload error";
				});
				return;
			}

			UpdateProgress(1.0);
			Helper.ControlInvokeRequired(StatusLbl, () => {
				StatusLbl.Text = "Sketch upload completed";
			});
		}

		private void UpdateProgress(double value)
		{
			int progress = (int)(value * 100 + 0.5);

			Helper.ControlInvokeRequired(UploadPb, () =>
			{
				UploadPb.Value = progress;
				UploadPb.Refresh();
			});
			Helper.ControlInvokeRequired(PercentLbl, () => {
				PercentLbl.Text = $"{progress}%";
				PercentLbl.Refresh();
			});
		}

		private void CloseBtn_Click(object sender, EventArgs e) {
			Close();
		}

	}

	public class ArduinoProgress : IProgress<double>
	{
		private Action<double> _updater;

		public ArduinoProgress(Action<double> updater)
		{
			_updater = updater;
		}

		public void Report(double value)
		{
			_updater(value);
		}
	}
}
