namespace RFIECTool
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRead = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPortList = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOBISData = new System.Windows.Forms.TextBox();
            this.radReadOBIS = new System.Windows.Forms.RadioButton();
            this.radReadFromFile = new System.Windows.Forms.RadioButton();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowser = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.cmbTimeout = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnOpenCOM = new System.Windows.Forms.Button();
            this.btnCloseCOM = new System.Windows.Forms.Button();
            this.cmbManufacture = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSerial
            // 
            this.txtSerial.Location = new System.Drawing.Point(52, 57);
            this.txtSerial.MaxLength = 1000;
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(100, 20);
            this.txtSerial.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Serial";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(429, 14);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(72, 23);
            this.btnRead.TabIndex = 3;
            this.btnRead.Text = "Đọc";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cổng";
            // 
            // cmbPortList
            // 
            this.cmbPortList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortList.FormattingEnabled = true;
            this.cmbPortList.Location = new System.Drawing.Point(52, 15);
            this.cmbPortList.Name = "cmbPortList";
            this.cmbPortList.Size = new System.Drawing.Size(100, 21);
            this.cmbPortList.TabIndex = 6;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(167, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "Làm mới COM";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.button4_Click);
            // 
            // rtbOutput
            // 
            this.rtbOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(12, 121);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(938, 374);
            this.rtbOutput.TabIndex = 17;
            this.rtbOutput.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(174, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Kiểu công tơ";
            // 
            // cmbMeterType
            // 
            this.cmbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMeterType.FormattingEnabled = true;
            this.cmbMeterType.Items.AddRange(new object[] {
            "1P1G",
            "3P1G",
            "3P3G TT"});
            this.cmbMeterType.Location = new System.Drawing.Point(257, 94);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(104, 21);
            this.cmbMeterType.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(391, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Obis";
            // 
            // txtOBISData
            // 
            this.txtOBISData.Location = new System.Drawing.Point(446, 57);
            this.txtOBISData.Name = "txtOBISData";
            this.txtOBISData.Size = new System.Drawing.Size(166, 20);
            this.txtOBISData.TabIndex = 22;
            this.txtOBISData.Text = "1.0.1.8.0";
            // 
            // radReadOBIS
            // 
            this.radReadOBIS.AutoSize = true;
            this.radReadOBIS.Checked = true;
            this.radReadOBIS.Location = new System.Drawing.Point(372, 61);
            this.radReadOBIS.Name = "radReadOBIS";
            this.radReadOBIS.Size = new System.Drawing.Size(14, 13);
            this.radReadOBIS.TabIndex = 23;
            this.radReadOBIS.TabStop = true;
            this.radReadOBIS.UseVisualStyleBackColor = true;
            // 
            // radReadFromFile
            // 
            this.radReadFromFile.AutoSize = true;
            this.radReadFromFile.Location = new System.Drawing.Point(372, 97);
            this.radReadFromFile.Name = "radReadFromFile";
            this.radReadFromFile.Size = new System.Drawing.Size(14, 13);
            this.radReadFromFile.TabIndex = 26;
            this.radReadFromFile.UseVisualStyleBackColor = true;
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(446, 93);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(166, 20);
            this.txtPath.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(391, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Từ file txt";
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(618, 92);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnBrowser.TabIndex = 27;
            this.btnBrowser.Text = "Chọn";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(516, 14);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 28;
            this.btnClearLog.Text = "Xoá Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Location = new System.Drawing.Point(597, 14);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLog.TabIndex = 29;
            this.btnSaveLog.Text = "Lưu Log";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            // 
            // cmbTimeout
            // 
            this.cmbTimeout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimeout.FormattingEnabled = true;
            this.cmbTimeout.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "8"});
            this.cmbTimeout.Location = new System.Drawing.Point(69, 93);
            this.cmbTimeout.Name = "cmbTimeout";
            this.cmbTimeout.Size = new System.Drawing.Size(83, 21);
            this.cmbTimeout.TabIndex = 30;
            this.cmbTimeout.SelectedIndexChanged += new System.EventHandler(this.cmbTimeout_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Timeout(s)";
            // 
            // btnOpenCOM
            // 
            this.btnOpenCOM.Location = new System.Drawing.Point(257, 14);
            this.btnOpenCOM.Name = "btnOpenCOM";
            this.btnOpenCOM.Size = new System.Drawing.Size(75, 23);
            this.btnOpenCOM.TabIndex = 32;
            this.btnOpenCOM.Text = "Mở COM";
            this.btnOpenCOM.UseVisualStyleBackColor = true;
            this.btnOpenCOM.Click += new System.EventHandler(this.btnOpenCOM_Click);
            // 
            // btnCloseCOM
            // 
            this.btnCloseCOM.Location = new System.Drawing.Point(338, 14);
            this.btnCloseCOM.Name = "btnCloseCOM";
            this.btnCloseCOM.Size = new System.Drawing.Size(75, 23);
            this.btnCloseCOM.TabIndex = 33;
            this.btnCloseCOM.Text = "Đóng COM";
            this.btnCloseCOM.UseVisualStyleBackColor = true;
            this.btnCloseCOM.Click += new System.EventHandler(this.btnCloseCOM_Click);
            // 
            // cmbManufacture
            // 
            this.cmbManufacture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbManufacture.FormattingEnabled = true;
            this.cmbManufacture.Items.AddRange(new object[] {
            "Gelex",
            "Psmart",
            "Huu hong"});
            this.cmbManufacture.Location = new System.Drawing.Point(257, 57);
            this.cmbManufacture.Name = "cmbManufacture";
            this.cmbManufacture.Size = new System.Drawing.Size(104, 21);
            this.cmbManufacture.TabIndex = 35;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(174, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 34;
            this.label7.Text = "Nhà sản xuất";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 507);
            this.Controls.Add(this.cmbManufacture);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnCloseCOM);
            this.Controls.Add(this.btnOpenCOM);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbTimeout);
            this.Controls.Add(this.btnSaveLog);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.radReadFromFile);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.radReadOBIS);
            this.Controls.Add(this.txtOBISData);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cmbPortList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSerial);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFIECTool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSerial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbPortList;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMeterType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOBISData;
        private System.Windows.Forms.RadioButton radReadOBIS;
        private System.Windows.Forms.RadioButton radReadFromFile;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowser;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnSaveLog;
        private System.Windows.Forms.ComboBox cmbTimeout;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOpenCOM;
        private System.Windows.Forms.Button btnCloseCOM;
        private System.Windows.Forms.ComboBox cmbManufacture;
        private System.Windows.Forms.Label label7;
    }
}

