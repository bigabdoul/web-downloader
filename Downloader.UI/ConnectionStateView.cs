using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Downloader.UI
{
    public partial class ConnectionStateView : UserControl
    {
        public ConnectionStateView()
        {
            InitializeComponent();
        }

        #region "Declarations"

        private string ConnectionStateString;

        [DllImport("wininet.dll", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool InternetGetConnectedState(ref int lpSFlags, int dwReserved);

        enum InetConnState
        {
            modem = 0x1,
            lan = 0x2,
            proxy = 0x4,
            ras = 0x10,
            offline = 0x20,
            configured = 0x40
        }

        #endregion

        #region "Control Methods"

        private void ConnectStateView_Load(object sender, System.EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void Timer1_Tick(System.Object sender, System.EventArgs e)
        {
            CheckInetConnection();
            lblConnectStatus.Text = "Connection Type:  " + ConnectionStateString;
        }

        public void CheckInetConnection()
        {
            int tmpflags = 0;
            if (InternetGetConnectedState(ref tmpflags, 0))
            {
                InetConnState flags = (InetConnState)tmpflags;
                if ((flags & InetConnState.lan) == InetConnState.lan)
                {
                    ConnectionStateString = "LAN";
                    lblConnectStatus.Image = ImageList1.Images[1];
                }
                else if ((flags & InetConnState.modem) == InetConnState.modem)
                {
                    ConnectionStateString = "Modem";
                    lblConnectStatus.Image = ImageList1.Images[1];
                }
                else if ((flags & InetConnState.configured) == InetConnState.configured)
                {
                    ConnectionStateString = "Configured";
                    lblConnectStatus.Image = ImageList1.Images[1];
                }
                else if ((flags & InetConnState.proxy) == InetConnState.proxy)
                {
                    ConnectionStateString = "Proxy";
                    lblConnectStatus.Image = ImageList1.Images[1];
                }
                else if ((flags & InetConnState.ras) == InetConnState.ras)
                {
                    ConnectionStateString = "RAS";
                    lblConnectStatus.Image = ImageList1.Images[1];
                }
                else if ((flags & InetConnState.offline) == InetConnState.offline)
                {
                    ConnectionStateString = "Offline";
                    this.lblConnectStatus.Image = ImageList1.Images[2];
                }
            }
            else
            {
                ConnectionStateString = "Not Connected";
                lblConnectStatus.Image = ImageList1.Images[3];
            }
        }
        #endregion
    }
}
