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
    public partial class ConnectQualityView : UserControl
    {
        public ConnectQualityView()
        {
            InitializeComponent();
        }

        #region "Declarations"
        
        string _quality = "Off";

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

        private void ConnectQualityView_Load(object sender, System.EventArgs e)
        {
            timer1.Enabled = true;
            this.DoubleBuffered = true;
        }

        private void Timer1_Tick(System.Object sender, System.EventArgs e)
        {
            lblConnectStatus.Refresh();
            CheckInetConnection();
        }

        public void CheckInetConnection()
        {
            int tmpflags = 0;
            if (InternetGetConnectedState(ref tmpflags, 0))
            {
                InetConnState flags = (InetConnState)tmpflags;
                // True
                if ((flags & InetConnState.lan) == InetConnState.lan)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                    this.Refresh();
                }
                else if ((flags & InetConnState.modem) == InetConnState.modem)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                }
                else if ((flags & InetConnState.configured) == InetConnState.configured)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                }
                else if ((flags & InetConnState.proxy) == InetConnState.proxy)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                }
                else if ((flags & InetConnState.ras) == InetConnState.ras)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                }
                else if ((flags & InetConnState.offline) == InetConnState.offline)
                {
                    switch (_quality)
                    {
                        case "Good":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Intermittent":
                            lblConnectStatus.ForeColor = Color.Green;
                            lblConnectStatus.Text = "Connection Quality:  Good";
                            _quality = "Good";
                            break;
                        case "Off":
                            lblConnectStatus.ForeColor = Color.DarkOrange;
                            lblConnectStatus.Text = "Connection Quality:  Intermittent";
                            _quality = "Intermittent";
                            break;
                    }
                }
            }
            else
            {
                // False
                switch (_quality)
                {
                    case "Good":
                        lblConnectStatus.ForeColor = Color.DarkOrange;
                        lblConnectStatus.Text = "Connection Quality:  Intermittent";
                        _quality = "Intermittent";
                        break;
                    case "Intermittent":
                        lblConnectStatus.ForeColor = Color.Red;
                        lblConnectStatus.Text = "Connection Quality:  Off";
                        _quality = "Off";
                        break;
                    case "Off":
                        lblConnectStatus.ForeColor = Color.Red;
                        lblConnectStatus.Text = "Connection Quality:  Off";
                        _quality = "Off";
                        break;
                }
            }
        }
        #endregion
    }
}
