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
            this.txtSeriModule = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRead = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPortList = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMeterType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radReadOBIS = new System.Windows.Forms.RadioButton();
            this.radReadFromFile = new System.Windows.Forms.RadioButton();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnBrowser = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSeriModule
            // 
            this.txtSeriModule.Location = new System.Drawing.Point(47, 57);
            this.txtSeriModule.MaxLength = 1000;
            this.txtSeriModule.Name = "txtSeriModule";
            this.txtSeriModule.Size = new System.Drawing.Size(100, 20);
            this.txtSeriModule.TabIndex = 1;
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
            this.btnRead.Location = new System.Drawing.Point(264, 16);
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
            this.label2.Location = new System.Drawing.Point(9, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cổng";
            // 
            // cmbPortList
            // 
            this.cmbPortList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortList.FormattingEnabled = true;
            this.cmbPortList.Location = new System.Drawing.Point(47, 17);
            this.cmbPortList.Name = "cmbPortList";
            this.cmbPortList.Size = new System.Drawing.Size(100, 21);
            this.cmbPortList.TabIndex = 6;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(165, 16);
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
            this.rtbOutput.Location = new System.Drawing.Point(768, 12);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.Size = new System.Drawing.Size(281, 424);
            this.rtbOutput.TabIndex = 17;
            this.rtbOutput.Text = "";
            // 
            // dgvResult
            // 
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Location = new System.Drawing.Point(9, 133);
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.Size = new System.Drawing.Size(753, 303);
            this.dgvResult.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 61);
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
            "CE-18",
            "ME-40",
            "ME-42"});
            this.cmbMeterType.Location = new System.Drawing.Point(235, 57);
            this.cmbMeterType.Name = "cmbMeterType";
            this.cmbMeterType.Size = new System.Drawing.Size(121, 21);
            this.cmbMeterType.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(396, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Dữ liệu";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(451, 57);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(166, 20);
            this.textBox1.TabIndex = 22;
            // 
            // radReadOBIS
            // 
            this.radReadOBIS.AutoSize = true;
            this.radReadOBIS.Checked = true;
            this.radReadOBIS.Location = new System.Drawing.Point(377, 61);
            this.radReadOBIS.Name = "radReadOBIS";
            this.radReadOBIS.Size = new System.Drawing.Size(14, 13);
            this.radReadOBIS.TabIndex = 23;
            this.radReadOBIS.TabStop = true;
            this.radReadOBIS.UseVisualStyleBackColor = true;
            // 
            // radReadFromFile
            // 
            this.radReadFromFile.AutoSize = true;
            this.radReadFromFile.Location = new System.Drawing.Point(377, 96);
            this.radReadFromFile.Name = "radReadFromFile";
            this.radReadFromFile.Size = new System.Drawing.Size(14, 13);
            this.radReadFromFile.TabIndex = 26;
            this.radReadFromFile.UseVisualStyleBackColor = true;
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(451, 92);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(166, 20);
            this.txtPath.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(396, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Từ file";
            // 
            // btnBrowser
            // 
            this.btnBrowser.Location = new System.Drawing.Point(623, 91);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnBrowser.TabIndex = 27;
            this.btnBrowser.Text = "Chọn";
            this.btnBrowser.UseVisualStyleBackColor = true;
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 448);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.radReadFromFile);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.radReadOBIS);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMeterType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvResult);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cmbPortList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSeriModule);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RFIECTool";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSeriModule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbPortList;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMeterType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radReadOBIS;
        private System.Windows.Forms.RadioButton radReadFromFile;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowser;
    }
}

