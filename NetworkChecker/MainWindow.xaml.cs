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

        DispatcherTimer timer = new DispatcherTimer();

        LogUnitViewModel logUnitViewModel = new LogUnitViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;

            lineDataGrid.ItemsSource = logUnitViewModel.Items;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AddLog("프로그램이 시작되었습니다.");

            string ip = CheckPublicIP();
            AddLog("IP: " + ip);

            //InitTimer();

            NetWorkChange();

            Thread t1 = new Thread(new ThreadStart(Run));
            t1.IsBackground = true;
            t1.Start();
        }

        private void InitTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += Timer_Tick;
            timer.Start();
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
                        AddLog("네트워크 끊김 I");
                    }
                }));

               Thread.Sleep(TimeSpan.FromSeconds(5));
            }
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

        private void AddLog(string str)
        {
            logUnitViewModel.Insert(str);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logUnitViewModel.Clear();
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
                    AddLog("네트워크 끊김");
                }
            };
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

        private void lineDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private string CheckPublicIP()
        {
            NetworkCheck();

            WebClient wc = new WebClient();
            try
            {
                string result = wc.DownloadString("http://checkip.dyndns.org");
                string ip = result.Split(':')[1].Split('<')[0].Trim();

                return ip;
            }

            catch(Exception e)
            {

            }

            return "IP를 체크할 수 없습니다";
        }

        private void NetworkCheck()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                AddLog("정상적으로 연결되었습니다");
            }
            else
            {
                AddLog("네트워크를 이용할 수 없습니다");
            }

            IsInternetConnected();
        }


    }
}
