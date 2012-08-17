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

namespace CCDConvert
{
    public partial class FormMain : Form
    {
        #region Local variables

        private static ILog log = LogManager.GetLogger(typeof(Program));
        private Image imgWorking = Properties.Resources.working;
        private Image imgError = Properties.Resources.error;
        // For TCP Server
        private TcpListener srvTcpListener = null;
        private Thread srvListenThread = null;
        // For TCP Client
        private TcpClient tcpClient = null;
        private TcpListener tcpListener;
        // For checking network status
        private Thread networkThread = null;

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Initial status image
            tslbStatus.Image = imgError;
            tslbHardware.Image = imgError;
            tslbSoftware.Image = imgError;

            // Load log4net config file
            XmlConfigurator.Configure(new FileInfo("log4netconfig.xml"));
            networkThread = new Thread(new ThreadStart(updateNetworkStatus));
            networkThread.IsBackground = true;
            networkThread.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start TCP Server
            //if (listenThread == null)
            //{
            //    btnStart.Text = "Stop";
            srvTcpListener = new TcpListener(IPAddress.Parse(txtSourceIP.Text), Int32.Parse(txtSourcePort.Text));
            srvListenThread = new Thread(new ThreadStart(ListenForClients));
            srvListenThread.IsBackground = true;
            srvListenThread.Start();
            btnStart.Enabled = false;
            //}
            //else
            //{
            //    btnStart.Text = "Start";
            //    listenThread.Abort();
            //    listenThread = null;
            //    tcpListener.Stop();
            //    tcpListener = null;
            //}
            // For TCP Client
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(txtDestIP.Text), Convert.ToInt32(txtDestPort.Text));
            tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(serverEndPoint);
            }
            catch { }
        }

        #region Methods

        private void updateNetworkStatus()
        {
            while (true)
            {
                if (isNetworkConnected())
                {
                    tslbHardware.Text = "Hardware OK";
                    tslbHardware.Image = imgWorking;
                }
                else
                {
                    tslbHardware.Text = "Hardware Error";
                    tslbHardware.Image = imgError;
                }
            }
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

        private void ListenForClients()
        {
            srvTcpListener.Start();

            while (srvListenThread != null && srvListenThread.IsAlive)
            {
                //blocks until a client has connected to the server
                TcpClient client = srvTcpListener.AcceptTcpClient();

                //create a thread to handle communication
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client)
        {
            TcpClient srvTcpClient = (TcpClient)client;
            NetworkStream clientStream = srvTcpClient.GetStream();
            
            byte[] message = new byte[4096];
            int bytesRead;

            while (srvListenThread != null && srvListenThread.IsAlive)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                UnicodeEncoding encoder = new UnicodeEncoding();
                MessageBox.Show(encoder.GetString(message, 0, bytesRead));
                responseMessage();
            }

            srvTcpClient.Close();
        }

        private void responseMessage()
        {
            NetworkStream clientStream = tcpClient.GetStream();

            UnicodeEncoding encoder = new UnicodeEncoding();
            byte[] buffer = encoder.GetBytes("Server Response" + "\r");

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        #endregion
    }
}
