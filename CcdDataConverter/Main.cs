using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CcdDataConverter.Model;
using CcdDataConverter.Helper;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;
using System.Collections;

namespace CcdDataConverter
{
    public partial class Main : Form
    {
        #region Local variables

        // store the image that used by status bar
        private Image _imgRun = Properties.Resources.Run;
        private Image _imgStop = Properties.Resources.Stop;

        // store log data
        private LogFormat _outputLog = new LogFormat();
        private bool _clearLog = false;
        private bool _createNewLog = false;
        private string _jobId = "";
        private string _fileName = "";
        
        // store setting data
        private double _defaultOffsetY, _offsetY, _offsetX, _rate;
        private Dictionary<string, string> _dtbRelativeData = new Dictionary<string, string>();
        
        // Log flag(false: Normal, true: Error)
        private bool _isHardwareError = false;
        private bool _isSoftwareError = false;

        // TCP Server
        private Socket _responseSocket;
        private static ArrayList _receiveData = new ArrayList(100);
        private static string _tmpUILog = "";
        private static string _memLock = "+";

        // TCP Client
        private TcpClient _tcpClient;
        NetworkStream _tcpOutputStream;
        private bool _clientConnected = false;

        #endregion

        public Main()
        {
            InitializeComponent();
        }

        #region Action Methods

        private void Main_Load(object sender, EventArgs e)
        {
            // Load config
            LoadConfig();

            // Initial UI control
            stulblSoftware.Text = "Software Idle";
            stulblSoftware.Image = _imgRun;
            btnSending.Enabled = false;

            // Check Network Availability
            Thread networkThread = new Thread(new ThreadStart(UpdateHardwareStatus));
            networkThread.IsBackground = true;
            networkThread.Start();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult drResult = MessageBox.Show("確認是否結束程式?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (drResult == DialogResult.Yes)
            {
                // Save Config
                if (!SaveConfig())
                {
                    e.Cancel = true;
                    MessageBox.Show("設定資料有誤，請修改");
                }
            }
            else if (drResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnListening_Click(object sender, EventArgs e)
        {
            btnListening.Enabled = false;
            btnSending.Enabled = true;
            stulblSoftware.Text = "Software Listening";
            txtSourceIP.Enabled = false;
            txtSourcePort.Enabled = false;
            this.Listen();
        }

        private void btnSending_Click(object sender, EventArgs e)
        {
            if (SaveConfig())
            {
                LoadConfig();
            }
            
            // Connect to remote server
            if (!_clientConnected)
            {
                try
                {
                    _tcpClient = new TcpClient(txtDestIP.Text, Convert.ToInt32(txtDestPort.Text));
                    btnSending.Text = "Stop(&S)";
                    txtDestIP.Enabled = false;
                    txtDestPort.Enabled = false;
                    txtOffsetY.Enabled = false;
                    txtOffsetX.Enabled = false;
                    txtRate.Enabled = false;
                    _clientConnected = true;
                    stulblSoftware.Text = "Software Listening-Sending";
                    stulblSoftware.Image = _imgRun;
                    _isSoftwareError = false;
                    dgvRelativeSettings.ReadOnly = true;
                }
                catch (SocketException ex)
                {
                    if (_isSoftwareError != true)
                    {
                        if (_fileName == "")
                        {
                            _fileName = String.Format("{0}_{1}", _jobId, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        _outputLog.InputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        _outputLog.InputData = String.Format("Software Error,{0},,,", ex.Message);
                        _outputLog.OutputData = ",,";
                        WriteHelper.Log(_outputLog, _fileName);
                        _isSoftwareError = true;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                    return;
                }
            }
            else // Disonnect from remote server
            {
                try
                {
                    if (_tcpOutputStream != null)
                    {
                        _tcpOutputStream.Close();
                    }
                    if (_tcpClient != null)
                    {
                        _tcpClient.Close();
                    }
                    btnSending.Text = "Send(&S)";
                    txtDestIP.Enabled = true;
                    txtDestPort.Enabled = true;
                    txtOffsetY.Enabled = true;
                    txtOffsetX.Enabled = true;
                    txtRate.Enabled = true;
                    _clientConnected = false;
                    stulblSoftware.Text = "Software Listening";
                    stulblSoftware.Image = _imgRun;
                    _isSoftwareError = false;
                    dgvRelativeSettings.ReadOnly = false;
                }
                catch (NetworkInformationException ex)
                {
                    if (_isSoftwareError != true)
                    {
                        if (_fileName == "")
                        {
                            _fileName = String.Format("{0}_{1}", _jobId, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        _outputLog.InputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        _outputLog.InputData = String.Format("Software Error,{0},,,", ex.Message);
                        _outputLog.OutputData = ",,";
                        WriteHelper.Log(_outputLog, _fileName);
                        _isSoftwareError = true;
                    }
                    return;
                }
            }
        }

        #endregion

        #region R Method

        // Validate offset and rate field data
        private void OffsetValidating(object sender, CancelEventArgs e)
        {
            //string pattern = @"^[\d]+(\.[\d]+)?$";
            //Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            //TextBox txt = (TextBox)sender;
            //if (!regex.IsMatch(txt.Text))
            //{
            //    txt.Text = "";
            //}
            TextBox txt = (TextBox)sender;
            try
            {
                Convert.ToDouble(txt.Text);
            }
            catch
            {
                txt.Text = "";
                txt.Focus();
            }
        }

        // Validate IP field data
        private void IpValidating(object sender, CancelEventArgs e)
        {
            //string pattern = @"\b((2[0-5]{2}|1[0-9]{2}|[0-9]{1,2})\.){3}(2[0-5]{2}|1[0-9]{2}|[0-9]{1,2})\b";
            string pattern = @"^\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            TextBox txt = (TextBox)sender;
            if (!regex.IsMatch(txt.Text))
            {
                txt.Text = "";
                txt.Focus();
            }
        }

        // Validate port field data
        private void PortValidating(object sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            int port = int.TryParse(txt.Text, out port) ? port : 0;
            if (port < 1 || port > 65535)
            {
                txt.Text = "";
                txt.Focus();
            }
        }

        // Load config from xml
        private void LoadConfig()
        {
            ConfigHelper ch = new ConfigHelper();
            //_dtbRelativeData = ch.GetRelativeData();
            dgvRelativeSettings.Rows.Clear();
            ch.GetRelativeData(dgvRelativeSettings);
            _dtbRelativeData = getRelativeGridViewToDictionary(dgvRelativeSettings);
            _offsetY = ch.GetOffsetY();
            _offsetX = ch.GetOffsetX();
            _defaultOffsetY = ch.GetDefaultOffsetY();
            _rate = ch.GetRate();
            // Update UI
            txtOffsetY.Text = _offsetY.ToString();
            txtOffsetX.Text = _offsetX.ToString();
            txtRate.Text = _rate.ToString();
            txtSourceIP.Text = ch.GetSourceIP();
            txtSourcePort.Text = ch.GetSourcePort();
            txtDestIP.Text = ch.GetDestIP();
            txtDestPort.Text = ch.GetDestPort();
        }

        // Save config to xml
        private bool SaveConfig()
        {
            bool result = false;
            result = Double.TryParse(txtOffsetY.Text, out _offsetY);
            result &= Double.TryParse(txtOffsetX.Text, out _offsetX);
            result &= Double.TryParse(txtRate.Text, out _rate);
            if (result)
            {
                ConfigHelper ch = new ConfigHelper();
                ch.SaveRelativeData(dgvRelativeSettings);
                ch.SaveOffsetY(_offsetY.ToString());
                ch.SaveOffsetX(_offsetX.ToString());
                ch.SaveRate(_rate.ToString());
                ch.SaveSourceIP(txtSourceIP.Text);
                ch.SaveSourcePort(txtSourcePort.Text);
                ch.SaveDestIP(txtDestIP.Text);
                ch.SaveDestPort(txtDestPort.Text);

                return true;
            }
            return false;
        }

        // Check network interface status
        private bool CheckNetworkAvailability()
        {
            IEnumerable<NetworkInterface> adapters =
                from interfaces in NetworkInterface.GetAllNetworkInterfaces()
                where
                    interfaces.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    interfaces.OperationalStatus == OperationalStatus.Down
                select interfaces;
            if (adapters.Count() > 0)
            {
                stulblHardware.Text = "Hardware Error";
                stulblHardware.Image = _imgStop;
                return false;
            }
            else
            {
                stulblHardware.Text = "Hardware OK";
                stulblHardware.Image = _imgRun;
                return true;
            }
        }

        // Update hardware status
        private void UpdateHardwareStatus()
        {
            while (true)
            {
                if (CheckNetworkAvailability())
                {
                    stulblHardware.Text = "Hardware OK";
                    stulblHardware.Image = _imgRun;
                    _isHardwareError = false;
                }
                else
                {
                    stulblHardware.Text = "Hardware Error";
                    stulblHardware.Image = _imgStop;
                    if (_isHardwareError != true)
                    {
                        if (_fileName == "")
                        {
                            _fileName = String.Format("{0}_{1}", _jobId, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                        _outputLog.InputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        _outputLog.InputData = "Hardware Error,,,,";
                        _outputLog.OutputData = ",,";
                        WriteHelper.Log(_outputLog, _fileName);
                        _isHardwareError = true;
                    }
                }
                Thread.Sleep(100);
            }
        }

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
                                tmpDict.Add(st.ToUpper(), dgv.Rows[i].Cells[1].Value.ToString().ToUpper());
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(dgv.Rows[i].Cells[0].Value.ToString()) && !String.IsNullOrEmpty(dgv.Rows[i].Cells[1].Value.ToString()))
                            tmpDict.Add(dgv.Rows[i].Cells[0].Value.ToString().ToUpper(), dgv.Rows[i].Cells[1].Value.ToString().ToUpper());
                    }

                }
                dgv.ReadOnly = false;
            }

            return tmpDict;
        }

        #endregion

        #region Testing

        public void Listen()
        {
            this.Shutdown();
            if (_responseSocket == null)
            {
                try
                {
                    IPAddress address = IPAddress.Parse(txtSourceIP.Text);
                    int port = Convert.ToInt32(txtSourcePort.Text);
                    IPEndPoint localEP = new IPEndPoint(address, port);
                    _responseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    LingerOption optionValue = new LingerOption(false, 0);
                    _responseSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, optionValue);
                    _responseSocket.Bind(localEP);
                    _responseSocket.Listen(1);
                    _responseSocket.BeginAccept(new AsyncCallback(this.AcceptCallback), _responseSocket);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    btnListening.Enabled = true;
                    btnSending.Enabled = false;
                    txtSourceIP.Enabled = true;
                    txtSourcePort.Enabled = true;
                    stulblSoftware.Text = "Software Idle";
                }
            }
        }

        private void Shutdown()
        {
            if (_responseSocket != null)
            {
                try
                {
                    _responseSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception) { }

                try
                {
                    _responseSocket.Close();
                }
                catch (Exception) { }
                _responseSocket = null;
            }
        }

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
                    if (_clientConnected)
                    {
                        stulblSoftware.Text = "Software Listening-Sending";
                    }
                    else
                    {
                        stulblSoftware.Text = "Software Listening";
                    }
                    stulblSoftware.Image = _imgRun;
                }

                _outputLog = new LogFormat();
                flag = false;
                int count = 0;
                str3 = "";
                byte[] bytes = new byte[512];
                try
                {
                    count = asyncSocket.Receive(bytes);
                }
                catch (SocketException ex)
                {
                    if ((ex.ErrorCode == 10060) || (ex.ErrorCode == 10035))
                    {
                        _isSoftwareError = false;
                        continue;
                    }
                    //else // 判斷 Client 是否斷線
                    //{
                    //    if (_isSoftwareError != true)
                    //    {
                    //        if (_fileName == "")
                    //        {
                    //            _fileName = String.Format("{0}_{1}", _jobId, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                    //        }
                    //        stulblSoftware.Text = "Software Error";
                    //        stulblSoftware.Image = _imgStop;
                    //        _outputLog.InputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    //        _outputLog.InputData = String.Format("Software Error,{0},,,", ex.Message);
                    //        _outputLog.OutputData = ",,";
                    //        WriteHelper.Log(_outputLog, _fileName);
                    //        _isSoftwareError = true;
                    //    }
                    //}
                    this.Listen();
                    return;
                }
                if (count <= 0)
                {
                    //stulblSoftware.Text = "Software Error";
                    //stulblSoftware.Image = _imgStop;
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
                                _receiveData.Add(str7);
                            }
                        }
                    }
                    while (_receiveData.Count > 0)
                    {
                        string str8 = (string)_receiveData[0];
                        _receiveData.RemoveAt(0);
                        str3 = String.Format("Receive: {0}", str8);
                        _outputLog.InputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        if (str8.CompareTo("GetStatus") == 0)
                        {
                            responseData = "Device_OK\r";
                            flag = true;
                        }
                        else if (str8.CompareTo("Rdy4_Xmit?") == 0)
                        {
                            responseData = "Send_New\r";
                            _clearLog = true;
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
                                str3 = String.Format("Response: {0}", responseData.Substring(0, responseData.Length - 1));
                            }
                        }
                        lock (_memLock)
                        {
                            _tmpUILog = str3;
                        }
                        updateLog method = new updateLog(this.UpdateLog);
                        this.txtLog.Invoke(method);

                        if (flag)
                        {
                            break;
                        }

                        string convertedData = ConvertData(str8);

                        if (_clientConnected && flag != true)
                        {
                            TransmitData(convertedData);
                        }

                        if (_createNewLog)
                        {
                            _fileName = String.Format("{0}_{1}", _jobId, System.DateTime.Now.ToString("yyyyMMddHHmmss"));
                            _createNewLog = false;
                        }

                        string[] splitReceiveData = _outputLog.InputData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] splitConvertedData = convertedData.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < splitReceiveData.Length; i++)
                        {
                            _outputLog.InputData = splitReceiveData[i];
                            //if (_clientConnected && flag != true)
                            //{
                                _outputLog.OutputData = splitConvertedData[i].Replace(";", ",");
                            //}
                            WriteHelper.Log(_outputLog, _fileName);
                        }
                    }
                }
            }
        }

        #region 跨執行緒更新 Log

        public delegate void updateLog();

        public void UpdateLog()
        {
            if (_clearLog)
            {
                txtLog.Text = "";
                _clearLog = false;
            }
            string message = txtLog.Text;
            lock (_memLock)
            {
                this.txtLog.Text = String.Format("{0}{1}", _tmpUILog.Trim(), Environment.NewLine);
                _tmpUILog = "";
            }
            this.txtLog.AppendText(message);
            this.txtLog.Select(0, 0);
            this.txtLog.ScrollToCaret();
        }

        #endregion

        private string ConvertData(string input)
        {
            string pattern = @"^DATA*";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Dictionary<string, string> dicOutpout = new Dictionary<string, string>();
            string result = "";
            string[] inputArray = input.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
            _outputLog.InputData = "";
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

                    if (!dicOutpout.ContainsKey("JobID"))
                    {
                        dicOutpout.Add("JobID", "");
                    }

                    // Deal format string
                    if ((_jobId != dicOutpout["JobID"]))
                    {
                        _createNewLog = true;
                        _jobId = dicOutpout["JobID"];
                    }

                    double y = (double.Parse(dicOutpout["FlawMD"]) * 1000 + _defaultOffsetY + _offsetY) * _rate;
                    double x = double.Parse(dicOutpout["FlawCD"]) * 1000 + _offsetX;

                    if (_dtbRelativeData.ContainsKey(dicOutpout["FlawName"].ToUpper()))
                        result += String.Format("{0};{1};{2}", _dtbRelativeData[dicOutpout["FlawName"].ToUpper()], y.ToString(), x.ToString()) + "\r\n";
                    else
                        // 2012/08/23: 若找不到符合的轉換資料，預設之輸出資料第一個欄位設為1
                        result += String.Format("{0};{1};{2}", "1", y.ToString(), x.ToString()) + "\r\n";

                    // 組合輸出 Log 格式
                    _outputLog.InputData = String.Format("{0}{1},{2},{3},{4},{5};", _outputLog.InputData, dicOutpout["FlawID"], dicOutpout["FlawName"], dicOutpout["FlawMD"], dicOutpout["FlawCD"], dicOutpout["JobID"]);
                }
                else
                {
                    result += inputData;
                }
            }

            return result;
        }

        // Send out converted data to remote server
        private void TransmitData(string data)
        {
            try
            {
                _outputLog.OutputDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

                _tcpOutputStream = _tcpClient.GetStream();

                UnicodeEncoding encoder = new UnicodeEncoding();
                byte[] tmp = encoder.GetBytes(String.Format("{0}", data));
                byte[] buffer = new byte[tmp.Length / 2];
                for (int i = 0, j = 0; i < tmp.Length; i += 2, j++)
                {
                    buffer[j] = tmp[i];
                }

                _tcpOutputStream.Write(buffer, 0, buffer.Length);
                _tcpOutputStream.Flush();
                _outputLog.Status = "Y";
                stulblSoftware.Text = "Software Listening-Sending";
                stulblSoftware.Image = _imgRun;
                _tmpUILog = String.Format("Transmit: {0}", data);
                _isSoftwareError = false;
                updateLog method = new updateLog(this.UpdateLog);
                this.txtLog.Invoke(method);
            }
            catch (Exception ex)
            {
                if (_isSoftwareError != true)
                {
                    bgWorker.RunWorkerAsync();
                }
            }
        }

        #endregion

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_tcpOutputStream != null)
            {
                _tcpOutputStream.Close();
            }
            if (_tcpClient != null)
            {
                _tcpClient.Close();
            }
            btnSending.Text = "Send(&S)";
            txtDestIP.Enabled = true;
            txtDestPort.Enabled = true;
            txtOffsetY.Enabled = true;
            txtOffsetX.Enabled = true;
            txtRate.Enabled = true;
            _clientConnected = false;
            stulblSoftware.Text = "Software Listening";
            stulblSoftware.Image = _imgRun;
            _isSoftwareError = false;
            dgvRelativeSettings.ReadOnly = false;
        }
    }
}
