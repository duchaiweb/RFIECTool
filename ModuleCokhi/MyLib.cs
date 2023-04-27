using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using Microsoft.Win32;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;

using int8_t = System.SByte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using int32_t = System.Int32;
using uint64_t = System.UInt64;

namespace RFIECTool
{
    class MyLib
    {
        public static Hashtable hGlobalConfig = new Hashtable();

        // Lấy cấu hình từ db từ file
        public static void ReadGlobalConfig()
        {
            string strPath = Path.Combine(GetAppPath(), "config.txt");
            if (!File.Exists(strPath))
            {
                NoticeError("Thiếu file config.txt", "Lỗi");
                Environment.Exit(1);
            }

            int equalIndex;
            FileInfo config_f = new FileInfo(Path.Combine(GetAppPath(), "config.txt"));
            foreach (string line in File.ReadAllLines(config_f.FullName))
            {
                if (line != string.Empty)
                {
                    equalIndex = line.IndexOf("=");
                    hGlobalConfig[line.Substring(0, equalIndex)] = line.Substring(equalIndex + 1, line.Length - equalIndex - 1);
                }
            }
        }

        // Lấy đường dẫn thư mục app
        public static string GetAppPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        // Thông báo dạng thông tin
        public static void NoticeInfo(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Thông báo dạng lỗi
        public static void NoticeError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Thông báo dạng cảnh báo
        public static void NoticeWarning(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Kiểm tra format serial
        public static bool CheckNoFormat(string serial)
        {
            if (serial.Length < 8 || !Int64.TryParse(serial, out Int64 n)) return false;
            if (serial.IndexOf("|") > -1) return false;
            return true;
        }

        // Kiểm tra định dạng dải serial
        public static bool CheckRangeFormat(string sRange)
        {
            try
            {
                string[] aRange = sRange.Split('-');
                if (aRange.Length != 2) return false;
                if (!Int64.TryParse(aRange[0], out Int64 n) || !Int64.TryParse(aRange[1], out Int64 n1)) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Kiểm tra phần mềm sản xuất đang chạy
        public static bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            if (pList.Length > 0) isRunning = true;

            return isRunning;
        }

        // Thêm app vào tự khởi động startup
        public static void AddApplicationToStartup()
        {
            string strAppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            try
            {
                string valueKey = key.GetValue(strAppName).ToString();
                if (valueKey != "\"" + Application.ExecutablePath + "\"")
                {
                    key.SetValue(strAppName, "\"" + Application.ExecutablePath + "\"");
                }

            }
            catch
            {
                key.SetValue(strAppName, "\"" + Application.ExecutablePath + "\"");
            }
        }

        // Xoá các file thư mục Logs
        public static void ClearOldLog()
        {
            try
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(GetAppPath(), @"Logs"));
                FileInfo[] Files = d.GetFiles("*.txt");
                foreach (FileInfo file in Files)
                {
                    var days = (DateTime.Now - file.LastWriteTime).TotalDays;
                    if (days > 30) file.Delete();
                }
            }
            catch { }
        }

        // Xoá các thư mục log quá 30 ngày
        public void ClearOldLogFolder()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(MyLib.GetAppPath(), @"Logs\"));

            if (Directory.Exists(di.FullName))
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    if (DateTime.Compare(dir.LastWriteTime, DateTime.Now.AddDays(-30)) < 0)
                    {
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            file.Delete();
                        }
                        dir.Delete(true);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(di.FullName);
            }
        }

        // Kiểm tra serial trong dải
        public static bool CheckSerialInRange(string strSerial, string strRange)
        {
            if (strRange is null || strRange.Trim().Length == 0)
            {
                return true;
            }
            string[] spRange = strRange.Split('&');
            for (int i = 0; i < spRange.Length; i++)
            {
                string[] arrRange = spRange[i].Split('-');
                if (arrRange.Length == 2)
                {
                    try
                    {
                        Int64 intStart = Convert.ToInt64(arrRange[0]);
                        Int64 intEnd = Convert.ToInt64(arrRange[1]);
                        Int64 intSerial = Convert.ToInt64(strSerial);

                        if (strSerial.Length >= 8 && arrRange[0].Length >= 8 && arrRange[1].Length >= 8)
                        {
                            if (intSerial >= intStart && intSerial <= intEnd && intStart <= intEnd) return true;
                        }
                    }
                    catch { }
                }
            }

            NoticeError(strSerial + " không trong dải quy định " + strRange, "Lỗi");
            return false;
        }

        // Xuất dữ liệu ra file Excel
        public static void ExportToExcel(DataTable tbl, string excelFilePath)
        {
            if (tbl == null || tbl.Columns == null) return;
            XSSFWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet();
            IRow row0 = sheet.CreateRow(0);
            IRow row1 = sheet.CreateRow(0);

            for (int i = 0; i < tbl.Columns.Count; i++)
            {
                row1.CreateCell(i).SetCellValue(tbl.Columns[i].ColumnName);
            }

            int rowIndex = 1;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                IRow newRow = sheet.CreateRow(rowIndex);
                for (int j = 0; j < tbl.Columns.Count; j++)
                {
                    if (Int64.TryParse(tbl.Rows[i][j].ToString(), out Int64 n))
                    {
                        newRow.CreateCell(j).SetCellValue(Convert.ToInt64(tbl.Rows[i][j].ToString()));
                    }
                    else if (Double.TryParse(tbl.Rows[i][j].ToString(), out Double n1))
                    {
                        newRow.CreateCell(j).SetCellValue(Convert.ToDouble(tbl.Rows[i][j].ToString()));
                    }
                    else
                    {
                        newRow.CreateCell(j).SetCellValue(tbl.Rows[i][j].ToString());
                    }
                }
                rowIndex++;
            }

            for (int i = 0; i < 50; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(excelFilePath));
                if (File.Exists(excelFilePath))
                {
                    File.Delete(excelFilePath);
                }
                FileStream fs = new FileStream(excelFilePath, FileMode.CreateNew);
                wb.Write(fs);
                wb.Close();
                fs.Close();
            }
            catch
            {
                wb.Close();
            }
        }

        // Lấy dữ liệu từ file Excel
        public static DataTable ReadFromExcel(string excelFilePath)
        {
            DataTable tbl = new DataTable();

            try
            {
                if (!File.Exists(excelFilePath))
                {
                    return tbl;
                }
                XSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new XSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheetAt(0);
                int lastColIndex = 0;

                for (int i = 0; i < 500; i++)
                {
                    try
                    {
                        if (sheet.GetRow(0).GetCell(i).StringCellValue.ToString().Length > 0)
                        {
                            lastColIndex = i;
                            tbl.Columns.Add(sheet.GetRow(0).GetCell(i).StringCellValue.ToString());

                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                    try
                    {
                        if (sheet.GetRow(0).GetCell(i).NumericCellValue.ToString().Length > 0)
                        {
                            lastColIndex = i;
                            tbl.Columns.Add(sheet.GetRow(0).GetCell(i).NumericCellValue.ToString());

                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                }

                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        if (row > 0)
                        {
                            DataRow dr = tbl.NewRow();
                            for (int i = 0; i <= lastColIndex; i++)
                            {
                                try
                                {
                                    dr[i] = sheet.GetRow(row).GetCell(i).StringCellValue;
                                }
                                catch { }
                                try
                                {
                                    dr[i] = sheet.GetRow(row).GetCell(i).NumericCellValue;
                                }
                                catch { }
                            }

                            tbl.Rows.Add(dr);
                        }
                    }
                }
                return tbl;
            }
            catch (Exception ex)
            {
                MyLib.NoticeError("Có lỗi xảy ra: " + ex.ToString(), "Lỗi");
                return tbl;
            }
        }

        // Kiểm tra là dạng số
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        // Chuyển IP sang mảng byte
        public static byte[] IPStringToBytes(string strIP)
        {
            char[] separator = new char[] { '.' };
            string[] strArray = strIP.Split(separator);
            return new byte[] { byte.Parse(strArray[0].ToString()), byte.Parse(strArray[1].ToString()), byte.Parse(strArray[2].ToString()), byte.Parse(strArray[3].ToString()) };
        }

        // Chuyển mảng byte sang chuỗi hex
        public static string ByteArrToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static void Debug(string text)
        {
            File.AppendAllText("debug.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + text + "\r\n");
        }

        // Xoá các dòng bị trùng của datatable
        public static DataTable GetDistinctSelf(DataTable SourceDt, string filedName)
        {
            for (int i = SourceDt.Rows.Count - 2; i > 0; i--)
            {
                if (SourceDt.Select(string.Format("{0}='{1}'", filedName, SourceDt.Rows[i][filedName])).Length > 1)
                {
                    SourceDt.Rows.RemoveAt(i);
                }
            }
            return SourceDt;
        }

        // Định dạng ký tự gạch dọc phân tách
        public static string FormatSeparate(string s)
        {
            return s.Replace("||", "|").Replace("||", "|").Replace("||", "|").TrimStart('|');
        }

        // Chuyển mảng Hex sang chuỗi Ascii 4F 4B => OK
        public static string ByteArrToASCII(byte[] aBytes)
        {
            try
            {
                string hexString = BitConverter.ToString(aBytes).Replace("-", "");
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;
                }

                return ascii;
            }
            catch { }

            return string.Empty;
        }

        // Chuyển dãy chuỗi hexa đầu vào sang mảng byte
        public static byte[] HexStrToByteArr(string HexString)
        {
            string[] HexArray;
            HexArray = FormatHexString(HexString.Replace("-", " ")).Split(' ');
            int NumberChars = HexArray.Length;
            byte[] bytes = new byte[NumberChars];
            for (int i = 0; i < NumberChars; i++)
            {
                bytes[i] = Convert.ToByte(HexArray[i], 16);
            }
            return bytes;
        }

        // Chuyển chuỗi sang NULL cho sql
        public static string FormatNumNull(string s)
        {
            if (s.Length == 0) return "NULL";
            else return s;
        }

        public static string LastChar(string s)
        {
            if (s.Length > 0)
            {
                return s.Substring(s.Length - 1, 1);
            }
            return "";
        }

        // Lấy số ký tự đầu tiên
        public static string FirstChar(string s, int n)
        {
            if (s.Length >= n)
            {
                return s.Substring(0, n);
            }
            return "";
        }

        // Lấy phần nguyên và phần thập phân ko làm tròn
        public static double Floor(double d, int decimals)
        {
            return Math.Floor(d * Math.Pow(10, decimals)) / Math.Pow(10, decimals);
        }

        // Lấy ký tự thuần của chuỗi
        public static string RawValue(string s)
        {
            return Regex.Replace(s.Trim(), @"\t|\n|\r", "");
        }

        // Xóa thư mục
        public static void DirectoryDelete(string targetDirectory)
        {
            // Xử lý mỗi file khi duyệt được
            string[] fs = Directory.GetFiles(targetDirectory);
            foreach (string fname in fs)
            {
                try
                {
                    if (File.Exists(fname)) File.Delete(fname);
                }
                catch { }
            }

            // Đệ quy thư mục con
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries) DirectoryDelete(subdirectory);

            try
            {
                if (Directory.Exists(targetDirectory)) Directory.Delete(targetDirectory);
            }
            catch { }
        }

        // Unit 12345688 => 0x58 0x61 0xBC 0x00
        public static byte[] Number2Array(UInt64 number, byte size)
        {
            byte index = 0;
            byte[] arr = new byte[size];
            for (byte i = (byte)(0); i < size; i++)
            {
                arr[i] = (byte)(number >> (byte)(index * 8));
                index++;
            }
            return arr;
        }

        // Unit 0x58 0x61 0xBC 0x00 => 12345688
        public static UInt64 Array2Number(byte[] arr, byte size)
        {
            UInt64 number = 0;
            byte index = 0;
            for (byte i = (byte)(0); i < size; i++)
            {
                number |= ((UInt32)(arr[i] << (index * 8)));
                index++;
            }
            return number;
        }

        // Chuyển chuỗi Ascii sang string hex OK => 4F 4B
        public static string ASCIIToHexString(string ascii)
        {
            return BitConverter.ToString(ASCIIEncoding.ASCII.GetBytes(ascii)).Replace("-", " ");
        }

        // Set 0h Datetime
        public static DateTime Set0h(DateTime dt)
        {
            return Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " 00:00:00");
        }

        // Định dạng frame hex
        public static string FormatHexString(string hex)
        {
            string newHex = "";
            hex = hex.Replace(" ", "");
            for (int i = 0; i < hex.Length; i++)
            {
                newHex += hex[i];
                if (i % 2 == 1)
                {
                    newHex += " ";
                }
            }
            return newHex.Trim();
        }

        // Chuyển byte dạng string hex sang byte
        public static byte HexStringToByte(string str)
        {
            if ((str == null) || (str.Length <= 0))
            {
                return 0;
            }
            if (str.Length < 2)
            {
                str = "0" + str;
            }
            if (str.Length > 2)
            {
                str = str.Substring(0, 2);
            }
            byte num = 0;
            for (int i = 0; i < 2; i++)
            {
                int num3 = 0;
                string s = str.Substring(i, 1);
                switch (s)
                {
                    case "A":
                    case "a":
                        num3 = 10;
                        break;

                    case "B":
                    case "b":
                        num3 = 11;
                        break;

                    case "C":
                    case "c":
                        num3 = 12;
                        break;

                    case "D":
                    case "d":
                        num3 = 13;
                        break;

                    case "E":
                    case "e":
                        num3 = 14;
                        break;

                    case "F":
                    case "f":
                        num3 = 15;
                        break;

                    default:
                        num3 = int.Parse(s);
                        break;
                }
                num = (byte)((num * 0x10) + num3);
            }
            return num;
        }

        // Chuyển string hex sang mảng byte
        public static byte[] HexStringToArrByte(string str)
        {
            if (str == null)
            {
                return null;
            }
            str = str.Replace(" ", "");

            byte[] buffer = new byte[str.Length / 2];
            int startIndex = 0;
            for (int i = 0; startIndex < str.Length; i++)
            {
                buffer[i] = MyLib.HexStringToByte(str.Substring(startIndex, 2));
                startIndex += 2;
            }
            return buffer;
        }

        // Bật Bit của Byte
        public static byte SetBit(byte value, byte bit)
        {
            byte result = value;
            result |= (byte)((1 << (byte)bit));
            return result;
        }

        // Xoá Bit của Byte
        public static byte ClearBit(byte value, byte bit)
        {
            byte result = value;
            result &= (byte)(~((1 << (byte)bit)));
            return result;
        }

        // Chuyển số sang hex theo số lượng n byte 60 => 3C 00 (n=2)
        public static string NumToNHex(string value, int n)
        {
            string result = "";
            string hex = Convert.ToInt32(value).ToString("X" + (n * 2).ToString());
            for (int i = 0; i < n * 2; i += 2)
            {
                result = hex[i].ToString() + hex[i + 1].ToString() + result;
            }
            return result;
        }

        // Tính số ngày liên quan
        public static string TimeAgo(DateTime date)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "một giây trước" : ts.Seconds + " giây trước";

            if (delta < 2 * MINUTE)
                return "một phút trước";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " phút trước";

            if (delta < 90 * MINUTE)
                return "một giờ trước";

            if (delta < 24 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 48 * HOUR)
                return "hôm qua";

            if (delta < 30 * DAY)
                return ts.Days + " ngày trước";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "một tháng trước" : months + " tháng trước";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "một năm trước" : years + " năm trước";
            }
        }

        // Cắt chuỗi không cắt từ thêm ...
        public static string TruncateAtWord(string input, int length, bool appendDots = false)
        {
            if (input == null || input.Length < length)
                return input;
            var iNextSpace = input.LastIndexOf(" ", length, StringComparison.Ordinal);
            var trimmedInput = string.Format("{0}", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());

            if (appendDots)
            {
                return trimmedInput + "...";
            }
            return trimmedInput;
        }

        // Chuyển chuỗi số thành chuỗi có số thập phân '50.00' thành 50, thường dùng do kiểu decimal của sql server
        public static string Str2Dub(string num, int digit)
        {
            try
            {
                double dNum = Convert.ToDouble(num);
                return dNum.ToString("f" + digit.ToString());
            }
            catch
            {
                return num;
            }
        }

        // Xuất kết quả Datagridview ra file Excel
        public static void ExportDgvToExcel(DataGridView dgvResult)
        {
            DataTable data = (DataTable)(dgvResult.DataSource);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save";
            saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string file = saveDialog.FileName;
                ExportToExcel(data, file);
                GC.Collect();
                NoticeInfo("Hoàn thành!", "Thông báo");
            }
            GC.Collect();
        }

        // Tính crc theo phép XOR
        public static string CRCXor(string hex)
        {
            hex = FormatHexString(hex);
            string[] arrTempString = hex.ToUpper().Split(' ');
            if (arrTempString.Length < 2) return "";
            byte crc = (byte)(Convert.ToByte(arrTempString[0], 16) ^ Convert.ToByte(arrTempString[1], 16));
            for (int i = 2; i < arrTempString.Length; i++)
            {
                crc = (byte)(crc ^ Convert.ToByte(arrTempString[i], 16));
            }
            return crc.ToString("X2");
        }
    }
}
