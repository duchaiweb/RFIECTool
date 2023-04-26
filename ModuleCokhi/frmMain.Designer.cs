namespace WM03Soft
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
            this.txtSeriModule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            this.btnRead = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPortList = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSeriMeter = new System.Windows.Forms.TextBox();
            this.txtPORT = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpTime = new System.Windows.Forms.DateTimePicker();
            this.radTimePC = new System.Windows.Forms.RadioButton();
            this.radTimeCustom = new System.Windows.Forms.RadioButton();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.txtPeriod = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtK = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtMType = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtActiveTotal = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.chkSeriModule = new System.Windows.Forms.CheckBox();
            this.chkSeriMeter = new System.Windows.Forms.CheckBox();
            this.chkIP = new System.Windows.Forms.CheckBox();
            this.chkPORT = new System.Windows.Forms.CheckBox();
            this.chkMType = new System.Windows.Forms.CheckBox();
            this.chkK = new System.Windows.Forms.CheckBox();
            this.chkPeriod = new System.Windows.Forms.CheckBox();
            this.chkActiveTotal = new System.Windows.Forms.CheckBox();
            this.chkPressureRange = new System.Windows.Forms.CheckBox();
            this.txtPressureRange = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chkRTC = new System.Windows.Forms.CheckBox();
            this.chkVersion = new System.Windows.Forms.CheckBox();
            this.chkClearData = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.chkSendingPeriod = new System.Windows.Forms.CheckBox();
            this.txtSendingPeriod = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.chkSetFlag = new System.Windows.Forms.CheckBox();
            this.label18 = new System.Windows.Forms.Label();
            this.chkClearFlag = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSeriModule
            // 
            this.txtSeriModule.Location = new System.Drawing.Point(114, 63);
            this.txtSeriModule.MaxLength = 1000;
            this.txtSeriModule.Name = "txtSeriModule";
            this.txtSeriModule.Size = new System.Drawing.Size(127, 20);
            this.txtSeriModule.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Seri module";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(362, 16);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(74, 23);
            this.btnWrite.TabIndex = 4;
            this.btnWrite.Text = "Ghi";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(274, 16);
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
            this.label2.Location = new System.Drawing.Point(14, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cổng";
            // 
            // cmbPortList
            // 
            this.cmbPortList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortList.FormattingEnabled = true;
            this.cmbPortList.Location = new System.Drawing.Point(54, 17);
            this.cmbPortList.Name = "cmbPortList";
            this.cmbPortList.Size = new System.Drawing.Size(100, 21);
            this.cmbPortList.TabIndex = 6;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(172, 16);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "Làm mới COM";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Seri đồng hồ";
            // 
            // txtSeriMeter
            // 
            this.txtSeriMeter.Location = new System.Drawing.Point(114, 93);
            this.txtSeriMeter.MaxLength = 1000;
            this.txtSeriMeter.Name = "txtSeriMeter";
            this.txtSeriMeter.Size = new System.Drawing.Size(127, 20);
            this.txtSeriMeter.TabIndex = 2;
            // 
            // txtPORT
            // 
            this.txtPORT.Location = new System.Drawing.Point(107, 46);
            this.txtPORT.MaxLength = 1000;
            this.txtPORT.Name = "txtPORT";
            this.txtPORT.Size = new System.Drawing.Size(127, 20);
            this.txtPORT.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "IP";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(107, 13);
            this.txtIP.MaxLength = 1000;
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(127, 20);
            this.txtIP.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(289, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Thời gian thực";
            // 
            // dtpTime
            // 
            this.dtpTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTime.Location = new System.Drawing.Point(491, 89);
            this.dtpTime.Name = "dtpTime";
            this.dtpTime.Size = new System.Drawing.Size(149, 20);
            this.dtpTime.TabIndex = 14;
            // 
            // radTimePC
            // 
            this.radTimePC.AutoSize = true;
            this.radTimePC.Checked = true;
            this.radTimePC.Location = new System.Drawing.Point(372, 64);
            this.radTimePC.Name = "radTimePC";
            this.radTimePC.Size = new System.Drawing.Size(113, 17);
            this.radTimePC.TabIndex = 15;
            this.radTimePC.TabStop = true;
            this.radTimePC.Text = "Thời gian máy tính";
            this.radTimePC.UseVisualStyleBackColor = true;
            // 
            // radTimeCustom
            // 
            this.radTimeCustom.AutoSize = true;
            this.radTimeCustom.Location = new System.Drawing.Point(372, 90);
            this.radTimeCustom.Name = "radTimeCustom";
            this.radTimeCustom.Size = new System.Drawing.Size(113, 17);
            this.radTimeCustom.TabIndex = 16;
            this.radTimeCustom.Text = "Thời gian tuỳ chọn";
            this.radTimeCustom.UseVisualStyleBackColor = true;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(768, 12);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(281, 424);
            this.rtbOutput.TabIndex = 17;
            this.rtbOutput.Text = "";
            // 
            // txtPeriod
            // 
            this.txtPeriod.Location = new System.Drawing.Point(107, 106);
            this.txtPeriod.MaxLength = 1000;
            this.txtPeriod.Name = "txtPeriod";
            this.txtPeriod.Size = new System.Drawing.Size(127, 20);
            this.txtPeriod.TabIndex = 30;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 110);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Chu kỳ chốt";
            // 
            // txtK
            // 
            this.txtK.Location = new System.Drawing.Point(107, 75);
            this.txtK.MaxLength = 1000;
            this.txtK.Name = "txtK";
            this.txtK.Size = new System.Drawing.Size(127, 20);
            this.txtK.TabIndex = 28;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(30, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(62, 13);
            this.label11.TabIndex = 29;
            this.label11.Text = "Hệ số nhân";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(288, 209);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Phiên bản";
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(353, 205);
            this.txtVersion.MaxLength = 1000;
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(127, 20);
            this.txtVersion.TabIndex = 27;
            // 
            // txtMType
            // 
            this.txtMType.Location = new System.Drawing.Point(107, 11);
            this.txtMType.MaxLength = 1000;
            this.txtMType.Name = "txtMType";
            this.txtMType.Size = new System.Drawing.Size(127, 20);
            this.txtMType.TabIndex = 24;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(30, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "Kiểu module";
            // 
            // txtActiveTotal
            // 
            this.txtActiveTotal.Location = new System.Drawing.Point(353, 126);
            this.txtActiveTotal.MaxLength = 1000;
            this.txtActiveTotal.Name = "txtActiveTotal";
            this.txtActiveTotal.Size = new System.Drawing.Size(127, 20);
            this.txtActiveTotal.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(288, 130);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 33;
            this.label14.Text = "Chỉ số";
            // 
            // chkSeriModule
            // 
            this.chkSeriModule.AutoSize = true;
            this.chkSeriModule.Location = new System.Drawing.Point(14, 66);
            this.chkSeriModule.Name = "chkSeriModule";
            this.chkSeriModule.Size = new System.Drawing.Size(15, 14);
            this.chkSeriModule.TabIndex = 34;
            this.chkSeriModule.UseVisualStyleBackColor = true;
            // 
            // chkSeriMeter
            // 
            this.chkSeriMeter.AutoSize = true;
            this.chkSeriMeter.Location = new System.Drawing.Point(14, 96);
            this.chkSeriMeter.Name = "chkSeriMeter";
            this.chkSeriMeter.Size = new System.Drawing.Size(15, 14);
            this.chkSeriMeter.TabIndex = 35;
            this.chkSeriMeter.UseVisualStyleBackColor = true;
            // 
            // chkIP
            // 
            this.chkIP.AutoSize = true;
            this.chkIP.Location = new System.Drawing.Point(7, 16);
            this.chkIP.Name = "chkIP";
            this.chkIP.Size = new System.Drawing.Size(15, 14);
            this.chkIP.TabIndex = 36;
            this.chkIP.UseVisualStyleBackColor = true;
            // 
            // chkPORT
            // 
            this.chkPORT.AutoSize = true;
            this.chkPORT.Location = new System.Drawing.Point(7, 49);
            this.chkPORT.Name = "chkPORT";
            this.chkPORT.Size = new System.Drawing.Size(15, 14);
            this.chkPORT.TabIndex = 37;
            this.chkPORT.UseVisualStyleBackColor = true;
            // 
            // chkMType
            // 
            this.chkMType.AutoSize = true;
            this.chkMType.Location = new System.Drawing.Point(7, 14);
            this.chkMType.Name = "chkMType";
            this.chkMType.Size = new System.Drawing.Size(15, 14);
            this.chkMType.TabIndex = 41;
            this.chkMType.UseVisualStyleBackColor = true;
            // 
            // chkK
            // 
            this.chkK.AutoSize = true;
            this.chkK.Location = new System.Drawing.Point(7, 78);
            this.chkK.Name = "chkK";
            this.chkK.Size = new System.Drawing.Size(15, 14);
            this.chkK.TabIndex = 42;
            this.chkK.UseVisualStyleBackColor = true;
            // 
            // chkPeriod
            // 
            this.chkPeriod.AutoSize = true;
            this.chkPeriod.Location = new System.Drawing.Point(7, 109);
            this.chkPeriod.Name = "chkPeriod";
            this.chkPeriod.Size = new System.Drawing.Size(15, 14);
            this.chkPeriod.TabIndex = 43;
            this.chkPeriod.UseVisualStyleBackColor = true;
            // 
            // chkActiveTotal
            // 
            this.chkActiveTotal.AutoSize = true;
            this.chkActiveTotal.Location = new System.Drawing.Point(267, 130);
            this.chkActiveTotal.Name = "chkActiveTotal";
            this.chkActiveTotal.Size = new System.Drawing.Size(15, 14);
            this.chkActiveTotal.TabIndex = 44;
            this.chkActiveTotal.UseVisualStyleBackColor = true;
            // 
            // chkPressureRange
            // 
            this.chkPressureRange.AutoSize = true;
            this.chkPressureRange.Location = new System.Drawing.Point(7, 46);
            this.chkPressureRange.Name = "chkPressureRange";
            this.chkPressureRange.Size = new System.Drawing.Size(15, 14);
            this.chkPressureRange.TabIndex = 47;
            this.chkPressureRange.UseVisualStyleBackColor = true;
            // 
            // txtPressureRange
            // 
            this.txtPressureRange.Location = new System.Drawing.Point(107, 43);
            this.txtPressureRange.MaxLength = 1000;
            this.txtPressureRange.Name = "txtPressureRange";
            this.txtPressureRange.Size = new System.Drawing.Size(127, 20);
            this.txtPressureRange.TabIndex = 45;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(30, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 46;
            this.label15.Text = "Dải áp suất";
            // 
            // chkRTC
            // 
            this.chkRTC.AutoSize = true;
            this.chkRTC.Location = new System.Drawing.Point(268, 66);
            this.chkRTC.Name = "chkRTC";
            this.chkRTC.Size = new System.Drawing.Size(15, 14);
            this.chkRTC.TabIndex = 48;
            this.chkRTC.UseVisualStyleBackColor = true;
            // 
            // chkVersion
            // 
            this.chkVersion.AutoSize = true;
            this.chkVersion.Location = new System.Drawing.Point(267, 209);
            this.chkVersion.Name = "chkVersion";
            this.chkVersion.Size = new System.Drawing.Size(15, 14);
            this.chkVersion.TabIndex = 49;
            this.chkVersion.UseVisualStyleBackColor = true;
            // 
            // chkClearData
            // 
            this.chkClearData.AutoSize = true;
            this.chkClearData.Location = new System.Drawing.Point(509, 160);
            this.chkClearData.Name = "chkClearData";
            this.chkClearData.Size = new System.Drawing.Size(15, 14);
            this.chkClearData.TabIndex = 52;
            this.chkClearData.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(530, 160);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 13);
            this.label16.TabIndex = 51;
            this.label16.Text = "Xoá dữ liệu";
            // 
            // chkSendingPeriod
            // 
            this.chkSendingPeriod.AutoSize = true;
            this.chkSendingPeriod.Location = new System.Drawing.Point(267, 163);
            this.chkSendingPeriod.Name = "chkSendingPeriod";
            this.chkSendingPeriod.Size = new System.Drawing.Size(15, 14);
            this.chkSendingPeriod.TabIndex = 55;
            this.chkSendingPeriod.UseVisualStyleBackColor = true;
            // 
            // txtSendingPeriod
            // 
            this.txtSendingPeriod.Location = new System.Drawing.Point(353, 159);
            this.txtSendingPeriod.MaxLength = 1000;
            this.txtSendingPeriod.Name = "txtSendingPeriod";
            this.txtSendingPeriod.Size = new System.Drawing.Size(127, 20);
            this.txtSendingPeriod.TabIndex = 53;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(288, 163);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 13);
            this.label17.TabIndex = 54;
            this.label17.Text = "Chu kỳ gửi";
            // 
            // chkSetFlag
            // 
            this.chkSetFlag.AutoSize = true;
            this.chkSetFlag.Location = new System.Drawing.Point(509, 131);
            this.chkSetFlag.Name = "chkSetFlag";
            this.chkSetFlag.Size = new System.Drawing.Size(15, 14);
            this.chkSetFlag.TabIndex = 57;
            this.chkSetFlag.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(530, 132);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(76, 13);
            this.label18.TabIndex = 56;
            this.label18.Text = "Chốt trạng thái";
            // 
            // chkClearFlag
            // 
            this.chkClearFlag.AutoSize = true;
            this.chkClearFlag.Location = new System.Drawing.Point(616, 132);
            this.chkClearFlag.Name = "chkClearFlag";
            this.chkClearFlag.Size = new System.Drawing.Size(15, 14);
            this.chkClearFlag.TabIndex = 59;
            this.chkClearFlag.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(635, 133);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(73, 13);
            this.label19.TabIndex = 58;
            this.label19.Text = "Xoá trạng thái";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtIP);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPORT);
            this.groupBox1.Controls.Add(this.chkIP);
            this.groupBox1.Controls.Add(this.chkPORT);
            this.groupBox1.Location = new System.Drawing.Point(8, 114);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 75);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtMType);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.chkMType);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtPressureRange);
            this.groupBox2.Controls.Add(this.chkPressureRange);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtK);
            this.groupBox2.Controls.Add(this.txtPeriod);
            this.groupBox2.Controls.Add(this.chkK);
            this.groupBox2.Controls.Add(this.chkPeriod);
            this.groupBox2.Location = new System.Drawing.Point(8, 194);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 138);
            this.groupBox2.TabIndex = 61;
            this.groupBox2.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 448);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkClearFlag);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.chkSetFlag);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.chkSendingPeriod);
            this.Controls.Add(this.txtVersion);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtSendingPeriod);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.chkClearData);
            this.Controls.Add(this.chkVersion);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.chkRTC);
            this.Controls.Add(this.chkActiveTotal);
            this.Controls.Add(this.chkSeriMeter);
            this.Controls.Add(this.chkSeriModule);
            this.Controls.Add(this.txtActiveTotal);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.radTimeCustom);
            this.Controls.Add(this.radTimePC);
            this.Controls.Add(this.dtpTime);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSeriMeter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cmbPortList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSeriModule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WM03Soft";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSeriModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbPortList;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSeriMeter;
        private System.Windows.Forms.TextBox txtPORT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpTime;
        private System.Windows.Forms.RadioButton radTimePC;
        private System.Windows.Forms.RadioButton radTimeCustom;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.TextBox txtPeriod;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtK;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtMType;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtActiveTotal;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox chkSeriModule;
        private System.Windows.Forms.CheckBox chkSeriMeter;
        private System.Windows.Forms.CheckBox chkIP;
        private System.Windows.Forms.CheckBox chkPORT;
        private System.Windows.Forms.CheckBox chkMType;
        private System.Windows.Forms.CheckBox chkK;
        private System.Windows.Forms.CheckBox chkPeriod;
        private System.Windows.Forms.CheckBox chkActiveTotal;
        private System.Windows.Forms.CheckBox chkPressureRange;
        private System.Windows.Forms.TextBox txtPressureRange;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chkRTC;
        private System.Windows.Forms.CheckBox chkVersion;
        private System.Windows.Forms.CheckBox chkClearData;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.CheckBox chkSendingPeriod;
        private System.Windows.Forms.TextBox txtSendingPeriod;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox chkSetFlag;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox chkClearFlag;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

