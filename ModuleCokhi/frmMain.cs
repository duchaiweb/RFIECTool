using System;
using System.Collections.Generic;
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
        public SerialPort port;
        public string[] ports = SerialPort.GetPortNames();
        private delegate void preventCrossThreadingString(string x);
        private preventCrossThreadingString updateOutputThread;
        private SerialPort myPort = new SerialPort();
        delegate void ButtonEnableHandler();
        public List<string> dataList = new List<string>();

        private string comName = "COM11";
        public string Serial = "";

        public int iIntervalTimer = 1000;   // 1s
        public int iTimeoutCOM = 1; // số giây
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
            }
            catch
            {
                displayLog("Không thể mở cổng " + comName);
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

            byte[] bufferOBIS = HexString2Bytes(strData.Replace(" ", ""));

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
                displayLog("Hết thời gian chờ");
                return "NACK";
            }

            str = bBufferRecv.Trim('-').Trim().Replace('-', ' ').Replace("  ", " ");

            displayLog("Recv: " + MyLib.FormatHexString(str));

            return str;
        }

        public string ConvertHex(string hexString)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                string[] a = hexString.Trim().Split(new char[] { ' ' });
                foreach (var h in a)
                {
                    sb.Append((char)int.Parse(h, System.Globalization.NumberStyles.HexNumber));
                }
                return sb.ToString();
            }
            catch { }

            return string.Empty;
        }

        private byte[] HexString2Bytes(string hexStr)
        {
            string hexString = hexStr.ToUpper();
            hexString = hexString.Replace(" ", "");

            if (hexString == null) return null;

            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;

            byte[] bs = new byte[len_half];
            try
            {
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch { }
            return bs;
        }

        public frmMain()
        {
            InitializeComponent();
            updateOutputThread = displayLog;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbPortList.DisplayMember = "Text";
                    cmbPortList.ValueMember = "Value";
                    cmbPortList.Items.Add(new { Text = ports[i], Value = ports[i] });
                }
            }

            cmbMeterType.Text = "CE-18";
        }

        public void port_SendData(string sendForm)
        {
            byte[] bytestosend;
            bytestosend = MyLib.HexStrToByteArr(sendForm);
            port.Write(bytestosend, 0, bytestosend.Length);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ports = SerialPort.GetPortNames();
            cmbPortList.Items.Clear();
            try
            {
                port.Close();
            }
            catch { }
            if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbPortList.DisplayMember = "Text";
                    cmbPortList.ValueMember = "Value";
                    cmbPortList.Items.Add(new { Text = ports[i], Value = ports[i] });
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

            InitTimer_COM();

            comName = cmbPortList.Text;

            try
            {
                myPort.PortName = comName;
                myPort.Parity = Parity.None;
                myPort.StopBits = StopBits.One;
                myPort.BaudRate = 9600;
                myPort.DataBits = 8;
                myPort.DtrEnable = true;
                myPort.RtsEnable = true;
            }
            catch
            {
            }

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
                if (msg.IndexOf("Send") > -1)
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
            if (!checkCom()) return;

            OpenCOM(9600);

            string strData = SendCOM("");

            if (strData != "NACK")
            {
                string[] aBattay = strData.Split(' ');

            }
            else
            {
                displayLog("Bắt tay không thành công");
                CloseCOM();
                return;
            }

            CloseCOM();
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
    }
}