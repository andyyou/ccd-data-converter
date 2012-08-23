using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace CCDConvert
{
    public partial class FormMain : Form
    {
        #region Local variables

        private static ILog log = LogManager.GetLogger(typeof(Program));
        private bool createNewLogFirst = true;
        private bool needCreateNewLog = false;
        private string jobID = "";
        private string[] outputLog = new string[5];
        private static string xmlPath = @"config\config.xml";
        private Image imgRun = Properties.Resources.Run;
        private Image imgStop = Properties.Resources.Stop;

        private double _offset_default_y, offset_y, offset_x;
        private Dictionary<string, string> dicRelative = new Dictionary<string, string>();

        // TCP Server
        private Socket responseSocket;
        private static ArrayList receiveData = new ArrayList(100);
        private bool clearLog = false;
        private static string tmpUILog = "";
        private static string memLock = "+";

        // TCP Client
        private TcpClient outputClient;
        private bool isConnect = false;
        NetworkStream outputStream;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Initial status image
            tslbHardware.Image = imgStop;
            tslbSoftware.Text = "Software Idle";
            tslbSoftware.Image = imgRun;

            // Load log4net config file
            //XmlConfigurator.Configure(new FileInfo("log4netconfig.xml"));
            Thread networkThread = new Thread(new ThreadStart(updateNetworkStatus));
            networkThread.IsBackground = true;
            networkThread.Start();

            // Load xml config , this is first step.
            bool IsConfigured = getXml(xmlPath, dgvRelativeSettings);

            // ** Notice : Before use [converData] need run again. 本來要將Relative的資料綁入converData 但效能差了2倍 
            updateDictionaryRelative();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            tslbSoftware.Text = "Software Listening";
            this.Listen();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Connect to remote server
            if (!isConnect)
            {
                try
                {
                    outputClient = new TcpClient(txtDestIP.Text, Convert.ToInt32(txtDestPort.Text));
                    btnStop.Text = "Stop(&S)";
                    isConnect = true;
                    tslbSoftware.Text = "Software Listening-Sending";
                    tslbSoftware.Image = imgRun;
                }
                catch (SocketException)
                {
                    outputLog[0] = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    outputLog[1] = "Software Error";
                    outputLog[4] = "N";
                    writeLog();
                    return;
                }
            }
            else
            {
                try
                {
                    outputStream.Close();
                    outputClient.Close();
                    btnStop.Text = "Send(&S)";
                    isConnect = false;
                    tslbSoftware.Text = "Software Listening";
                    tslbSoftware.Image = imgStop;
                }
                catch (NetworkInformationException)
                {
                    outputLog[0] = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    outputLog[1] = "Software Error";
                    outputLog[4] = "N";
                    writeLog();
                    return;
                }
            }
        }

        #region Methods

        private void updateNetworkStatus()
        {
            while (true)
            {
                if (isNetworkConnected())
                {
                    tslbHardware.Text = "Hardware OK";
                    tslbHardware.Image = imgRun;
                }
                else
                {
                    tslbHardware.Text = "Hardware Error";
                    tslbHardware.Image = imgStop;
                    if (jobID != "")
                    {
                        outputLog[0] = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        outputLog[1] = "Hardware Error";
                        outputLog[4] = "N";
                        writeLog();
                    }
                }
            }

        }

        //將DataGridView的對資料轉存成Dictionary提升效能
        private Dictionary<string, string> getRelativeGridViewToDictionary(DataGridView dgv)
        {
            Dictionary<string, string> tmpDict = new Dictionary<string, string>();
           
            lock (dgv)
            {
                dgv.ReadOnly = true;
                for (int i = 0; i < dgv.Rows.Count - 1; i++)
                {
                    if (dgv.Rows[i].Cells[0].Value.ToString().IndexOf(',') > 0)
                    {
                        string[] tmpOriginColumn = dgv.Rows[i].Cells[0].Value.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string st in tmpOriginColumn)
                        {
                            if (!String.IsNullOrEmpty(st) && !String.IsNullOrEmpty(dgv.Rows[i].Cells[1].Value.ToString()))
                            {
                                tmpDict.Add(st, dgv.Rows[i].Cells[1].Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(dgv.Rows[i].Cells[0].Value.ToString()) && !String.IsNullOrEmpty(dgv.Rows[i].Cells[1].Value.ToString()))
                            tmpDict.Add(dgv.Rows[i].Cells[0].Value.ToString(), dgv.Rows[i].Cells[1].Value.ToString());
                    }

                }
                dgv.ReadOnly = false;
            }

            return tmpDict;
        }

        /// <summary>
        /// Check network status
        /// </summary>
        /// <returns>whether network is alive or died</returns>
        private Boolean isNetworkConnected()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface face in interfaces)
            {
                if (face.OperationalStatus == OperationalStatus.Up || face.OperationalStatus == OperationalStatus.Unknown)
                {
                    // Internal network interfaces from VM adapters can still be connected 
                    IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();
                    if (statistics.BytesReceived > 0 && statistics.BytesSent > 0)
                    {
                        // A network interface is up
                        return true;
                    }
                }
            }
            // No Interfaces are up
            return false;
        }

        /// <summary>
        /// Convert data from Source IP to Dest IP 
        /// </summary>
        /// <param name="input">Source Data</param>
        /// <returns></returns>
        private string convertData(string input, double default_offset_y)
        {
            string pattern = @"^DATA*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Dictionary<string, string> dicOutpout = new Dictionary<string, string>();
            string result = "";
            string[] inputArray = input.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string inputData in inputArray)
            {
                dicOutpout.Clear();
                if (regex.IsMatch(inputData))
                {
                    string[] tmp = inputData.Substring(inputData.IndexOf(',') + 1).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string i in tmp)
                    {
                        if (i.IndexOf(',') > 0)
                        {
                            string[] sp = i.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if ((i.IndexOf(',') > 0) && (!dicOutpout.ContainsKey(sp[0].ToString())))
                            {
                                dicOutpout.Add(sp[0].ToString(), sp[1].ToString());
                            }
                        }
                    }

                    // Deal format string
                    //double offset_y = double.TryParse(txtY.Text, out offset_y) ? offset_y : 0;
                    //double offset_x = double.TryParse(txtX.Text, out offset_x) ? offset_x : 0;
                    if ((jobID != dicOutpout["JobID"]) && (createNewLogFirst == false))
                    {
                        needCreateNewLog = true;
                    }
                    jobID = dicOutpout["JobID"];

                    double y = double.Parse(dicOutpout["FlawMD"]) * 1000 + _offset_default_y + offset_y;
                    double x = double.Parse(dicOutpout["FlawCD"]) * 1000 + offset_x;

                    if (dicRelative.ContainsKey(dicOutpout["FlawName"]))
                        result = result + String.Format("{0};{1};{2}", dicRelative[dicOutpout["FlawName"]], y.ToString(), x.ToString()) + "\r";
                    else
                        // 2012/08/23: 若找不到符合的轉換資料，預設之輸出資料第一個欄位設為1
                        result = result + String.Format("{0};{1};{2}", "1", y.ToString(), x.ToString()) + "\r";
                }
                else
                {
                    result += inputData;
                }
            }

            return result;
        }

        private void updateDictionaryRelative()
        {
            // Set relative data and data of convert method.
            dicRelative = getRelativeGridViewToDictionary(dgvRelativeSettings);
            offset_y = double.TryParse(txtY.Text, out offset_y) ? offset_y : 0;
            offset_x = double.TryParse(txtX.Text, out offset_x) ? offset_x : 0;
        }

        private bool getXml(string path, DataGridView dgv)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            XPathDocument document = new XPathDocument(stream);
            XPathNavigator navigator = document.CreateNavigator();
           
            txtY.Text = navigator.SelectSingleNode("//offset[@name='Y']").Value;
            txtX.Text = navigator.SelectSingleNode("//offset[@name='X']").Value;
            _offset_default_y = navigator.SelectSingleNode("//offset[@name='DefaultOffsetY']").ValueAsDouble;

            XPathNodeIterator node = navigator.Select("//relative_table/column");
            while (node.MoveNext())
            {
                dgv.Rows.Add(node.Current.SelectSingleNode("source").Value, node.Current.SelectSingleNode("target").Value);
            }

            #region 原本要使用DataSet, 因為整個Dataset, BindSource 不熟 暫時還沒用.
            //DataSet ds = new DataSet();
            //DataTable dt = new DataTable("RelativeTable");
            //dt.Columns.Add("Source", typeof(string));
            //dt.Columns.Add("Target", typeof(string));
            //dt.Rows.Add("AAAA", "BBB");
            //ds.Tables.Add(dt);
            //ds.ReadXml(path);
            #endregion

            return true;
        }

        private void saveXml(string path, DataGridView dgv)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XPathNavigator navigator = document.CreateNavigator();
            navigator.SelectSingleNode("//offset[@name='Y']").SetValue(txtY.Text);
            navigator.SelectSingleNode("//offset[@name='X']").SetValue(txtX.Text);

            // Remove old relative_table for add new record
            if (navigator.Select("//relative_table/*").Count > 0)
            {
                XPathNavigator first = navigator.SelectSingleNode("//relative_table/*[1]");
                XPathNavigator last = navigator.SelectSingleNode("//relative_table/*[last()]");
                navigator.MoveTo(first);
                navigator.DeleteRange(last);
            }

            lock (dgv)
            {
                for (int i = 0; i < dgv.Rows.Count - 1; i++)
                {
                    string source = dgv.Rows[i].Cells[0].Value.ToString();
                    string target = dgv.Rows[i].Cells[1].Value.ToString();
                    navigator.SelectSingleNode("//relative_table").AppendChildElement(string.Empty, "column", string.Empty, null);
                    // Move to last column element and add source , target value.
                    navigator.SelectSingleNode("//relative_table/column[last()]").AppendChildElement(string.Empty, "source", string.Empty, source.ToUpper());
                    navigator.SelectSingleNode("//relative_table/column[last()]").AppendChildElement(string.Empty, "target", string.Empty, target.ToUpper());

                }
            }
            document.Save(path); 
        }

        public void updateLogText()
        {
            if (clearLog)
            {
                txtLog.Text = "";
                clearLog = false;
            }
            string text = txtLog.Text;
            lock (memLock)
            {
                this.txtLog.Text = tmpUILog;
                tmpUILog = "";
            }
            this.txtLog.AppendText(text);
            this.txtLog.Select(0, 0);
            this.txtLog.ScrollToCaret();
        }

        public delegate void updateLog();

        public void AcceptCallback(IAsyncResult ar)
        {
            bool flag;
            Socket asyncSocket = ((Socket)ar.AsyncState).EndAccept(ar);
            if ((asyncSocket == null) || !asyncSocket.Connected)
            {
                return;
            }
            asyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            asyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 1000);
            asyncSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, 512);
            string str = "";
            string responseData = "";
            string str3 = "";
            while (true)
            {
                if (asyncSocket.Connected)
                {
                    tslbSoftware.Text = "Software Listening";
                    tslbSoftware.Image = imgRun;
                }
                
                outputLog = new string[5];
                flag = false;
                int count = 0;
                str3 = "";
                byte[] bytes = new byte[512];
                try
                {
                    count = asyncSocket.Receive(bytes);
                }
                catch (SocketException exception)
                {
                    if ((exception.ErrorCode == 10060) || (exception.ErrorCode == 10035))
                    {
                        continue;
                    }
                    else
                    {
                        tslbSoftware.Text = "Software Error";
                        tslbSoftware.Image = imgStop;
                    }
                    this.Listen();
                    return;
                }
                if (count <= 0)
                {
                    if (true)
                    {
                        this.Listen();
                        return;
                    }
                }
                else
                {
                    string str4 = Encoding.Unicode.GetString(bytes, 0, count);
                    if (str.Length > 0)
                    {
                        str4 = str + str4;
                        str = "";
                    }
                    if (str4.Length > 0)
                    {
                        int num2 = str4.LastIndexOf("\r");
                        if (num2 == -1)
                        {
                            str = str4;
                            str4 = "";
                        }
                        else if (num2 < (str4.Length - 1))
                        {
                            str = str4.Substring(num2 + 1);
                            str4 = str4.Remove(num2 + 1, (str4.Length - num2) - 1);
                        }
                        foreach (string str7 in str4.Split(new char[] { '\r' }))
                        {
                            if (str7.Length >= 1)
                            {
                                receiveData.Add(str7);
                            }
                        }
                    }
                    while (receiveData.Count > 0)
                    {
                        string str8 = (string)receiveData[0];
                        receiveData.RemoveAt(0);
                        str3 = String.Format("Receive: {0}\r\n", str8);
                        outputLog[0] = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        outputLog[1] = str8;
                        outputLog[4] = "N";
                        if (str8.CompareTo("GetStatus") == 0)
                        {
                            responseData = "Device_OK\r";
                            flag = true;
                        }
                        else if (str8.CompareTo("Rdy4_Xmit?") == 0)
                        {
                            responseData = "Send_All\r";
                            clearLog = true;
                            flag = true;
                        }
                        if (flag)
                        {
                            bytes = Encoding.Unicode.GetBytes(responseData);
                            int responseDataLength = 0;
                            try
                            {
                                responseDataLength = asyncSocket.Send(bytes);
                            }
                            catch (SocketException exception2)
                            {
                                if ((exception2.ErrorCode == 10060) || (exception2.ErrorCode == 10035))
                                {
                                    continue;
                                }
                                this.Listen();
                                break;
                            }
                            if (responseDataLength > 0)
                            {
                                str3 = String.Format("Response: {0}\r\n", responseData.Substring(0, responseData.Length - 1));
                            }
                        }
                        lock (memLock)
                        {
                            tmpUILog = str3;
                        }
                        updateLog method = new updateLog(this.updateLogText);
                        this.txtLog.Invoke(method);

                        if (flag)
                        {
                            continue;
                        }

                        string convertedData = convertData(str8, _offset_default_y);
                        if (createNewLogFirst)
                        {
                            setLogFile();
                            XmlConfigurator.Configure(new FileInfo("log4netconfig.xml"));
                            createNewLogFirst = false;
                        }
                        else if (needCreateNewLog && createNewLogFirst == false)
                        {
                            createNewLogFile();
                            needCreateNewLog = false;
                        }
                        if (isConnect && flag != true)
                        {
                            sendData(convertedData);
                        }

                        string[] splitReceiveData = str8.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] splitConvertedData = convertedData.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < splitReceiveData.Length;i++)
                        {
                            outputLog[1] = splitReceiveData[i];
                            outputLog[2] = splitConvertedData[i];
                            writeLog();
                        }
                    }
                }
            }
        }

        public void Listen()
        {
            this.Shutdown();
            if (responseSocket == null)
            {
                try
                {
                    IPAddress address = IPAddress.Parse(txtSourceIP.Text);
                    int port = Convert.ToInt32(txtSourcePort.Text);
                    IPEndPoint localEP = new IPEndPoint(address, port);
                    responseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LingerOption optionValue = new LingerOption(false, 0);
                    responseSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, optionValue);
                    responseSocket.Bind(localEP);
                    responseSocket.Listen(1);
                    responseSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), responseSocket);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    btnStart.Enabled = true;
                }
            }
        }

        private void Shutdown()
        {
            if (responseSocket != null)
            {
                try
                {
                    responseSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                }
                try
                {
                    responseSocket.Close();
                }
                catch (Exception)
                {
                }
                responseSocket = null;
            }
        }

        private bool TestConnection()
        {
            return false;
        }

        private void sendData(string output)
        {
            try
            {
                outputLog[2] = output;
                outputLog[3] = System.DateTime.Now.ToString("yyyyMMddHHmmssfff");

                outputStream = outputClient.GetStream();

                UnicodeEncoding encoder = new UnicodeEncoding();
                byte[] buffer = encoder.GetBytes(String.Format("{0}",output));

                outputStream.Write(buffer, 0, buffer.Length);
                outputStream.Flush();
                outputLog[4] = "Y";
                tslbSoftware.Text = "Software Listening-Sending";
                tslbSoftware.Image = imgRun;
                tmpUILog = String.Format("Output: {0}\r\n", output);
                updateLog method = new updateLog(this.updateLogText);
                this.txtLog.Invoke(method);
            }
            catch (NetworkInformationException)
            {
                tslbSoftware.Text = "Software Error";
                tslbSoftware.Image = imgStop;
                outputLog[2] = "Software Error";
            }
        }

        private void setLogFile()
        {
            string logName = String.Format("{0}_{1}.csv", jobID, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            log4net.GlobalContext.Properties["LogName"] = logName;
        }

        public bool createNewLogFile()
        {
            string logName = String.Format(@"log\{0}_{1}.csv", jobID, System.DateTime.Now.ToString("yyyyMMddHHmmss"));

            var rootRepository = log4net.LogManager.GetRepository();
            foreach (var appender in rootRepository.GetAppenders())
            {
                if (appender.Name.Equals("fileAppender") && appender is log4net.Appender.FileAppender)
                {
                    var fileAppender = appender as log4net.Appender.FileAppender;
                    fileAppender.File = logName;
                    fileAppender.ActivateOptions();
                    return true;  // Appender found and name changed to NewFilename
                }
            }
            return false; // appender not found
        }

        private void writeLog()
        {
            log.Info(String.Format("{0};{1};{2};{3};{4}", outputLog[0], outputLog[1], outputLog[2], outputLog[3], outputLog[4]));
        }

        #endregion

        #region Event
        private void offset_Validating(object sender, CancelEventArgs e)
        {
            string pattern = @"[0-9]+(?:\.[0-9]*)?";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            TextBox txt = (TextBox)sender;
            if (!regex.IsMatch(txt.Text))
            {
                txt.Text = "";
            }
        }

        private void IP_Validating(object sender, CancelEventArgs e)
        {
            string pattern = @"\b((2[0-5]{2}|1[0-9]{2}|[0-9]{1,2})\.){3}(2[0-5]{2}|1[0-9]{2}|[0-9]{1,2})\b";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            TextBox txt = (TextBox)sender;
            if (!regex.IsMatch(txt.Text))
            {
                txt.Text = "";
            }
        }

        private void Port_Validating(object sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            int port = int.TryParse(txt.Text, out port) ? port : 0;
            if (port < 0 && port > 65536)
            {
                txt.Text = "";
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult drResult = MessageBox.Show("確認是否結束程式?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (drResult == DialogResult.Yes)
            {
                saveXml(xmlPath, dgvRelativeSettings);
            }
            else if (drResult == DialogResult.No)
            {
                e.Cancel = true;
            }

        }
        #endregion
    }
}
