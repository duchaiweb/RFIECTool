using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace WM03Soft
{
    public partial class frmMain : Form
    {
        public SerialPort port;
        public string[] ports = SerialPort.GetPortNames();
        private delegate void preventCrossThreadingString(string x);
        private preventCrossThreadingString updateOutputThread;
        private SerialPort myPort = new SerialPort();
        delegate void ButtonEnableHandler();

        public string secretKey = "2B7E151629AED2A6ABF7159909CF";
        private string comName = "COM11";
        private string strF_Battay_WM03 = "1F 2F 3F FF 01";
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

            if (chkSeriModule.Checked)
            {
                txtSeriModule.Text = "";

                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "82 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            enData = recData[10].ToString() + recData[11].ToString() + recData[8].ToString() + recData[9].ToString() + recData[6].ToString() + recData[7].ToString() + recData[4].ToString() + recData[5].ToString() + recData[2].ToString() + recData[3].ToString() + recData[0].ToString() + recData[1].ToString();

                            txtSeriModule.Text = enData.TrimStart('0');
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkSeriMeter.Checked)
            {
                txtSeriMeter.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "87 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sValue = "";

                            sValue = Convert.ToInt64(recData[6].ToString() + recData[7].ToString() + recData[4].ToString() + recData[5].ToString() + recData[2].ToString() + recData[3].ToString() + recData[0].ToString() + recData[1].ToString(), 16).ToString();

                            txtSeriMeter.Text = sValue;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkRTC.Checked)
            {
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "81 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));

                            enData = "20" + MyLib.HexStringToByte(recData[12].ToString() + recData[13].ToString()).ToString().PadLeft(2, '0') + "-" + MyLib.HexStringToByte(recData[10].ToString() + recData[11].ToString()).ToString().PadLeft(2, '0') + "-" + MyLib.HexStringToByte(recData[6].ToString() + recData[7].ToString()).ToString().PadLeft(2, '0') + " " + MyLib.HexStringToByte(recData[4].ToString() + recData[5].ToString()).ToString().PadLeft(2, '0') + ":" + MyLib.HexStringToByte(recData[2].ToString() + recData[3].ToString()).ToString().PadLeft(2, '0') + ":" + MyLib.HexStringToByte(recData[0].ToString() + recData[1].ToString()).ToString().PadLeft(2, '0');

                            dtpTime.Text = enData;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkIP.Checked)
            {
                txtIP.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "83 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sIP = "";
                            string sPort = "";
                            for (int i = 0; i < 80; i += 2)
                            {
                                if ((recData[i].ToString() + recData[i + 1].ToString()) != "00")
                                {
                                    sIP += recData[i].ToString() + recData[i + 1].ToString() + " ";
                                }
                            }
                            sIP = ConvertHex(sIP).Replace("\0", "");
                            sPort = Convert.ToInt64(recData[82].ToString() + recData[83].ToString() + recData[80].ToString() + recData[81].ToString(), 16).ToString();
                            txtIP.Text = sIP;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkPORT.Checked)
            {
                txtPORT.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "83 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sIP = "";
                            string sPort = "";
                            for (int i = 0; i < 80; i += 2)
                            {
                                if ((recData[i].ToString() + recData[i + 1].ToString()) != "00")
                                {
                                    sIP += recData[i].ToString() + recData[i + 1].ToString() + " ";
                                }
                            }
                            sIP = ConvertHex(sIP).Replace("\0", "");
                            sPort = Convert.ToInt64(recData[82].ToString() + recData[83].ToString() + recData[80].ToString() + recData[81].ToString(), 16).ToString();
                            txtPORT.Text = sPort;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkMType.Checked)
            {
                txtMType.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "85 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sMType = "";
                            string sVer = "";
                            string sK = "";
                            string sPeriod = "";

                            sMType = Convert.ToInt32(recData[0].ToString() + recData[1].ToString(), 16).ToString();
                            sK = Convert.ToInt32(recData[2].ToString() + recData[3].ToString(), 16).ToString();
                            sVer = Convert.ToInt32(recData[4].ToString() + recData[5].ToString(), 16).ToString();
                            sPeriod = Convert.ToInt32(recData[8].ToString() + recData[9].ToString() + recData[6].ToString() + recData[7].ToString(), 16).ToString();

                            txtMType.Text = sMType;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkVersion.Checked)
            {
                txtVersion.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "89 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sVer = "";

                            sVer = Convert.ToInt32(recData[0].ToString() + recData[1].ToString(), 16).ToString();

                            txtVersion.Text = sVer;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkK.Checked)
            {
                txtK.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "85 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sK = "";

                            sK = Convert.ToInt32(recData[2].ToString() + recData[3].ToString(), 16).ToString();

                            txtK.Text = sK;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            // Chu kỳ chốt
            if (chkPeriod.Checked)
            {
                txtPeriod.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "85 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sPeriod = "";

                            sPeriod = Convert.ToInt32(recData[8].ToString() + recData[9].ToString() + recData[6].ToString() + recData[7].ToString(), 16).ToString();

                            txtPeriod.Text = sPeriod;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            // Dải áp suất
            if (chkPressureRange.Checked)
            {
                txtPressureRange.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "85 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));

                            string sPressureRange = "";

                            sPressureRange = Convert.ToInt32(recData[16].ToString() + recData[17].ToString() + recData[14].ToString() + recData[15].ToString() + recData[12].ToString() + recData[13].ToString() + recData[10].ToString() + recData[11].ToString(), 16).ToString();

                            txtPressureRange.Text = sPressureRange;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkActiveTotal.Checked)
            {
                txtActiveTotal.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "86 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sChiso = "";

                            sChiso = Convert.ToInt64(recData[6].ToString() + recData[7].ToString() + recData[4].ToString() + recData[5].ToString() + recData[2].ToString() + recData[3].ToString() + recData[0].ToString() + recData[1].ToString(), 16).ToString();

                            txtActiveTotal.Text = sChiso;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            // Chu kỳ gửi
            if (chkSendingPeriod.Checked)
            {
                txtSendingPeriod.Text = "";
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "88 ";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData != "NACK")
                        {
                            string[] aData = strData.Split(' ');
                            string enData = "";
                            for (int i = 5; i < aData.Length - 2; i++)
                            {
                                enData += aData[i];
                            }
                            bData = MyLib.HexStringToArrByte(enData.Replace(" ", ""));
                            string recData = MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_dec(bData, bKey));
                            string sValue = "";

                            sValue = Convert.ToInt64(recData[2].ToString() + recData[3].ToString() + recData[0].ToString() + recData[1].ToString(), 16).ToString();

                            txtSendingPeriod.Text = sValue;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            CloseCOM();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!checkCom()) return;

            OpenCOM(9600);

            if (chkSeriModule.Checked)
            {
                if (txtSeriModule.Text.Length == 0)
                {
                    displayLog("Cần nhập seri module");
                    CloseCOM();
                    return;
                }

                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string sSerialHex = txtSeriModule.Text.PadLeft(12, '0');
                        string sNo = sSerialHex[10].ToString() + sSerialHex[11].ToString() + " " + sSerialHex[8].ToString() + sSerialHex[9].ToString() + " " + sSerialHex[6].ToString() + sSerialHex[7].ToString() + " " + sSerialHex[4].ToString() + sSerialHex[5].ToString() + " " + sSerialHex[2].ToString() + sSerialHex[3].ToString() + " " + sSerialHex[0].ToString() + sSerialHex[1].ToString();
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "02 ";
                        sCommand += sNo;

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài seri module lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkSeriMeter.Checked)
            {
                if (txtSeriMeter.Text.Length == 0)
                {
                    displayLog("Cần nhập seri đồng hồ cơ");
                    CloseCOM();
                    return;
                }
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "09 ";

                        string sMeterID = Convert.ToInt64(txtSeriMeter.Text).ToString("X8");

                        sCommand += sMeterID[6].ToString() + sMeterID[7].ToString() + " " + sMeterID[4].ToString() + sMeterID[5].ToString() + " " + sMeterID[2].ToString() + sMeterID[3].ToString() + " " + sMeterID[0].ToString() + sMeterID[1].ToString();

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài seri đồng hồ cơ lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkRTC.Checked)
            {
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "01 ";

                        string sTime = "";
                        DateTime setTime = new DateTime();
                        if (radTimePC.Checked)
                        {
                            setTime = DateTime.Now;
                        }
                        else
                        {
                            setTime = dtpTime.Value;
                        }
                        sTime += Convert.ToInt32(setTime.ToString("ss")).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.ToString("mm")).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.ToString("HH")).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.ToString("dd")).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.DayOfWeek).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.ToString("MM")).ToString("X2") + " ";
                        sTime += Convert.ToInt32(setTime.ToString("yy")).ToString("X2") + " ";
                        sCommand += sTime;

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài thời gian lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkIP.Checked || chkPORT.Checked)
            {
                if (txtIP.Text.Length == 0 || txtPORT.Text.Length == 0)
                {
                    displayLog("Cần nhập đủ IP và PORT");
                    CloseCOM();
                    return;
                }
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "03 ";

                        string[] temIPHex = BitConverter.ToString(ASCIIEncoding.ASCII.GetBytes(txtIP.Text)).Replace("-", " ").Split(' ');
                        string sIPHex = BitConverter.ToString(ASCIIEncoding.ASCII.GetBytes(txtIP.Text)).Replace("-", " ") + " ";
                        for (int i = 1; i <= (40 - temIPHex.Length); i++)
                        {
                            sIPHex += "00 ";
                        }
                        string sPORTHex = Convert.ToInt64(txtPORT.Text).ToString("X4");

                        string sPort = sPORTHex[2].ToString() + sPORTHex[3].ToString() + " " + sPORTHex[0].ToString() + sPORTHex[1].ToString() + " ";
                        sCommand += sIPHex + sPort;

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài IP, PORT lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkK.Checked || chkPeriod.Checked || chkMType.Checked || chkPressureRange.Checked)
            {
                if (txtK.Text.Length == 0 || txtPeriod.Text.Length == 0 || txtMType.Text.Length == 0 || txtPressureRange.Text.Length == 0)
                {
                    displayLog("Cần nhập đủ hệ số nhân, chu kỳ gửi, kiểu module, phiên bản, dải áp suất");
                    CloseCOM();
                    return;
                }
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "05 ";

                        sCommand += MyLib.NumToNHex(txtMType.Text, 1) + " " + MyLib.NumToNHex(txtK.Text, 1) + " 00 ";
                        sCommand += MyLib.NumToNHex(txtPeriod.Text, 2);
                        sCommand += MyLib.NumToNHex(txtPressureRange.Text, 4);

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài hệ số nhân, chu kỳ gửi, kiểu module, phiên bản, dải áp suất lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkClearData.Checked)
            {
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "08 ";
                        sCommand += "FF";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Xoá dữ liệu lỗi");
                            CloseCOM();
                            return;
                        }
                        else
                        {
                            displayLog("Xoá dữ liệu thành công");
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkSetFlag.Checked)
            {
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "06 ";
                        sCommand += "55";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài cờ trạng thái lỗi");
                            CloseCOM();
                            return;
                        }
                        else
                        {
                            displayLog("Cài cờ trạng thái thành công");
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkClearFlag.Checked)
            {
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "06 ";
                        sCommand += "FF";

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Xoá cờ trạng thái lỗi");
                            CloseCOM();
                            return;
                        }
                        else
                        {
                            displayLog("Xoá cờ trạng thái thành công");
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            if (chkSendingPeriod.Checked)
            {
                if (txtSendingPeriod.Text.Length == 0)
                {
                    displayLog("Cần nhập chu kỳ gửi");
                    CloseCOM();
                    return;
                }
                string strData = SendCOM(strF_Battay_WM03);

                if (strData != "NACK")
                {
                    string[] aBattay = strData.Split(' ');
                    if (aBattay.Length >= 5 && aBattay[4] == "02")
                    {
                        string sDeviceSerialHex = aBattay[5] + aBattay[6] + aBattay[7] + aBattay[8] + aBattay[9] + aBattay[10];
                        string key = secretKey + crc_16(MyLib.HexStringToArrByte(sDeviceSerialHex.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        byte[] bKey = MyLib.HexStringToArrByte(key.Replace(" ", ""));

                        string sSendData = "1F 2F 3F FF 03 ";
                        string sCommand = "0A ";

                        string sSendingPeriod = Convert.ToInt64(txtSendingPeriod.Text).ToString("X4");

                        sCommand += sSendingPeriod[2].ToString() + sSendingPeriod[3].ToString() + " " + sSendingPeriod[0].ToString() + sSendingPeriod[1].ToString();

                        byte[] bData = MyLib.HexStringToArrByte(sCommand.Replace(" ", ""));

                        sSendData += MyLib.ByteArrToHexString(clsAES128_WM03.aes_128_en(bData, bKey));
                        sSendData += crc_16(MyLib.HexStringToArrByte(sSendData.Replace(" ", ""))).ToString("X2").PadLeft(4, '0');
                        sSendData = sSendData.Replace(" ", "");
                        strData = SendCOM(sSendData);
                        if (strData == "NACK")
                        {
                            displayLog("Cài chu kỳ gửi lỗi");
                            CloseCOM();
                            return;
                        }
                    }
                    else
                    {
                        displayLog("Bắt tay lỗi");
                        CloseCOM();
                        return;
                    }
                }
                else
                {
                    displayLog("Bắt tay không thành công");
                    CloseCOM();
                    return;
                }
            }

            CloseCOM();
        }

        public UInt16 crc_16(byte[] data)
        {
            UInt16 crc = 0xFFFF;
            for (UInt16 pos = 0; pos < data.Length; pos++)
            {
                crc ^= (UInt16)data[pos];

                for (int i = 8; i != 0; i--)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else crc >>= 1;
                }
            }
            return crc;
        }

    }
}