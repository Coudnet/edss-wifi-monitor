using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;

namespace Wi_Fi_Monitor.Models
{
    class Device
    {
        public static List<Wlan.WlanAvailableNetwork> FindNetworks()
        {
            WlanClient client = new WlanClient();
            var networks = new List<Wlan.WlanAvailableNetwork>();

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {

                Wlan.WlanAvailableNetwork[] wlanBssEntries = wlanIface.GetAvailableNetworkList(0);

                foreach (Wlan.WlanAvailableNetwork network in wlanBssEntries)
                {
                    networks.Add(network);
                }
            }

            return networks;
        }
    }
}
