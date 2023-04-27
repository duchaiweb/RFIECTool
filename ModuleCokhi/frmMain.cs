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
        public static int iTimeoutCOM = 3; // số giây
        public int iCounterTimeout = 0;
        public bool bEnableCouter = false;
        public bool bTimeoutFlag = false;

        System.Timers.Timer myTimer = new System.Timers.Timer();

        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private string bBufferRecv = "";
        public DataTable dtResultFinal = new DataTable();

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
                displayLog("Hết thời gian chờ");
                return "NACK";
            }

            str = bBufferRecv.Trim('-').Trim().Replace('-', ' ').Replace("  ", " ");

            displayLog("Recv: " + MyLib.FormatHexString(str));

            return str;
        }

        public frmMain()
        {
            InitializeComponent();
            updateOutputThread = displayLog;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dtResultFinal.Columns.Add("#");
            dtResultFinal.Columns.Add("Time");
            dtResultFinal.Columns.Add("Send Frame");
            dtResultFinal.Columns.Add("Recv Frame");
            dtResultFinal.Columns.Add("Send Hex");
            dtResultFinal.Columns.Add("Recv Hex");

            if (aPorts.Length > 0)
            {
                for (int i = 0; i < aPorts.Length; i++)
                {
                    cmbPortList.DisplayMember = "Text";
                    cmbPortList.ValueMember = "Value";
                    cmbPortList.Items.Add(new { Text = aPorts[i], Value = aPorts[i] });
                }
            }

            cmbMeterType.Text = "CE-18";
            cmbTimeout.Text = "3";
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
            CloseCOM();

            // Lưu log
            DataTable data = (DataTable)(dgvResult.DataSource);
            string pathExcel = Path.Combine(MyLib.GetAppPath(), @"Logs\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            MyLib.ExportToExcel(data, pathExcel);
            GC.Collect();

            MyLib.NoticeInfo("Hoàn thành!", "Thông tin");
        }

        public void ReadData(string data)
        {
            DataRow drResultFinal = dtResultFinal.NewRow();
            string sendHex = CreateFrame(data);
            string strData = SendCOM(sendHex);

            drResultFinal["#"] = (dtResultFinal.Rows.Count + 1).ToString();
            drResultFinal["Time"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            drResultFinal["Send Frame"] = "<SOH>R1<STX>" + data + "()<ETX><BCC>";
            try
            {

                drResultFinal["Recv Frame"] = "<STX>" + MyLib.ByteArrToASCII(MyLib.HexStringToArrByte(strData.Substring(48, strData.Length - 60))) + "<ETX><BCC>";
            }
            catch { }
            drResultFinal["Send Hex"] = sendHex;
            drResultFinal["Recv Hex"] = strData;

            AddDgvLog(drResultFinal);
        }

        public void AddDgvLog(DataRow dr)
        {
            dtResultFinal.Rows.InsertAt(dr, 0);
            dgvResult.DataSource = dtResultFinal;
            Application.DoEvents();
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
            result += "01 "; // gelex
            if (cmbMeterType.Text == "CE-18")
            {
                result += "01 ";
            }
            else if (cmbMeterType.Text == "ME-40")
            {
                result += "02 ";
            }
            else if (cmbMeterType.Text == "ME-42")
            {
                result += "03 ";
            }
            result += "00 "; // sequence

            string hexData = "";
            hexData += "52 31 "; // r1
            hexData += "02 "; // stx
            hexData += MyLib.ASCIIToHexString(data) + " ";   // data
            hexData += "28 29 "; // ()
            hexData += "03 "; // etx
            hexData += MyLib.CRCXor(hexData) + " ";   // bcc
            hexData = "01 " + hexData; // soh

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
                dgvResult.DataSource = null;
                dtResultFinal.Clear();
            }
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            MyLib.ExportDgvToExcel(dgvResult);
        }

        private void cmbTimeout_SelectedIndexChanged(object sender, EventArgs e)
        {
            iTimeoutCOM = Convert.ToInt32(cmbTimeout.Text);
        }
    }
}