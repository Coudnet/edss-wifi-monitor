using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Wi_Fi_Monitor.Models
{
    public delegate void DimensionNotifyDelegate(Dimension value);
    public delegate void ErrorNotifyDelegate(string error);
    public delegate void MessageNotifyDelegate(string message);

    class Device
    {
        private volatile bool MeasurePermission = false;
        private string Url = "http://192.168.4.1/dim";
        private Thread thread;
        public event DimensionNotifyDelegate DimensionGet;
        public event ErrorNotifyDelegate ErrorGet;
        public event MessageNotifyDelegate MessageGet;

        public static void Connect(Network network, string pass)
        {
            WlanClient client = new WlanClient();
            WlanClient.WlanInterface wlanIface = client.Interfaces[0];

            String strTemplate = Properties.Resources.WPA2PSK;
            String authentication = "WPA2PSK";
            String encryption = network.CipherAlgorithm;
            String key = pass;
            String profileXml = String.Format(strTemplate, network.SSID, authentication, key);
            String hex = "";

            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
            wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, network.SSID);
        }

        public static ObservableCollection<Wlan.WlanAvailableNetwork> FindNetworks()
        {
            WlanClient client = new WlanClient();
            var networks = new ObservableCollection<Wlan.WlanAvailableNetwork>();

            WlanClient.WlanInterface wlanIface = client.Interfaces[0]; //Получаем первый интерфейс, который связан с сетевой картой
            if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected)
            {
                throw new Exception("Интерфейс занят! Отключите все сети");
            }

            Wlan.WlanAvailableNetwork[] wlanBssEntries = wlanIface.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles);

            IEnumerable<Wlan.WlanAvailableNetwork> sortedNetworks = wlanBssEntries.OrderByDescending(s => s.wlanSignalQuality);
            foreach (Wlan.WlanAvailableNetwork network in sortedNetworks)
            {
                networks.Add(network);
            }

            return networks;
        }

        public static bool IsConnectedToNetwork()
        {
            WlanClient client = new WlanClient();
            WlanClient.WlanInterface wlanIface = client.Interfaces[0]; //Получаем первый интерфейс, который связан с сетевой картой
            if (wlanIface.InterfaceState == Wlan.WlanInterfaceState.Connected) return true;
            return false;
        }

        public void StartMeasure()
        {
            MeasurePermission = true;
            thread = new Thread(this.DoMeasure);
            thread.Start(SynchronizationContext.Current);
        }

        public void StopMeasure()
        {
            MeasurePermission = false;
            thread.Join();
           
            MessageGet("Измерение окончено");
        }

        private void DoMeasure(object param)
        {
            SynchronizationContext _context = (SynchronizationContext)param;

            _context.Send(OnMessageGet, "Измерение началось...");
            String valueDim = "";
            int i = 1000;
            while (MeasurePermission)
            {
                _context.Send(OnMessageGet, "Получаю значение...");
                try
                {
                    string value = "";                    
                   
                    var req = (HttpWebRequest)WebRequest.Create(Url);
                    req.Timeout = 10000;
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    using (StreamReader stream = new StreamReader(
                         resp.GetResponseStream(), Encoding.UTF8))
                    {
                        value = stream.ReadToEnd();

                    }
                    //_context.Send(OnMessageGet, value);
                    //_context.Send(OnMessageGet, valueDim);
                    resp.Close();
                    if(value != "/" && value != "")
                    {
                        valueDim += value;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        _context.Send(OnDimensionGet, valueDim);
                        valueDim = "";
                        _context.Send(OnMessageGet, "Успешно");
                        Thread.Sleep(1000);

                    }
                   // _context.Send(OnMessageGet, "Успешно");
                  
                    
                }
                catch (Exception err)
                {
                    _context.Send(OnErrorGet, String.Format("Ошибка. {0}", err.Message));
                    Thread.Sleep(1000);
               }
                
            } 
        }

        private void OnDimensionGet(object param)
        {
            if (param != null)
                if(DimensionGet != null) DimensionGet(new Dimension((string)param));
        }

        private void OnMessageGet(object param)
        {
            if (param != null)
                if(MessageGet != null) MessageGet((string)param);
        }

        private void OnErrorGet(object param)
        {
            if (param != null)
                if(ErrorGet != null) ErrorGet((string)param);
        }


    }

    public class Dimension
    {
        public string value;
        public string time;

        public Dimension(string val)
        {
            DateTime localDate = DateTime.Now;
            time = localDate.ToLongTimeString();
            value = val;
        }

    }


}
