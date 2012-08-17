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

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Initial status image
            tslbStatus.Image = Properties.Resources.error;
            tslbHardware.Image = Properties.Resources.error;
            tslbSoftware.Image = Properties.Resources.error;

            // Load log4net config file
            XmlConfigurator.Configure(new FileInfo("log4netconfig.xml"));
            Thread networkThread = new Thread(new ThreadStart(updateNetworkStatus));
            networkThread.IsBackground = true;
            networkThread.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start TCP Server

        }

        #region Methods

        private void updateNetworkStatus()
        {
            while (true)
            {
                if (isNetworkConnected())
                {
                    tslbHardware.Text = "Hardware OK";
                    tslbHardware.Image = Properties.Resources.working;
                }
                else
                {
                    tslbHardware.Text = "Hardware Error";
                    tslbHardware.Image = Properties.Resources.error;
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
        
        #endregion
    }
}
