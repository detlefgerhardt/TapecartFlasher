namespace TapecartFlasher
{
	partial class MainView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
			this.CloseBtn = new System.Windows.Forms.Button();
			this.ComPortCb = new System.Windows.Forms.ComboBox();
			this.ConnectBtn = new System.Windows.Forms.Button();
			this.FlasherVersionTb = new System.Windows.Forms.TextBox();
			this.SketchVersionLbl = new System.Windows.Forms.Label();
			this.TcrtInfoTb = new System.Windows.Forms.TextBox();
			this.TabcartVersionLbl = new System.Windows.Forms.Label();
			this.TapecartVersionTb = new System.Windows.Forms.TextBox();
			this.TcrtInfoLbl = new System.Windows.Forms.Label();
			this.DetectBtn = new System.Windows.Forms.Button();
			this.WriteTcrtBtn = new System.Windows.Forms.Button();
			this.ReadTcrtBtn = new System.Windows.Forms.Button();
			this.TcrtFilenameTb = new System.Windows.Forms.TextBox();
			this.TcrtFilenameLbl = new System.Windows.Forms.Label();
			this.ReadWritePb = new System.Windows.Forms.ProgressBar();
			this.ReadWriteStatusLbl = new System.Windows.Forms.Label();
			this.ReadWritePercentLbl = new System.Windows.Forms.Label();
			this.UpdateSketchBtn = new System.Windows.Forms.Button();
			this.AboutBtn = new System.Windows.Forms.Button();
			this.TestBtn = new System.Windows.Forms.Button();
			this.ChecksumCb = new System.Windows.Forms.CheckBox();
			this.BrowserInfoLbl = new System.Windows.Forms.Label();
			this.BrowserInfoTb = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// CloseBtn
			// 
			this.CloseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseBtn.Location = new System.Drawing.Point(502, 253);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(75, 23);
			this.CloseBtn.TabIndex = 0;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// ComPortCb
			// 
			this.ComPortCb.FormattingEnabled = true;
			this.ComPortCb.Location = new System.Drawing.Point(98, 10);
			this.ComPortCb.Name = "ComPortCb";
			this.ComPortCb.Size = new System.Drawing.Size(121, 21);
			this.ComPortCb.TabIndex = 1;
			this.ComPortCb.SelectedIndexChanged += new System.EventHandler(this.ComPortCb_SelectedIndexChanged);
			// 
			// ConnectBtn
			// 
			this.ConnectBtn.Location = new System.Drawing.Point(223, 9);
			this.ConnectBtn.Name = "ConnectBtn";
			this.ConnectBtn.Size = new System.Drawing.Size(75, 23);
			this.ConnectBtn.TabIndex = 2;
			this.ConnectBtn.Text = "Connect";
			this.ConnectBtn.UseVisualStyleBackColor = true;
			this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
			// 
			// FlasherVersionTb
			// 
			this.FlasherVersionTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FlasherVersionTb.Location = new System.Drawing.Point(98, 42);
			this.FlasherVersionTb.Name = "FlasherVersionTb";
			this.FlasherVersionTb.ReadOnly = true;
			this.FlasherVersionTb.Size = new System.Drawing.Size(480, 20);
			this.FlasherVersionTb.TabIndex = 3;
			// 
			// SketchVersionLbl
			// 
			this.SketchVersionLbl.AutoSize = true;
			this.SketchVersionLbl.Location = new System.Drawing.Point(10, 45);
			this.SketchVersionLbl.Name = "SketchVersionLbl";
			this.SketchVersionLbl.Size = new System.Drawing.Size(82, 13);
			this.SketchVersionLbl.TabIndex = 4;
			this.SketchVersionLbl.Text = "Flasher Version:";
			// 
			// TcrtInfoTb
			// 
			this.TcrtInfoTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TcrtInfoTb.Location = new System.Drawing.Point(254, 145);
			this.TcrtInfoTb.Name = "TcrtInfoTb";
			this.TcrtInfoTb.ReadOnly = true;
			this.TcrtInfoTb.Size = new System.Drawing.Size(324, 20);
			this.TcrtInfoTb.TabIndex = 6;
			// 
			// TabcartVersionLbl
			// 
			this.TabcartVersionLbl.AutoSize = true;
			this.TabcartVersionLbl.Location = new System.Drawing.Point(10, 71);
			this.TabcartVersionLbl.Name = "TabcartVersionLbl";
			this.TabcartVersionLbl.Size = new System.Drawing.Size(74, 13);
			this.TabcartVersionLbl.TabIndex = 7;
			this.TabcartVersionLbl.Text = "Tapecart Info:";
			// 
			// TapecartVersionTb
			// 
			this.TapecartVersionTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TapecartVersionTb.Location = new System.Drawing.Point(98, 68);
			this.TapecartVersionTb.Name = "TapecartVersionTb";
			this.TapecartVersionTb.ReadOnly = true;
			this.TapecartVersionTb.Size = new System.Drawing.Size(480, 20);
			this.TapecartVersionTb.TabIndex = 8;
			// 
			// TcrtInfoLbl
			// 
			this.TcrtInfoLbl.AutoSize = true;
			this.TcrtInfoLbl.Location = new System.Drawing.Point(188, 151);
			this.TcrtInfoLbl.Name = "TcrtInfoLbl";
			this.TcrtInfoLbl.Size = new System.Drawing.Size(60, 13);
			this.TcrtInfoLbl.TabIndex = 9;
			this.TcrtInfoLbl.Text = "TCRT Info:";
			// 
			// DetectBtn
			// 
			this.DetectBtn.Location = new System.Drawing.Point(13, 9);
			this.DetectBtn.Name = "DetectBtn";
			this.DetectBtn.Size = new System.Drawing.Size(75, 23);
			this.DetectBtn.TabIndex = 11;
			this.DetectBtn.Text = "Detect";
			this.DetectBtn.UseVisualStyleBackColor = true;
			this.DetectBtn.Click += new System.EventHandler(this.DetectBtn_Click);
			// 
			// WriteTcrtBtn
			// 
			this.WriteTcrtBtn.Location = new System.Drawing.Point(12, 117);
			this.WriteTcrtBtn.Name = "WriteTcrtBtn";
			this.WriteTcrtBtn.Size = new System.Drawing.Size(160, 23);
			this.WriteTcrtBtn.TabIndex = 15;
			this.WriteTcrtBtn.Text = "Write TCRT file to Tapcart";
			this.WriteTcrtBtn.UseVisualStyleBackColor = true;
			this.WriteTcrtBtn.Click += new System.EventHandler(this.WriteTcrtBtn_Click);
			// 
			// ReadTcrtBtn
			// 
			this.ReadTcrtBtn.Location = new System.Drawing.Point(12, 146);
			this.ReadTcrtBtn.Name = "ReadTcrtBtn";
			this.ReadTcrtBtn.Size = new System.Drawing.Size(160, 23);
			this.ReadTcrtBtn.TabIndex = 16;
			this.ReadTcrtBtn.Text = "Read TCRT file from Tapecart";
			this.ReadTcrtBtn.UseVisualStyleBackColor = true;
			this.ReadTcrtBtn.Click += new System.EventHandler(this.ReadTcrtBtn_Click);
			// 
			// TcrtFilenameTb
			// 
			this.TcrtFilenameTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TcrtFilenameTb.Location = new System.Drawing.Point(254, 119);
			this.TcrtFilenameTb.Name = "TcrtFilenameTb";
			this.TcrtFilenameTb.ReadOnly = true;
			this.TcrtFilenameTb.Size = new System.Drawing.Size(324, 20);
			this.TcrtFilenameTb.TabIndex = 17;
			// 
			// TcrtFilenameLbl
			// 
			this.TcrtFilenameLbl.AutoSize = true;
			this.TcrtFilenameLbl.Location = new System.Drawing.Point(193, 122);
			this.TcrtFilenameLbl.Name = "TcrtFilenameLbl";
			this.TcrtFilenameLbl.Size = new System.Drawing.Size(55, 13);
			this.TcrtFilenameLbl.TabIndex = 18;
			this.TcrtFilenameLbl.Text = "File name:";
			// 
			// ReadWritePb
			// 
			this.ReadWritePb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ReadWritePb.Location = new System.Drawing.Point(13, 204);
			this.ReadWritePb.Margin = new System.Windows.Forms.Padding(2);
			this.ReadWritePb.Name = "ReadWritePb";
			this.ReadWritePb.Size = new System.Drawing.Size(526, 21);
			this.ReadWritePb.TabIndex = 19;
			// 
			// ReadWriteStatusLbl
			// 
			this.ReadWriteStatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ReadWriteStatusLbl.Location = new System.Drawing.Point(13, 227);
			this.ReadWriteStatusLbl.Name = "ReadWriteStatusLbl";
			this.ReadWriteStatusLbl.Size = new System.Drawing.Size(559, 18);
			this.ReadWriteStatusLbl.TabIndex = 20;
			this.ReadWriteStatusLbl.Text = "Status";
			this.ReadWriteStatusLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ReadWritePercentLbl
			// 
			this.ReadWritePercentLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ReadWritePercentLbl.AutoSize = true;
			this.ReadWritePercentLbl.Location = new System.Drawing.Point(543, 208);
			this.ReadWritePercentLbl.Name = "ReadWritePercentLbl";
			this.ReadWritePercentLbl.Size = new System.Drawing.Size(33, 13);
			this.ReadWritePercentLbl.TabIndex = 21;
			this.ReadWritePercentLbl.Text = "100%";
			// 
			// UpdateSketchBtn
			// 
			this.UpdateSketchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.UpdateSketchBtn.Location = new System.Drawing.Point(435, 9);
			this.UpdateSketchBtn.Name = "UpdateSketchBtn";
			this.UpdateSketchBtn.Size = new System.Drawing.Size(141, 23);
			this.UpdateSketchBtn.TabIndex = 22;
			this.UpdateSketchBtn.Text = "Upload / Update Sketch";
			this.UpdateSketchBtn.UseVisualStyleBackColor = true;
			this.UpdateSketchBtn.Click += new System.EventHandler(this.UpdateSketchBtn_Click);
			// 
			// AboutBtn
			// 
			this.AboutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.AboutBtn.Location = new System.Drawing.Point(13, 253);
			this.AboutBtn.Name = "AboutBtn";
			this.AboutBtn.Size = new System.Drawing.Size(75, 23);
			this.AboutBtn.TabIndex = 23;
			this.AboutBtn.Text = "About";
			this.AboutBtn.UseVisualStyleBackColor = true;
			this.AboutBtn.Click += new System.EventHandler(this.AboutBtn_Click);
			// 
			// TestBtn
			// 
			this.TestBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.TestBtn.Location = new System.Drawing.Point(93, 253);
			this.TestBtn.Name = "TestBtn";
			this.TestBtn.Size = new System.Drawing.Size(75, 23);
			this.TestBtn.TabIndex = 24;
			this.TestBtn.Text = "Test";
			this.TestBtn.UseVisualStyleBackColor = true;
			this.TestBtn.Click += new System.EventHandler(this.TestBtn_Click);
			// 
			// ChecksumCb
			// 
			this.ChecksumCb.AutoSize = true;
			this.ChecksumCb.Location = new System.Drawing.Point(16, 97);
			this.ChecksumCb.Name = "ChecksumCb";
			this.ChecksumCb.Size = new System.Drawing.Size(181, 17);
			this.ChecksumCb.TabIndex = 25;
			this.ChecksumCb.Text = "Checksum test after write or read";
			this.ChecksumCb.UseVisualStyleBackColor = true;
			// 
			// BrowserInfoLbl
			// 
			this.BrowserInfoLbl.AutoSize = true;
			this.BrowserInfoLbl.Location = new System.Drawing.Point(179, 174);
			this.BrowserInfoLbl.Name = "BrowserInfoLbl";
			this.BrowserInfoLbl.Size = new System.Drawing.Size(69, 13);
			this.BrowserInfoLbl.TabIndex = 27;
			this.BrowserInfoLbl.Text = "Browser Info:";
			// 
			// BrowserInfoTb
			// 
			this.BrowserInfoTb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.BrowserInfoTb.Location = new System.Drawing.Point(254, 171);
			this.BrowserInfoTb.Name = "BrowserInfoTb";
			this.BrowserInfoTb.ReadOnly = true;
			this.BrowserInfoTb.Size = new System.Drawing.Size(324, 20);
			this.BrowserInfoTb.TabIndex = 26;
			// 
			// MainView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(585, 285);
			this.Controls.Add(this.BrowserInfoLbl);
			this.Controls.Add(this.BrowserInfoTb);
			this.Controls.Add(this.ChecksumCb);
			this.Controls.Add(this.TestBtn);
			this.Controls.Add(this.AboutBtn);
			this.Controls.Add(this.UpdateSketchBtn);
			this.Controls.Add(this.ReadWritePercentLbl);
			this.Controls.Add(this.ReadWriteStatusLbl);
			this.Controls.Add(this.ReadWritePb);
			this.Controls.Add(this.TcrtFilenameLbl);
			this.Controls.Add(this.TcrtFilenameTb);
			this.Controls.Add(this.ReadTcrtBtn);
			this.Controls.Add(this.WriteTcrtBtn);
			this.Controls.Add(this.DetectBtn);
			this.Controls.Add(this.TcrtInfoLbl);
			this.Controls.Add(this.TapecartVersionTb);
			this.Controls.Add(this.TabcartVersionLbl);
			this.Controls.Add(this.TcrtInfoTb);
			this.Controls.Add(this.SketchVersionLbl);
			this.Controls.Add(this.FlasherVersionTb);
			this.Controls.Add(this.ConnectBtn);
			this.Controls.Add(this.ComPortCb);
			this.Controls.Add(this.CloseBtn);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainView";
			this.Text = "Tapecart Flasher";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainView_FormClosing);
			this.Load += new System.EventHandler(this.MainView_Load);
			this.ResizeEnd += new System.EventHandler(this.MainView_ResizeEnd);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CloseBtn;
		private System.Windows.Forms.ComboBox ComPortCb;
		private System.Windows.Forms.Button ConnectBtn;
		private System.Windows.Forms.TextBox FlasherVersionTb;
		private System.Windows.Forms.Label SketchVersionLbl;
		private System.Windows.Forms.TextBox TcrtInfoTb;
		private System.Windows.Forms.Label TabcartVersionLbl;
		private System.Windows.Forms.TextBox TapecartVersionTb;
		private System.Windows.Forms.Label TcrtInfoLbl;
		private System.Windows.Forms.Button DetectBtn;
		private System.Windows.Forms.Button WriteTcrtBtn;
		private System.Windows.Forms.Button ReadTcrtBtn;
		private System.Windows.Forms.TextBox TcrtFilenameTb;
		private System.Windows.Forms.Label TcrtFilenameLbl;
		private System.Windows.Forms.ProgressBar ReadWritePb;
		private System.Windows.Forms.Label ReadWriteStatusLbl;
		private System.Windows.Forms.Label ReadWritePercentLbl;
		private System.Windows.Forms.Button UpdateSketchBtn;
        private System.Windows.Forms.Button AboutBtn;
        private System.Windows.Forms.Button TestBtn;
		private System.Windows.Forms.CheckBox ChecksumCb;
		private System.Windows.Forms.Label BrowserInfoLbl;
		private System.Windows.Forms.TextBox BrowserInfoTb;
	}
}