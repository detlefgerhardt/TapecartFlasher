namespace TapecartFlasher {
    partial class UpdateView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateView));
			this.MessageLbl = new System.Windows.Forms.Label();
			this.UploadPb = new System.Windows.Forms.ProgressBar();
			this.PercentLbl = new System.Windows.Forms.Label();
			this.CloseBtn = new System.Windows.Forms.Button();
			this.StatusLbl = new System.Windows.Forms.Label();
			this.UpdateBtn = new System.Windows.Forms.Button();
			this.BoardTypeCb = new System.Windows.Forms.ComboBox();
			this.BoardTypeLbl = new System.Windows.Forms.Label();
			this.OldVersionLbl = new System.Windows.Forms.Label();
			this.NewVersionLbl = new System.Windows.Forms.Label();
			this.OldVersion = new System.Windows.Forms.Label();
			this.NewVersion = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// MessageLbl
			// 
			this.MessageLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MessageLbl.Location = new System.Drawing.Point(5, 6);
			this.MessageLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.MessageLbl.Name = "MessageLbl";
			this.MessageLbl.Size = new System.Drawing.Size(366, 36);
			this.MessageLbl.TabIndex = 0;
			this.MessageLbl.Text = "Msg";
			// 
			// UploadPb
			// 
			this.UploadPb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.UploadPb.Location = new System.Drawing.Point(8, 76);
			this.UploadPb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.UploadPb.Name = "UploadPb";
			this.UploadPb.Size = new System.Drawing.Size(329, 21);
			this.UploadPb.TabIndex = 1;
			// 
			// PercentLbl
			// 
			this.PercentLbl.AutoSize = true;
			this.PercentLbl.Location = new System.Drawing.Point(341, 80);
			this.PercentLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.PercentLbl.Name = "PercentLbl";
			this.PercentLbl.Size = new System.Drawing.Size(33, 13);
			this.PercentLbl.TabIndex = 2;
			this.PercentLbl.Text = "100%";
			// 
			// CloseBtn
			// 
			this.CloseBtn.Location = new System.Drawing.Point(191, 136);
			this.CloseBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.CloseBtn.Name = "CloseBtn";
			this.CloseBtn.Size = new System.Drawing.Size(75, 23);
			this.CloseBtn.TabIndex = 3;
			this.CloseBtn.Text = "Close";
			this.CloseBtn.UseVisualStyleBackColor = true;
			this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
			// 
			// StatusLbl
			// 
			this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.StatusLbl.Location = new System.Drawing.Point(7, 99);
			this.StatusLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.StatusLbl.Name = "StatusLbl";
			this.StatusLbl.Size = new System.Drawing.Size(367, 20);
			this.StatusLbl.TabIndex = 4;
			this.StatusLbl.Text = "Status";
			this.StatusLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UpdateBtn
			// 
			this.UpdateBtn.Location = new System.Drawing.Point(113, 136);
			this.UpdateBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.UpdateBtn.Name = "UpdateBtn";
			this.UpdateBtn.Size = new System.Drawing.Size(75, 23);
			this.UpdateBtn.TabIndex = 5;
			this.UpdateBtn.Text = "Update";
			this.UpdateBtn.UseVisualStyleBackColor = true;
			this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
			// 
			// BoardTypeCb
			// 
			this.BoardTypeCb.FormattingEnabled = true;
			this.BoardTypeCb.Location = new System.Drawing.Point(155, 48);
			this.BoardTypeCb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.BoardTypeCb.Name = "BoardTypeCb";
			this.BoardTypeCb.Size = new System.Drawing.Size(182, 21);
			this.BoardTypeCb.TabIndex = 6;
			this.BoardTypeCb.SelectedIndexChanged += new System.EventHandler(this.BoardTypeCb_SelectedIndexChanged);
			// 
			// BoardTypeLbl
			// 
			this.BoardTypeLbl.AutoSize = true;
			this.BoardTypeLbl.Location = new System.Drawing.Point(8, 51);
			this.BoardTypeLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.BoardTypeLbl.Name = "BoardTypeLbl";
			this.BoardTypeLbl.Size = new System.Drawing.Size(141, 13);
			this.BoardTypeLbl.TabIndex = 7;
			this.BoardTypeLbl.Text = "Board type - Sketch version:";
			// 
			// OldVersionLbl
			// 
			this.OldVersionLbl.AutoSize = true;
			this.OldVersionLbl.Location = new System.Drawing.Point(5, 6);
			this.OldVersionLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.OldVersionLbl.Name = "OldVersionLbl";
			this.OldVersionLbl.Size = new System.Drawing.Size(63, 13);
			this.OldVersionLbl.TabIndex = 8;
			this.OldVersionLbl.Text = "Old version:";
			// 
			// NewVersionLbl
			// 
			this.NewVersionLbl.AutoSize = true;
			this.NewVersionLbl.Location = new System.Drawing.Point(5, 21);
			this.NewVersionLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.NewVersionLbl.Name = "NewVersionLbl";
			this.NewVersionLbl.Size = new System.Drawing.Size(69, 13);
			this.NewVersionLbl.TabIndex = 9;
			this.NewVersionLbl.Text = "New version:";
			// 
			// OldVersion
			// 
			this.OldVersion.AutoSize = true;
			this.OldVersion.Location = new System.Drawing.Point(70, 6);
			this.OldVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.OldVersion.Name = "OldVersion";
			this.OldVersion.Size = new System.Drawing.Size(23, 13);
			this.OldVersion.TabIndex = 10;
			this.OldVersion.Text = "Old";
			// 
			// NewVersion
			// 
			this.NewVersion.AutoSize = true;
			this.NewVersion.Location = new System.Drawing.Point(70, 21);
			this.NewVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.NewVersion.Name = "NewVersion";
			this.NewVersion.Size = new System.Drawing.Size(29, 13);
			this.NewVersion.TabIndex = 11;
			this.NewVersion.Text = "New";
			// 
			// UpdateView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 162);
			this.Controls.Add(this.NewVersion);
			this.Controls.Add(this.OldVersion);
			this.Controls.Add(this.NewVersionLbl);
			this.Controls.Add(this.OldVersionLbl);
			this.Controls.Add(this.BoardTypeLbl);
			this.Controls.Add(this.BoardTypeCb);
			this.Controls.Add(this.UpdateBtn);
			this.Controls.Add(this.StatusLbl);
			this.Controls.Add(this.CloseBtn);
			this.Controls.Add(this.PercentLbl);
			this.Controls.Add(this.UploadPb);
			this.Controls.Add(this.MessageLbl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "UpdateView";
			this.Text = "Update Arduino Sketch";
			this.Load += new System.EventHandler(this.UpdateView_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageLbl;
        private System.Windows.Forms.ProgressBar UploadPb;
        private System.Windows.Forms.Label PercentLbl;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Label StatusLbl;
        private System.Windows.Forms.Button UpdateBtn;
        private System.Windows.Forms.ComboBox BoardTypeCb;
        private System.Windows.Forms.Label BoardTypeLbl;
        private System.Windows.Forms.Label OldVersionLbl;
        private System.Windows.Forms.Label NewVersionLbl;
        private System.Windows.Forms.Label OldVersion;
        private System.Windows.Forms.Label NewVersion;
    }
}