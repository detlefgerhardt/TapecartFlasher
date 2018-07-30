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
            this.MessageLbl.Location = new System.Drawing.Point(8, 9);
            this.MessageLbl.Name = "MessageLbl";
            this.MessageLbl.Size = new System.Drawing.Size(549, 56);
            this.MessageLbl.TabIndex = 0;
            this.MessageLbl.Text = "Msg";
            // 
            // UploadPb
            // 
            this.UploadPb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UploadPb.Location = new System.Drawing.Point(12, 117);
            this.UploadPb.Name = "UploadPb";
            this.UploadPb.Size = new System.Drawing.Size(494, 33);
            this.UploadPb.TabIndex = 1;
            // 
            // PercentLbl
            // 
            this.PercentLbl.AutoSize = true;
            this.PercentLbl.Location = new System.Drawing.Point(512, 123);
            this.PercentLbl.Name = "PercentLbl";
            this.PercentLbl.Size = new System.Drawing.Size(50, 20);
            this.PercentLbl.TabIndex = 2;
            this.PercentLbl.Text = "100%";
            // 
            // CloseBtn
            // 
            this.CloseBtn.Location = new System.Drawing.Point(287, 209);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(112, 35);
            this.CloseBtn.TabIndex = 3;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // StatusLbl
            // 
            this.StatusLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLbl.Location = new System.Drawing.Point(11, 153);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(551, 31);
            this.StatusLbl.TabIndex = 4;
            this.StatusLbl.Text = "Status";
            this.StatusLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateBtn
            // 
            this.UpdateBtn.Location = new System.Drawing.Point(169, 209);
            this.UpdateBtn.Name = "UpdateBtn";
            this.UpdateBtn.Size = new System.Drawing.Size(112, 35);
            this.UpdateBtn.TabIndex = 5;
            this.UpdateBtn.Text = "Update";
            this.UpdateBtn.UseVisualStyleBackColor = true;
            this.UpdateBtn.Click += new System.EventHandler(this.UpdateBtn_Click);
            // 
            // BoardTypeCb
            // 
            this.BoardTypeCb.FormattingEnabled = true;
            this.BoardTypeCb.Location = new System.Drawing.Point(108, 75);
            this.BoardTypeCb.Name = "BoardTypeCb";
            this.BoardTypeCb.Size = new System.Drawing.Size(186, 28);
            this.BoardTypeCb.TabIndex = 6;
            this.BoardTypeCb.SelectedIndexChanged += new System.EventHandler(this.BoardTypeCb_SelectedIndexChanged);
            // 
            // BoardTypeLbl
            // 
            this.BoardTypeLbl.AutoSize = true;
            this.BoardTypeLbl.Location = new System.Drawing.Point(12, 78);
            this.BoardTypeLbl.Name = "BoardTypeLbl";
            this.BoardTypeLbl.Size = new System.Drawing.Size(90, 20);
            this.BoardTypeLbl.TabIndex = 7;
            this.BoardTypeLbl.Text = "Board type:";
            // 
            // OldVersionLbl
            // 
            this.OldVersionLbl.AutoSize = true;
            this.OldVersionLbl.Location = new System.Drawing.Point(8, 9);
            this.OldVersionLbl.Name = "OldVersionLbl";
            this.OldVersionLbl.Size = new System.Drawing.Size(91, 20);
            this.OldVersionLbl.TabIndex = 8;
            this.OldVersionLbl.Text = "Old version:";
            // 
            // NewVersionLbl
            // 
            this.NewVersionLbl.AutoSize = true;
            this.NewVersionLbl.Location = new System.Drawing.Point(8, 32);
            this.NewVersionLbl.Name = "NewVersionLbl";
            this.NewVersionLbl.Size = new System.Drawing.Size(98, 20);
            this.NewVersionLbl.TabIndex = 9;
            this.NewVersionLbl.Text = "New version:";
            // 
            // OldVersion
            // 
            this.OldVersion.AutoSize = true;
            this.OldVersion.Location = new System.Drawing.Point(105, 9);
            this.OldVersion.Name = "OldVersion";
            this.OldVersion.Size = new System.Drawing.Size(33, 20);
            this.OldVersion.TabIndex = 10;
            this.OldVersion.Text = "Old";
            // 
            // NewVersion
            // 
            this.NewVersion.AutoSize = true;
            this.NewVersion.Location = new System.Drawing.Point(105, 32);
            this.NewVersion.Name = "NewVersion";
            this.NewVersion.Size = new System.Drawing.Size(40, 20);
            this.NewVersion.TabIndex = 11;
            this.NewVersion.Text = "New";
            // 
            // UpdateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 249);
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