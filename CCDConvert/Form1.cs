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
        private static string xmlPath = @"C:\Projects\Github\CCDConvert\CCDConvert\config\config.xml";

        private double _offset_default_y, offset_y, offset_x;
        private Dictionary<string, string> dicRelative = new Dictionary<string, string>();

        // TCP Server
        private TcpListener tcpListener;
        private Thread listenThread;

        // TCP Client
        private byte[] data = new byte[1024];
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
            //tslbStatus.Image = Properties.Resources.Stop;
            tslbHardware.Image = Properties.Resources.Stop;
            tslbSoftware.Image = Properties.Resources.Stop;

            // Load log4net config file
            XmlConfigurator.Configure(new FileInfo("log4netconfig.xml"));
            Thread networkThread = new Thread(new ThreadStart(updateNetworkStatus));
            networkThread.IsBackground = true;
            networkThread.Start();


            // Load xml config , this is first step.
            bool IsConfigured = getXml(xmlPath, dgvRelativeSettings);

            // ** Notice : Before use [converData] need run again. 本來要將Relative的資料綁入converData 但效能差了2倍 
            updateDictionaryRelative();

            //if (IsConfigured)
            //{
            //    Stopwatch sw = new Stopwatch();
            //    sw.Start();
            //    convertData("DATA,FlawID,0;FlawName,WS;FlawMD,0.804000;FlawCD,0.924000;JobID,231tst-13;", _offset_default_y);
            //    sw.Stop();
            //    MessageBox.Show(sw.Elapsed.ToString());
            //}
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start TCP Server
            try
            {
                btnStart.Enabled = false;
                tcpListener = new TcpListener(IPAddress.Parse(txtSourceIP.Text), Convert.ToInt32(txtSourcePort.Text));
                listenThread = new Thread(new ThreadStart(ListenForClients));
                listenThread.IsBackground = true;
                listenThread.Start();

                log.Info("Success to start TCPServer");
            }
            catch (Exception ex)
            {
                log.Error("Fail to start TCPServer. Error: " + ex.Message);
            }
            //if (isConnect) sendData("Server Data!!!");
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
                    tslbSoftware.Text = "Software OK";
                    tslbSoftware.Image = Properties.Resources.Run;
                }
                catch (SocketException)
                {
                    log.Error("Fail to connect to server");
                    return;
                }
            }
            else
            {
                outputStream.Close();
                outputClient.Close();
                btnStop.Text = "Send(&S)";
                isConnect = false;
                tslbSoftware.Text = "Software Error";
                tslbSoftware.Image = Properties.Resources.Stop;
            }
        }

        #region Methods

        private void updateNetworkStatus()
        {
            Image imgRun = Properties.Resources.Run;
            Image imgStop = Properties.Resources.Stop;
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

            if (regex.IsMatch(input))
            {
                string[] tmp = input.Substring(input.IndexOf(',') + 1).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

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
            }

            // Deal format string
            //double offset_y = double.TryParse(txtY.Text, out offset_y) ? offset_y : 0;
            //double offset_x = double.TryParse(txtX.Text, out offset_x) ? offset_x : 0;
            double y = double.Parse(dicOutpout["FlawMD"]) * 1000 + _offset_default_y + offset_y;
            double x = double.Parse(dicOutpout["FlawCD"]) * 1000 + offset_x;
            string result = "";
            if (dicRelative.ContainsKey(dicOutpout["FlawName"]))
                result = String.Format("{0};{1};{2}", dicRelative[dicOutpout["FlawName"]], y.ToString(), x.ToString());
            else
                result = String.Format("{0};{1};{2}", "0", y.ToString(), x.ToString());

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

            dgv.EndEdit();
            for (int i = 0; i < dgv.Rows.Count - 1; i++)
            {
                string source = dgv.Rows[i].Cells[0].Value.ToString();
                string target = dgv.Rows[i].Cells[1].Value.ToString();
                navigator.SelectSingleNode("//relative_table").AppendChildElement(string.Empty, "column", string.Empty, null);
                // Move to last column element and add source , target value.
                navigator.SelectSingleNode("//relative_table/column[last()]").AppendChildElement(string.Empty, "source", string.Empty, source.ToUpper());
                navigator.SelectSingleNode("//relative_table/column[last()]").AppendChildElement(string.Empty, "target", string.Empty, target.ToUpper());
               
            }
            document.Save(path); 
        }

        private void ListenForClients()
        {
            try
            {
                tcpListener.Start();
                while (true)
                {
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient();

                    //create a thread to handle communication
                    //with connected client
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                log.Error("Fail to ListenForClients. Error: " + ex.Message);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception ex)
                {
                    //a socket error has occured
                    log.Error("Fail to HandleClientComm. Error: " + ex.Message);
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                UnicodeEncoding encoder = new UnicodeEncoding();
                string recvData = encoder.GetString(message, 0, bytesRead);

                ModifyTextBox("Input: " + recvData);
                log.Info("Input: " + recvData);
                if (isConnect)
                {
                    recvData = convertData(recvData, _offset_default_y);
                    sendData(recvData);
                }
            }

            tcpClient.Close();
        }

        public delegate void ModifyTextBoxDelegate(String s);
        private void ModifyTextBox(String s)
        {
            if (txtLog.InvokeRequired)
            {
                ModifyTextBoxDelegate d = new ModifyTextBoxDelegate(ModifyTextBox);
                this.Invoke(d, s);
            }
            else
            {
                txtLog.Text = txtLog.Text + s + Environment.NewLine;
            }
            return;
        }

        private void sendData(string output)
        {
            try
            {
                outputStream = outputClient.GetStream();

                UnicodeEncoding encoder = new UnicodeEncoding();
                byte[] buffer = encoder.GetBytes(output + "\r");
                ModifyTextBox("Output: " + output);
                log.Info("Output: " + output);

                outputStream.Write(buffer, 0, buffer.Length);
                outputStream.Flush();
                tslbSoftware.Text = "Software OK";
                tslbSoftware.Image = Properties.Resources.Run;
            }
            catch (NetworkInformationException ex)
            {
                tslbSoftware.Text = "Software Error";
                tslbSoftware.Image = Properties.Resources.Stop;
                log.Error("Fail to send data. Error: " + ex.Message);
            }
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
                log.Info("Application Close");
            }
            else if (drResult == DialogResult.No)
            {
                e.Cancel = true;
            }

        }
        #endregion

       



    }
}
