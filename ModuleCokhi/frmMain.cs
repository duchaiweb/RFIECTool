using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace RFIECTool
{
    public partial class frmMain : Form
    {
        public string[] aPorts = SerialPort.GetPortNames();
        private delegate void preventCrossThreadingString(string x);
        private preventCrossThreadingString updateOutputThread;
        private SerialPort myPort = new SerialPort();
        delegate void ButtonEnableHandler();
        public List<string> dataList = new List<string>();

        private string comName = "";
        public string Serial = "";

        public int iIntervalTimer = 1000;   // 1s
        public static int iTimeoutCOM = 5; // số giây
        public int iCounterTimeout = 0;
        public bool bEnableCouter = false;
        public bool bTimeoutFlag = false;

        System.Timers.Timer myTimer = new System.Timers.Timer();

        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private string bBufferRecv = "";

        private void timer_Tick(object sender, EventArgs e)
        {
            if (bEnableCouter)
            {
                iCounterTimeout++;
                if (iCounterTimeout > iTimeoutCOM)
                {
                    StopCounterTimer();
                    SetEventFlag(true);
                    bTimeoutFlag = true;
                }
            }
        }

        private void initWaitRec()
        {
            bBufferRecv = "";
            StartCounterTimer();
        }

        private void InitTimer_COM()
        {
            myTimer.Enabled = true;
            myTimer.Interval = iIntervalTimer;
            myTimer.Elapsed += new ElapsedEventHandler(timer_Tick);
            myPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            int ibyte = serialPort.BytesToRead;
            byte[] myArrData = new byte[ibyte];
            serialPort.Read(myArrData, 0, ibyte);
            bBufferRecv = MyLib.FormatHexString(bBufferRecv + BitConverter.ToString(myArrData));
            try
            {
                if (bBufferRecv.Length >= Convert.ToInt32(bBufferRecv.Substring(3, 5).Replace(" ",""), 16)*3-1)
                {
                    StopCounterTimer();
                    SetEventFlag(true);
                }
            }
            catch { }
        }

        public void StartCounterTimer()
        {
            iCounterTimeout = 0;
            bEnableCouter = true;
            bTimeoutFlag = false;
        }

        public void StopCounterTimer()
        {
            iCounterTimeout = 0;
            bEnableCouter = false;
        }

        private void SetEventFlag(bool bRecieveFlag)
        {
            if (bRecieveFlag)
            {
                receiveDone.Set();
            }
            else
            {
                receiveDone.Reset();
            }
        }

        private void OpenCOM(int iBaudRate)
        {
            try
            {
                myPort.Close();
                myPort.BaudRate = iBaudRate;
                myPort.Open();
                displayLog("Mở " + comName + " thành công");
            }
            catch
            {
                displayLog("Mở" + comName + " lỗi");
                return;
            }
        }

        private void CloseCOM()
        {
            try
            {
                myPort.Close();
            }
            catch { }
        }

        private string SendCOM(string strData)
        {
            string str = "NACK";

            displayLog("Send: " + MyLib.FormatHexString(strData));
            try
            {
                displayLog("Data send: " + MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(strData.Substring(42, strData.Length - 48)))));
            }
            catch { }

            byte[] bufferOBIS = MyLib.HexStringToArrByte(strData);

            try
            {
                myPort.ReadExisting();
            }
            catch
            {
                displayLog("Xoá buffer COM lỗi");
                return "NACK";
            }

            receiveDone.Reset();

            initWaitRec();

            myPort.Write(bufferOBIS, 0, bufferOBIS.Length);

            receiveDone.WaitOne();

            if (bBufferRecv == "")
            {
                StopCounterTimer();
                SetEventFlag(true);
                displayLog("Hết thời gian chờ");
                return "NACK";
            }

            str = bBufferRecv.Trim('-').Trim().Replace('-', ' ').Replace("  ", " ");

            displayLog("Recv: " + MyLib.FormatHexString(str));
            try
            {
                displayLog("Data recv: " + MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
            }
            catch { }

            checkRecv(MyLib.FormatHexString(str));

            return str;
        }

        private string SendCOMThau(string strData)
        {
            string str = "NACK";

            byte[] bufferOBIS = MyLib.HexStringToArrByte(strData);

            try
            {
                myPort.ReadExisting();
            }
            catch
            {
                displayLog("Xoá buffer COM lỗi");
                return "NACK";
            }

            receiveDone.Reset();

            initWaitRec();

            myPort.Write(bufferOBIS, 0, bufferOBIS.Length);

            receiveDone.WaitOne();

            if (bBufferRecv == "")
            {
                StopCounterTimer();
                SetEventFlag(true);
                return "NACK";
            }

            str = bBufferRecv.Trim('-').Trim().Replace('-', ' ').Replace("  ", " ");

            return str;
        }

        public void checkRecv(string recv)
        {
            string seri = txtSerial.Text.PadLeft(12, '0');
            string ck_seri = seri.Substring(0, 2) + " " + seri.Substring(2, 2) + " " + seri.Substring(4, 2) + " " + seri.Substring(6, 2) + " " + seri.Substring(8, 2) + " " + seri.Substring(10, 2);

            try
            {
                if (recv.Substring(9, 17) != ck_seri)
                {
                    displayLog("Seri sai");
                }
            }
            catch { }

            try
            {
                if (cmbManufacture.Text == "Gelex")
                {
                    if (recv.Substring(27, 2) != "01")
                    {
                        displayLog("Nhà sx sai");
                    }
                }
                else if (cmbManufacture.Text == "Psmart")
                {
                    if (recv.Substring(27, 2) != "02")
                    {
                        displayLog("Nhà sx sai");
                    }
                }
                else if (cmbManufacture.Text == "Huu hong")
                {
                    if (recv.Substring(27, 2) != "03")
                    {
                        displayLog("Nhà sx sai");
                    }
                }
            }
            catch { }

            try
            {
                if (cmbMeterType.Text == "1P1G")
                {
                    if (recv.Substring(30, 2) != "01")
                    {
                        displayLog("Kiểu công tơ sai");
                    }
                }
                else if (cmbMeterType.Text == "3P1G")
                {
                    if (recv.Substring(30, 2) != "02")
                    {
                        displayLog("Kiểu công tơ sai");
                    }
                }
                else if (cmbMeterType.Text == "3P3G TT")
                {
                    if (recv.Substring(30, 2) != "03")
                    {
                        displayLog("Kiểu công tơ sai");
                    }
                }
            }
            catch { }

            try
            {
                if (recv.Substring(33, 2) != "FF")
                {
                    displayLog("Sequence sai");
                }
            }
            catch { }

            try
            {
                if ((recv.Substring(0, recv.Length).Replace(" ", "").Length / 2).ToString("X4") != recv.Substring(3, 5).Replace(" ", ""))
                {
                    displayLog("Length frame sai");
                }
            }
            catch { }
            try
            {
                if ((recv.Substring(42, recv.Length - 48).Replace(" ", "").Length / 2).ToString("X4") != recv.Substring(36, 5).Replace(" ", ""))
                {
                    displayLog("Length data sai");
                }
            }
            catch { }
        }

        public frmMain()
        {
            InitializeComponent();
            updateOutputThread = displayLog;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (aPorts.Length > 0)
            {
                for (int i = 0; i < aPorts.Length; i++)
                {
                    cmbPortList.DisplayMember = "Text";
                    cmbPortList.ValueMember = "Value";
                    cmbPortList.Items.Add(new { Text = aPorts[i], Value = aPorts[i] });
                }
            }

            cmbMeterType.Text = "1P1G";
            cmbManufacture.Text = "Gelex";
            cmbTimeout.Text = "5";

            InitTimer_COM();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            aPorts = SerialPort.GetPortNames();
            cmbPortList.Items.Clear();

            if (aPorts.Length > 0)
            {
                for (int i = 0; i < aPorts.Length; i++)
                {
                    cmbPortList.DisplayMember = "Text";
                    cmbPortList.ValueMember = "Value";
                    cmbPortList.Items.Add(new { Text = aPorts[i], Value = aPorts[i] });
                }
            }
        }

        public bool checkCom()
        {
            if (cmbPortList.Text == string.Empty)
            {
                displayLog("Chưa chọn cổng COM");
                return false;
            }

            comName = cmbPortList.Text;

            if (!myPort.IsOpen)
            {
                myPort.PortName = comName;
            }
            
            myPort.Parity = Parity.None;
            myPort.StopBits = StopBits.One;
            myPort.BaudRate = 9600;
            myPort.DataBits = 8;
            myPort.DtrEnable = true;
            myPort.RtsEnable = true;

            return true;
        }

        public void displayLog(string msg)
        {
            if (rtbOutput.InvokeRequired)
            {
                try
                {
                    rtbOutput.Invoke(updateOutputThread, "---\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" + msg + Environment.NewLine);
                }
                catch { }
            }
            else
            {
                Color textColor = new Color();
                if (msg.ToLower().IndexOf("send") > -1 || msg.ToLower().IndexOf("thành công") > -1)
                {
                    textColor = Color.Blue;
                }
                else
                {
                    textColor = Color.Red;
                }

                rtbOutput.SelectionStart = rtbOutput.TextLength;
                rtbOutput.SelectionLength = 0;

                rtbOutput.SelectionColor = textColor;
                rtbOutput.AppendText("---\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n" + msg + Environment.NewLine);
                rtbOutput.SelectionColor = rtbOutput.ForeColor;

                
                rtbOutput.ScrollToCaret();
                Application.DoEvents();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (radReadOBIS.Checked)
            {
                ReadData(txtOBISData.Text.Trim());
            }
            else if (radReadFromFile.Checked)
            {
                foreach (string s in dataList)
                {
                    ReadData(s);
                }
            }
            string path = Path.Combine(MyLib.GetAppPath(), @"Logs\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, rtbOutput.Text);
            MyLib.NoticeInfo("Hoàn thành!", "Thông tin");
        }

        public void ReadData(string data)
        {
            string sendHex = CreateFrame(data);
            string strData = SendCOM(sendHex);
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Browse txt File",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "Text File|*.txt;",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = openFileDialog1.FileName;
            }

            ReadDataList();
        }

        public void ReadDataList()
        {
            dataList.Clear();
            string strPath = txtPath.Text;

            FileInfo config_f = new FileInfo(strPath);
            foreach (string line in File.ReadAllLines(config_f.FullName))
            {
                if (line != string.Empty)
                {
                    dataList.Add(line);
                }
            }
        }

        public string CreateFrame(string data)
        {
            string result = "";
            string seri = txtSerial.Text.PadLeft(12, '0');
            result += seri.Substring(0, 2) + " " + seri.Substring(2, 2) + " " + seri.Substring(4, 2) + " " + seri.Substring(6, 2) + " " + seri.Substring(8, 2) + " " + seri.Substring(10, 2) + " ";

            if (cmbManufacture.Text == "Gelex")
            {
                result += "01 ";
            }
            else if (cmbManufacture.Text == "Psmart")
            {
                result += "02 ";
            }
            else if (cmbManufacture.Text == "Huu hong")
            {
                result += "03 ";
            }

            if (cmbMeterType.Text == "1P1G")
            {
                result += "01 ";
            }
            else if (cmbMeterType.Text == "3P1G")
            {
                result += "02 ";
            }
            else if (cmbMeterType.Text == "3P3G TT")
            {
                result += "03 ";
            }
            result += "00 "; // sequence

            string hexData = "";
            hexData += MyLib.ASCIIToHexString(data) + " ";   // data
            hexData += "28 29 "; // ()

            result += MyLib.FormatHexString((hexData.Replace(" ", "").Length / 2).ToString("X4")) + " ";  // length data
            result += hexData;
            result = MyLib.FormatHexString((16 + hexData.Replace(" ", "").Length / 2).ToString("X4")) + " " + result;  // length frame
            result = MyLib.FormatHexString(result);
            
            result += MyLib.CRCXor(result) + " ";   // crc
            result += "16";
            result = "68 " + result;

            return MyLib.FormatHexString(result);
        }

        public string CreateFrameThau(string data, string seri, string manu, string metertype)
        {
            string result = "";
            seri = seri.PadLeft(12, '0');
            result += seri.Substring(0, 2) + " " + seri.Substring(2, 2) + " " + seri.Substring(4, 2) + " " + seri.Substring(6, 2) + " " + seri.Substring(8, 2) + " " + seri.Substring(10, 2) + " ";

            result += manu + " ";
            result += metertype + " ";

            result += "00 "; // sequence

            string hexData = "";
            hexData += MyLib.ASCIIToHexString(data) + " ";   // data
            hexData += "28 29 "; // ()

            result += MyLib.FormatHexString((hexData.Replace(" ", "").Length / 2).ToString("X4")) + " ";  // length data
            result += hexData;
            result = MyLib.FormatHexString((16 + hexData.Replace(" ", "").Length / 2).ToString("X4")) + " " + result;  // length frame
            result = MyLib.FormatHexString(result);

            result += MyLib.CRCXor(result) + " ";   // crc
            result += "16";
            result = "68 " + result;

            return MyLib.FormatHexString(result);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn chắc chắn xóa log?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                rtbOutput.Clear();
            }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save";
            saveDialog.Filter = "Txt Files (*.txt)|*.txt";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveDialog.FileName;
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.AppendAllText(file, rtbOutput.Text);
                GC.Collect();
                MyLib.NoticeInfo("Hoàn thành!", "Thông báo");
            }
            GC.Collect();
        }

        private void cmbTimeout_SelectedIndexChanged(object sender, EventArgs e)
        {
            iTimeoutCOM = Convert.ToInt32(cmbTimeout.Text);
        }

        private void btnOpenCOM_Click(object sender, EventArgs e)
        {
            if (!checkCom()) return;

            OpenCOM(9600);
        }

        private void btnCloseCOM_Click(object sender, EventArgs e)
        {
            CloseCOM();
            displayLog("Hoàn thành đóng COM");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCOM();
        }

        private void btnReadThau_Click(object sender, EventArgs e)
        {
            string str = "";

            if (chkSF80C21.Checked && chkSF80C21_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtSF80C21_Seri.Text, "02", "01");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtSF80C21_180.Text = str;
                txtSF80C21_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }
            if (chkDDS26D.Checked && chkDDS26D_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtDDS26D_Seri.Text, "03", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtDDS26D_180.Text = str;
                txtDDS26D_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }

            if (chkSF80C21_2.Checked && chkSF80C21_2_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtSF80C21_2_Seri.Text, "02", "01");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtSF80C21_2_180.Text = str;
                txtSF80C21_2_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }
            if (chkDDS26D_2.Checked && chkDDS26D_2_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtDDS26D_2_Seri.Text, "03", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtDDS26D_2_180.Text = str;
                txtDDS26D_2_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }

            if (chkTF100P31.Checked && chkTF100P31_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtTF100P31_Seri.Text, "02", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100P31_180.Text = str;
                txtTF100P31_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }
            if (chkTF100P31.Checked && chkTF100P31_380.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0", txtTF100P31_Seri.Text, "02", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100P31_380.Text = str;
                txtTF100P31_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }

            if (chkDTS273P1T.Checked && chkDTS273P1T_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtDTS273P1T_Seri.Text, "03", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtDTS273P1T_180.Text = str;
                txtDTS273P1T_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }
            if (chkDTS273P1T.Checked && chkDTS273P1T_380.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0", txtDTS273P1T_Seri.Text, "03", "02");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtDTS273P1T_380.Text = str;
                txtDTS273P1T_Time.Text = "(" + DateTime.Now.ToString("yyMMddHHmm") + ")";
            }
            if (chkTF100m31.Checked && chkTF100m31_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_180.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_181.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_181.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_182.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.2", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_182.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_183.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.3", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_183.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_280.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.0", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_280.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_281.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_281.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_282.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.2", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_282.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_283.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.3", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_283.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_380.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_380.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_480.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.4.8.0", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }
                txtTF100m31_480.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_Time.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("0.0.0.9.4", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustTime(str);
                }
                catch { }
                txtTF100m31_Time.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_Event.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("0.0.C.7.10", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustEvent(str);
                }
                catch { }

                rtbTF100m31_Event.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_1801.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1811.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.1.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_1811.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1821.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.2.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_1821.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1831.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.3.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_1831.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_2801.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2811.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.1.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_2811.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2821.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.2.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_2821.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2831.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.3.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_2831.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_3801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_3801.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_4801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.4.8.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtTF100m31_4801.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_MonthTime.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.0.1.3.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustTime(str);
                }
                catch { }

                txtTF100m31_MonthTime.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1601.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_1601.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1611.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.1.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_1611.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1621.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.2.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_1621.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_1631.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.3.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_1631.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2601.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.0.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_2601.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2611.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.1.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_2611.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2621.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.2.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_2621.Text = str;
            }
            if (chkTF100m31.Checked && chkTF100m31_2631.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.3.1", txtTF100m31_Seri.Text, "02", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtTF100m31_2631.Text = str;
            }

            if (chkDTS273P3T.Checked && chkDTS273P3T_180.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_180.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_181.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_181.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_182.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.2", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_182.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_183.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.3", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_183.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_280.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.0", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_280.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_281.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_281.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_282.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.2", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_282.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_283.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.3", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_283.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_380.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_380.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_480.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.4.8.0", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_480.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_Time.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("0.0.0.9.4", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustTime(str);
                }
                catch { }
                txtDTS273P3T_Time.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_Event.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("0.0.C.7.10", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustEvent(str);
                }
                catch { }

                rtbDTS273P3T_Event.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_1801.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1811.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.1.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_1811.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1821.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.2.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_1821.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1831.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.8.3.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_1831.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_2801.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2811.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.1.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_2811.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2821.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.2.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_2821.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2831.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.8.3.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_2831.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_3801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.3.8.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_3801.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_4801.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.4.8.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                }
                catch { }

                txtDTS273P3T_4801.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_MonthTime.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.0.1.3.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustTime(str);
                }
                catch { }

                txtDTS273P3T_MonthTime.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1601.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_1601.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1611.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.1.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_1611.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1621.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.2.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_1621.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_1631.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.1.6.3.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_1631.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2601.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.0.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_2601.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2611.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.1.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_2611.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2621.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.2.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_2621.Text = str;
            }
            if (chkDTS273P3T.Checked && chkDTS273P3T_2631.Checked)
            {
                str = "";
                try
                {
                    str = ReadDataThau("1.0.2.6.3.1", txtDTS273P3T_Seri.Text, "03", "03");
                    str = GetValue(MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(MyLib.FormatHexString(str.Substring(42, str.Length - 48)))));
                    str = AdjustMD(str);
                    str = AddRemoveRetail(3, str);
                    if (MyLib.countChar('(', str) < 2)
                    {
                        str += "(0000000000)";
                    }
                }
                catch { }

                txtDTS273P3T_2631.Text = str;
            }

            MyLib.NoticeInfo("Hoàn thành!", "Thông tin");
        }

        public string GetValue(string s)
        {
            int idex = s.IndexOf("(");
            return s.Substring(idex, s.Length - idex);
        }

        public string ReadDataThau(string data, string seri, string manu, string metertype)
        {
            string sendHex = CreateFrameThau(data, seri, manu, metertype);
            string strData = SendCOMThau(sendHex);
            return strData;
        }

        public string AddRemoveRetail(byte num, string buf)
        {
            string temp = buf;
            string zero_pad = "";
            int pos1 = temp.IndexOf('.');
            int pos2 = temp.IndexOf('*');
            int pos = (pos2 - pos1);

            if ((num + 1) > pos) // can them 000000
            {
                int num_add = 1 + num - pos;
                for (UInt16 i = 0; i < num_add; i++)
                {
                    zero_pad = zero_pad.Insert(0, "0");
                }
                buf = buf.Insert(pos2, zero_pad);
                // (1234.6*kWh)
            }
            else if ((num + 1) < pos) // can bot byte 0
            {
                int num_add = pos - num - 1;
                buf = temp.Remove(pos1 + num + 1, num_add);
            }
            return buf;
        }

        public string AdjustEvent(string buf)
        {
            // (198)(123456789012)(987654321087)
            int pos = buf.IndexOf(")");
            buf = buf.Remove(0, pos + 1);
            buf = buf.Remove(11, 2);
            buf = buf.Insert(12, "(85)\r\n");
            buf = buf.Remove(buf.Length - 3, 2);
            buf = buf.Insert(buf.Length, "(86)\r\n");
            return buf;
        }

        public string AdjustMD(string buf)
        {
            buf = buf.Remove(buf.Length - 3, 2);
            return buf;
        }

        public string AdjustTime(string buf)
        {
            buf = buf.Remove(buf.Length - 3, 2);
            return buf;
        }
    }
}