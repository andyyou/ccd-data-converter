namespace CcdDataConverter
{
    partial class Main
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnSending = new System.Windows.Forms.Button();
            this.lblXUnit = new System.Windows.Forms.Label();
            this.lblYUnit = new System.Windows.Forms.Label();
            this.txtOffsetX = new System.Windows.Forms.TextBox();
            this.lblXOffset = new System.Windows.Forms.Label();
            this.txtOffsetY = new System.Windows.Forms.TextBox();
            this.lblYOffset = new System.Windows.Forms.Label();
            this.grpRelativeSettings = new System.Windows.Forms.GroupBox();
            this.dgvRelativeSettings = new System.Windows.Forms.DataGridView();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnListening = new System.Windows.Forms.Button();
            this.txtDestPort = new System.Windows.Forms.TextBox();
            this.lb2 = new System.Windows.Forms.Label();
            this.txtDestIP = new System.Windows.Forms.TextBox();
            this.lblDestIP = new System.Windows.Forms.Label();
            this.txtSourcePort = new System.Windows.Forms.TextBox();
            this.lb1 = new System.Windows.Forms.Label();
            this.txtSourceIP = new System.Windows.Forms.TextBox();
            this.lblSourceIP = new System.Windows.Forms.Label();
            this.stuInfo = new System.Windows.Forms.StatusStrip();
            this.stulblHardware = new System.Windows.Forms.ToolStripStatusLabel();
            this.stulblSoftware = new System.Windows.Forms.ToolStripStatusLabel();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.grpLog.SuspendLayout();
            this.grpRelativeSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelativeSettings)).BeginInit();
            this.stuInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtRate
            // 
            this.txtRate.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtRate.Location = new System.Drawing.Point(486, 12);
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(67, 22);
            this.txtRate.TabIndex = 15;
            this.txtRate.Text = "1";
            this.txtRate.Validating += new System.ComponentModel.CancelEventHandler(this.OffsetValidating);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblRate.Location = new System.Drawing.Point(447, 17);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(38, 12);
            this.lblRate.TabIndex = 14;
            this.lblRate.Text = "Rate：";
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Location = new System.Drawing.Point(432, 92);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(422, 419);
            this.grpLog.TabIndex = 19;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Message";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(17, 21);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(386, 383);
            this.txtLog.TabIndex = 0;
            // 
            // btnSending
            // 
            this.btnSending.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSending.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnSending.Location = new System.Drawing.Point(676, 6);
            this.btnSending.Name = "btnSending";
            this.btnSending.Size = new System.Drawing.Size(86, 80);
            this.btnSending.TabIndex = 17;
            this.btnSending.Text = "Send(&S)";
            this.btnSending.UseVisualStyleBackColor = true;
            this.btnSending.Click += new System.EventHandler(this.btnSending_Click);
            // 
            // lblXUnit
            // 
            this.lblXUnit.AutoSize = true;
            this.lblXUnit.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblXUnit.Location = new System.Drawing.Point(404, 45);
            this.lblXUnit.Name = "lblXUnit";
            this.lblXUnit.Size = new System.Drawing.Size(23, 12);
            this.lblXUnit.TabIndex = 13;
            this.lblXUnit.Text = "mm";
            // 
            // lblYUnit
            // 
            this.lblYUnit.AutoSize = true;
            this.lblYUnit.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblYUnit.Location = new System.Drawing.Point(404, 17);
            this.lblYUnit.Name = "lblYUnit";
            this.lblYUnit.Size = new System.Drawing.Size(23, 12);
            this.lblYUnit.TabIndex = 10;
            this.lblYUnit.Text = "mm";
            // 
            // txtOffsetX
            // 
            this.txtOffsetX.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOffsetX.Location = new System.Drawing.Point(336, 40);
            this.txtOffsetX.Name = "txtOffsetX";
            this.txtOffsetX.Size = new System.Drawing.Size(67, 22);
            this.txtOffsetX.TabIndex = 12;
            this.txtOffsetX.Validating += new System.ComponentModel.CancelEventHandler(this.OffsetValidating);
            // 
            // lblXOffset
            // 
            this.lblXOffset.AutoSize = true;
            this.lblXOffset.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblXOffset.Location = new System.Drawing.Point(279, 45);
            this.lblXOffset.Name = "lblXOffset";
            this.lblXOffset.Size = new System.Drawing.Size(56, 12);
            this.lblXOffset.TabIndex = 11;
            this.lblXOffset.Text = "X Offset：";
            // 
            // txtOffsetY
            // 
            this.txtOffsetY.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtOffsetY.Location = new System.Drawing.Point(336, 12);
            this.txtOffsetY.Name = "txtOffsetY";
            this.txtOffsetY.Size = new System.Drawing.Size(67, 22);
            this.txtOffsetY.TabIndex = 9;
            this.txtOffsetY.Validating += new System.ComponentModel.CancelEventHandler(this.OffsetValidating);
            // 
            // lblYOffset
            // 
            this.lblYOffset.AutoSize = true;
            this.lblYOffset.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblYOffset.Location = new System.Drawing.Point(279, 17);
            this.lblYOffset.Name = "lblYOffset";
            this.lblYOffset.Size = new System.Drawing.Size(56, 12);
            this.lblYOffset.TabIndex = 8;
            this.lblYOffset.Text = "Y Offset：";
            // 
            // grpRelativeSettings
            // 
            this.grpRelativeSettings.Controls.Add(this.dgvRelativeSettings);
            this.grpRelativeSettings.Location = new System.Drawing.Point(16, 92);
            this.grpRelativeSettings.Name = "grpRelativeSettings";
            this.grpRelativeSettings.Size = new System.Drawing.Size(387, 419);
            this.grpRelativeSettings.TabIndex = 16;
            this.grpRelativeSettings.TabStop = false;
            this.grpRelativeSettings.Text = "Relative Settings";
            // 
            // dgvRelativeSettings
            // 
            this.dgvRelativeSettings.AllowUserToResizeColumns = false;
            this.dgvRelativeSettings.AllowUserToResizeRows = false;
            this.dgvRelativeSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRelativeSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Source,
            this.Target});
            this.dgvRelativeSettings.Location = new System.Drawing.Point(16, 21);
            this.dgvRelativeSettings.Name = "dgvRelativeSettings";
            this.dgvRelativeSettings.RowTemplate.Height = 24;
            this.dgvRelativeSettings.Size = new System.Drawing.Size(345, 383);
            this.dgvRelativeSettings.TabIndex = 1;
            // 
            // Source
            // 
            this.Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            // 
            // Target
            // 
            this.Target.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Target.HeaderText = "Target";
            this.Target.Name = "Target";
            // 
            // btnListening
            // 
            this.btnListening.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnListening.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnListening.Location = new System.Drawing.Point(768, 6);
            this.btnListening.Name = "btnListening";
            this.btnListening.Size = new System.Drawing.Size(86, 80);
            this.btnListening.TabIndex = 18;
            this.btnListening.Text = "Start(&L)";
            this.btnListening.UseVisualStyleBackColor = true;
            this.btnListening.Click += new System.EventHandler(this.btnListening_Click);
            // 
            // txtDestPort
            // 
            this.txtDestPort.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDestPort.Location = new System.Drawing.Point(223, 40);
            this.txtDestPort.MaxLength = 5;
            this.txtDestPort.Name = "txtDestPort";
            this.txtDestPort.Size = new System.Drawing.Size(36, 22);
            this.txtDestPort.TabIndex = 7;
            this.txtDestPort.Validating += new System.ComponentModel.CancelEventHandler(this.PortValidating);
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lb2.Location = new System.Drawing.Point(205, 45);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(17, 12);
            this.lb2.TabIndex = 6;
            this.lb2.Text = "：";
            // 
            // txtDestIP
            // 
            this.txtDestIP.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtDestIP.Location = new System.Drawing.Point(81, 40);
            this.txtDestIP.Name = "txtDestIP";
            this.txtDestIP.Size = new System.Drawing.Size(123, 22);
            this.txtDestIP.TabIndex = 5;
            this.txtDestIP.Validating += new System.ComponentModel.CancelEventHandler(this.IpValidating);
            // 
            // lblDestIP
            // 
            this.lblDestIP.AutoSize = true;
            this.lblDestIP.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDestIP.Location = new System.Drawing.Point(30, 45);
            this.lblDestIP.Name = "lblDestIP";
            this.lblDestIP.Size = new System.Drawing.Size(50, 12);
            this.lblDestIP.TabIndex = 4;
            this.lblDestIP.Text = "Dest IP：";
            // 
            // txtSourcePort
            // 
            this.txtSourcePort.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSourcePort.Location = new System.Drawing.Point(223, 12);
            this.txtSourcePort.MaxLength = 5;
            this.txtSourcePort.Name = "txtSourcePort";
            this.txtSourcePort.Size = new System.Drawing.Size(36, 22);
            this.txtSourcePort.TabIndex = 3;
            this.txtSourcePort.Validating += new System.ComponentModel.CancelEventHandler(this.PortValidating);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lb1.Location = new System.Drawing.Point(205, 17);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(17, 12);
            this.lb1.TabIndex = 2;
            this.lb1.Text = "：";
            // 
            // txtSourceIP
            // 
            this.txtSourceIP.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtSourceIP.Location = new System.Drawing.Point(81, 12);
            this.txtSourceIP.Name = "txtSourceIP";
            this.txtSourceIP.Size = new System.Drawing.Size(123, 22);
            this.txtSourceIP.TabIndex = 1;
            this.txtSourceIP.Validating += new System.ComponentModel.CancelEventHandler(this.IpValidating);
            // 
            // lblSourceIP
            // 
            this.lblSourceIP.AutoSize = true;
            this.lblSourceIP.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSourceIP.Location = new System.Drawing.Point(18, 17);
            this.lblSourceIP.Name = "lblSourceIP";
            this.lblSourceIP.Size = new System.Drawing.Size(62, 12);
            this.lblSourceIP.TabIndex = 0;
            this.lblSourceIP.Text = "Source IP：";
            // 
            // stuInfo
            // 
            this.stuInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stulblHardware,
            this.stulblSoftware});
            this.stuInfo.Location = new System.Drawing.Point(0, 521);
            this.stuInfo.Name = "stuInfo";
            this.stuInfo.Size = new System.Drawing.Size(867, 22);
            this.stuInfo.SizingGrip = false;
            this.stuInfo.TabIndex = 20;
            this.stuInfo.Text = "stuInfo";
            // 
            // stulblHardware
            // 
            this.stulblHardware.Name = "stulblHardware";
            this.stulblHardware.Size = new System.Drawing.Size(83, 17);
            this.stulblHardware.Text = "Hardware OK";
            // 
            // stulblSoftware
            // 
            this.stulblSoftware.Name = "stulblSoftware";
            this.stulblSoftware.Size = new System.Drawing.Size(78, 17);
            this.stulblSoftware.Text = "Software OK";
            // 
            // bgWorker
            // 
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 543);
            this.Controls.Add(this.stuInfo);
            this.Controls.Add(this.txtRate);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.btnSending);
            this.Controls.Add(this.lblXUnit);
            this.Controls.Add(this.lblYUnit);
            this.Controls.Add(this.txtOffsetX);
            this.Controls.Add(this.lblXOffset);
            this.Controls.Add(this.txtOffsetY);
            this.Controls.Add(this.lblYOffset);
            this.Controls.Add(this.grpRelativeSettings);
            this.Controls.Add(this.btnListening);
            this.Controls.Add(this.txtDestPort);
            this.Controls.Add(this.lb2);
            this.Controls.Add(this.txtDestIP);
            this.Controls.Add(this.lblDestIP);
            this.Controls.Add(this.txtSourcePort);
            this.Controls.Add(this.lb1);
            this.Controls.Add(this.txtSourceIP);
            this.Controls.Add(this.lblSourceIP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "CCD Data Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpRelativeSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelativeSettings)).EndInit();
            this.stuInfo.ResumeLayout(false);
            this.stuInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnSending;
        private System.Windows.Forms.Label lblXUnit;
        private System.Windows.Forms.Label lblYUnit;
        private System.Windows.Forms.TextBox txtOffsetX;
        private System.Windows.Forms.Label lblXOffset;
        private System.Windows.Forms.TextBox txtOffsetY;
        private System.Windows.Forms.Label lblYOffset;
        private System.Windows.Forms.GroupBox grpRelativeSettings;
        private System.Windows.Forms.Button btnListening;
        private System.Windows.Forms.TextBox txtDestPort;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.TextBox txtDestIP;
        private System.Windows.Forms.Label lblDestIP;
        private System.Windows.Forms.TextBox txtSourcePort;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.TextBox txtSourceIP;
        private System.Windows.Forms.Label lblSourceIP;
        private System.Windows.Forms.StatusStrip stuInfo;
        private System.Windows.Forms.ToolStripStatusLabel stulblHardware;
        private System.Windows.Forms.ToolStripStatusLabel stulblSoftware;
        private System.Windows.Forms.DataGridView dgvRelativeSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.ComponentModel.BackgroundWorker bgWorker;
    }
}

