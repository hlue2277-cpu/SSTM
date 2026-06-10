using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Helpers
{
    public class NetworkHelper
    {
        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
        public static bool CheckNetworkStatus()
        {
            try
            {
                //bool pingResult = false;
                //Ping ping = new Ping();
                //PingReply pr = ping.Send("baidu.com");
                //pingResult = (pr.Status == IPStatus.Success);
                bool netStatus = false;
                int i;
                netStatus = InternetGetConnectedState(out i, 0);
                return (netStatus);
            }
            catch
            {
                return false;
            }

        }
    }
}
