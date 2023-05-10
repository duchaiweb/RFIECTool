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
            bBufferRecv += "-" + BitConverter.ToString(myArrData);
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
    }
}