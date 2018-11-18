using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetworkChecker.Common
{
    public class ComUtil
    {
        public static bool NetworkCheck()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return true;
            }

            return false;
        }

        public static string CheckPublicIP()
        {
            NetworkCheck();

            WebClient wc = new WebClient();
            try
            {
                string result = wc.DownloadString("http://checkip.dyndns.org");
                string ip = result.Split(':')[1].Split('<')[0].Trim();

                return ip;
            }

            catch (Exception e)
            {

            }

            return null;
        }



    }
}
