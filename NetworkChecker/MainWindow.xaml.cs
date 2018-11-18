using NetworkChecker.Common;
using NetworkChecker.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
 * ref. http://www.csharpstudy.com/Tip/Tip-network-connectivity.aspx
 * 
 * 
 * 
 */

namespace NetworkChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        LogUnitViewModel logUnitViewModel = new LogUnitViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            lineDataGrid.ItemsSource = logUnitViewModel.Items;
            StatDataGrid.ItemsSource = logUnitViewModel.StatItems;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddLog("프로그램이 시작되었습니다.");

            string ip = ComUtil.CheckPublicIP();
            if(ip == null)
            {
                AddLog("IP를 알 수 없습니다" + ip, false);
            }
            else
            {
                AddLog("IP: " + ip);
            }
            
            NetWorkChange();

            Thread netcheckThread = new Thread(new ThreadStart(Run));
            netcheckThread.IsBackground = true;
            netcheckThread.Start();
        }

        private void Run()
        {
            while(true)
            {
                bool bRet = IsInternetConnected();

                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (bRet)
                    {
                        AddLog("네트워크 정상연결됨 I");
                    }
                    else
                    {
                        AddLog("네트워크 끊김 I", false);
                    }
                }));

               Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void AddLog(string str, bool bConn = true)
        {
            logUnitViewModel.Insert(str, bConn);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logUnitViewModel.Clear();
        }

        public bool IsInternetConnected()
        {
            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try
            {
                // Check NCSI test link
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT)
                {
                    return false;
                }

                // Check NCSI DNS IP
                var dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() < 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        private void NetWorkChange()
        {
            NetworkChange.NetworkAvailabilityChanged += (s, ne) =>
            {
                if (ne.IsAvailable)
                {
                    AddLog("네트워크 정상연결됨");
                }
                else
                {
                    AddLog("네트워크 끊김", false);
                }
            };
        }


#if false //NOT USED
        DispatcherTimer timer = new DispatcherTimer();

        private void InitTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            bool bRet = IsInternetConnected();

            if(bRet)
            {
                AddLog("네트워크 정상연결됨 I");
            }
            else
            {
                AddLog("네트워크 끊김 I");
            }
        }
#endif

    }
}
