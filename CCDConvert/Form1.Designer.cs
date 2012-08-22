namespace CCDConvert
{
    partial class FormMain
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
            this.lbSourceIP = new System.Windows.Forms.Label();
            this.txtSourceIP = new System.Windows.Forms.TextBox();
            this.lb1 = new System.Windows.Forms.Label();
            this.txtSourcePort = new System.Windows.Forms.TextBox();
            this.lbDestIP = new System.Windows.Forms.Label();
            this.txtDestIP = new System.Windows.Forms.TextBox();
            this.lb2 = new System.Windows.Forms.Label();
            this.txtDestPort = new System.Windows.Forms.TextBox();
            this.stInfo = new System.Windows.Forms.StatusStrip();
            this.tslbHardware = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslbSoftware = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnStart = new System.Windows.Forms.Button();
            this.gpRelativeSettings = new System.Windows.Forms.GroupBox();
            this.dgvRelativeSettings = new System.Windows.Forms.DataGridView();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbYOffset = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.lbXOffset = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.lbUnit_1 = new System.Windows.Forms.Label();
            this.lbUnit_2 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.gpLog = new System.Windows.Forms.GroupBox();
            this.stInfo.SuspendLayout();
            this.gpRelativeSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelativeSettings)).BeginInit();
            this.gpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbSourceIP
            // 
            this.lbSourceIP.AutoSize = true;
            this.lbSourceIP.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSourceIP.Location = new System.Drawing.Point(14, 16);
            this.lbSourceIP.Name = "lbSourceIP";
            this.lbSourceIP.Size = new System.Drawing.Size(70, 15);
            this.lbSourceIP.TabIndex = 0;
            this.lbSourceIP.Text = "Source IP";
            // 
            // txtSourceIP
            // 
            this.txtSourceIP.Location = new System.Drawing.Point(90, 12);
            this.txtSourceIP.Name = "txtSourceIP";
            this.txtSourceIP.Size = new System.Drawing.Size(123, 22);
            this.txtSourceIP.TabIndex = 1;
            this.txtSourceIP.Validating += new System.ComponentModel.CancelEventHandler(this.IP_Validating);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb1.Location = new System.Drawing.Point(219, 16);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(14, 15);
            this.lb1.TabIndex = 2;
            this.lb1.Text = ":";
            // 
            // txtSourcePort
            // 
            this.txtSourcePort.Location = new System.Drawing.Point(239, 12);
            this.txtSourcePort.Name = "txtSourcePort";
            this.txtSourcePort.Size = new System.Drawing.Size(36, 22);
            this.txtSourcePort.TabIndex = 3;
            this.txtSourcePort.Validating += new System.ComponentModel.CancelEventHandler(this.Port_Validating);
            // 
            // lbDestIP
            // 
            this.lbDestIP.AutoSize = true;
            this.lbDestIP.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestIP.Location = new System.Drawing.Point(28, 44);
            this.lbDestIP.Name = "lbDestIP";
            this.lbDestIP.Size = new System.Drawing.Size(56, 15);
            this.lbDestIP.TabIndex = 4;
            this.lbDestIP.Text = "Dest IP";
            // 
            // txtDestIP
            // 
            this.txtDestIP.Location = new System.Drawing.Point(90, 40);
            this.txtDestIP.Name = "txtDestIP";
            this.txtDestIP.Size = new System.Drawing.Size(123, 22);
            this.txtDestIP.TabIndex = 5;
            this.txtDestIP.Validating += new System.ComponentModel.CancelEventHandler(this.IP_Validating);
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb2.Location = new System.Drawing.Point(219, 44);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(14, 15);
            this.lb2.TabIndex = 6;
            this.lb2.Text = ":";
            // 
            // txtDestPort
            // 
            this.txtDestPort.Location = new System.Drawing.Point(239, 40);
            this.txtDestPort.Name = "txtDestPort";
            this.txtDestPort.Size = new System.Drawing.Size(36, 22);
            this.txtDestPort.TabIndex = 7;
            this.txtDestPort.Validating += new System.ComponentModel.CancelEventHandler(this.Port_Validating);
            // 
            // stInfo
            // 
            this.stInfo.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslbHardware,
            this.tslbSoftware});
            this.stInfo.Location = new System.Drawing.Point(0, 543);
            this.stInfo.Name = "stInfo";
            this.stInfo.Size = new System.Drawing.Size(867, 22);
            this.stInfo.TabIndex = 8;
            this.stInfo.Text = "statusStrip1";
            // 
            // tslbHardware
            // 
            this.tslbHardware.Name = "tslbHardware";
            this.tslbHardware.Size = new System.Drawing.Size(83, 17);
            this.tslbHardware.Text = "Hardware OK";
            // 
            // tslbSoftware
            // 
            this.tslbSoftware.Name = "tslbSoftware";
            this.tslbSoftware.Size = new System.Drawing.Size(78, 17);
            this.tslbSoftware.Text = "Software OK";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnStart.Location = new System.Drawing.Point(664, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(191, 80);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Start(&L)";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gpRelativeSettings
            // 
            this.gpRelativeSettings.Controls.Add(this.dgvRelativeSettings);
            this.gpRelativeSettings.Location = new System.Drawing.Point(17, 92);
            this.gpRelativeSettings.Name = "gpRelativeSettings";
            this.gpRelativeSettings.Size = new System.Drawing.Size(387, 419);
            this.gpRelativeSettings.TabIndex = 11;
            this.gpRelativeSettings.TabStop = false;
            this.gpRelativeSettings.Text = "Relative Settings";
            // 
            // dgvRelativeSettings
            // 
            this.dgvRelativeSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRelativeSettings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Source,
            this.Target});
            this.dgvRelativeSettings.Location = new System.Drawing.Point(17, 21);
            this.dgvRelativeSettings.Name = "dgvRelativeSettings";
            this.dgvRelativeSettings.RowTemplate.Height = 24;
            this.dgvRelativeSettings.Size = new System.Drawing.Size(345, 383);
            this.dgvRelativeSettings.TabIndex = 0;
            // 
            // Source
            // 
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.Width = 150;
            // 
            // Target
            // 
            this.Target.HeaderText = "Target";
            this.Target.Name = "Target";
            this.Target.Width = 150;
            // 
            // lbYOffset
            // 
            this.lbYOffset.AutoSize = true;
            this.lbYOffset.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbYOffset.Location = new System.Drawing.Point(294, 16);
            this.lbYOffset.Name = "lbYOffset";
            this.lbYOffset.Size = new System.Drawing.Size(63, 15);
            this.lbYOffset.TabIndex = 12;
            this.lbYOffset.Text = "Y Offset";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(363, 15);
            this.txtY.Name = "txtY";
            this.txtY.Size = new System.Drawing.Size(67, 22);
            this.txtY.TabIndex = 13;
            this.txtY.Validating += new System.ComponentModel.CancelEventHandler(this.offset_Validating);
            // 
            // lbXOffset
            // 
            this.lbXOffset.AutoSize = true;
            this.lbXOffset.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbXOffset.Location = new System.Drawing.Point(294, 44);
            this.lbXOffset.Name = "lbXOffset";
            this.lbXOffset.Size = new System.Drawing.Size(63, 15);
            this.lbXOffset.TabIndex = 14;
            this.lbXOffset.Text = "X Offset";
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(363, 43);
            this.txtX.Name = "txtX";
            this.txtX.Size = new System.Drawing.Size(67, 22);
            this.txtX.TabIndex = 15;
            this.txtX.Validating += new System.ComponentModel.CancelEventHandler(this.offset_Validating);
            // 
            // lbUnit_1
            // 
            this.lbUnit_1.AutoSize = true;
            this.lbUnit_1.Location = new System.Drawing.Point(431, 22);
            this.lbUnit_1.Name = "lbUnit_1";
            this.lbUnit_1.Size = new System.Drawing.Size(23, 12);
            this.lbUnit_1.TabIndex = 16;
            this.lbUnit_1.Text = "mm";
            // 
            // lbUnit_2
            // 
            this.lbUnit_2.AutoSize = true;
            this.lbUnit_2.Location = new System.Drawing.Point(431, 50);
            this.lbUnit_2.Name = "lbUnit_2";
            this.lbUnit_2.Size = new System.Drawing.Size(23, 12);
            this.lbUnit_2.TabIndex = 17;
            this.lbUnit_2.Text = "mm";
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnStop.Location = new System.Drawing.Point(553, 6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(86, 80);
            this.btnStop.TabIndex = 18;
            this.btnStop.Text = "Send(&S)";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(17, 21);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(386, 383);
            this.txtLog.TabIndex = 19;
            // 
            // gpLog
            // 
            this.gpLog.Controls.Add(this.txtLog);
            this.gpLog.Location = new System.Drawing.Point(433, 92);
            this.gpLog.Name = "gpLog";
            this.gpLog.Size = new System.Drawing.Size(422, 419);
            this.gpLog.TabIndex = 21;
            this.gpLog.TabStop = false;
            this.gpLog.Text = "Message";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 565);
            this.Controls.Add(this.gpLog);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lbUnit_2);
            this.Controls.Add(this.lbUnit_1);
            this.Controls.Add(this.txtX);
            this.Controls.Add(this.lbXOffset);
            this.Controls.Add(this.txtY);
            this.Controls.Add(this.lbYOffset);
            this.Controls.Add(this.gpRelativeSettings);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.stInfo);
            this.Controls.Add(this.txtDestPort);
            this.Controls.Add(this.lb2);
            this.Controls.Add(this.txtDestIP);
            this.Controls.Add(this.lbDestIP);
            this.Controls.Add(this.txtSourcePort);
            this.Controls.Add(this.lb1);
            this.Controls.Add(this.txtSourceIP);
            this.Controls.Add(this.lbSourceIP);
            this.Name = "FormMain";
            this.Text = "CCD Data Convert";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.stInfo.ResumeLayout(false);
            this.stInfo.PerformLayout();
            this.gpRelativeSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelativeSettings)).EndInit();
            this.gpLog.ResumeLayout(false);
            this.gpLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbSourceIP;
        private System.Windows.Forms.TextBox txtSourceIP;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.TextBox txtSourcePort;
        private System.Windows.Forms.Label lbDestIP;
        private System.Windows.Forms.TextBox txtDestIP;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.TextBox txtDestPort;
        private System.Windows.Forms.StatusStrip stInfo;
        private System.Windows.Forms.ToolStripStatusLabel tslbHardware;
        private System.Windows.Forms.ToolStripStatusLabel tslbSoftware;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gpRelativeSettings;
        private System.Windows.Forms.DataGridView dgvRelativeSettings;
        private System.Windows.Forms.Label lbYOffset;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label lbXOffset;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label lbUnit_1;
        private System.Windows.Forms.Label lbUnit_2;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.GroupBox gpLog;
    }
}

