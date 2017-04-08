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

namespace Wi_Fi_Monitor.Models
{
    public delegate void DimensionNotifyDelegate(string value);
    public delegate void ErrorNotifyDelegate(string error);
    public delegate void MessageNotifyDelegate(string message);

    class Device
    {
        private volatile bool MeasurePermission = false;
        private string Url = "http://127.0.0.1:8000/";        
        private Thread thread;
        public event DimensionNotifyDelegate DimensionGet;
        public event ErrorNotifyDelegate ErrorGet;
        public event MessageNotifyDelegate MessageGet;

        public Device(Wlan.WlanAvailableNetwork network)
        {
            WlanClient client = new WlanClient();
            WlanClient.WlanInterface wlanIface = client.Interfaces[0];

            String strTemplate = Properties.Resources.WPA2PSK; ;
            String authentication = "WPA2PSK";
            String encryption = network.dot11DefaultCipherAlgorithm.ToString().Trim((char)0); ;
            String key = "7822617735";
            String profileXml = String.Format(strTemplate, network.profileName, authentication, key);
            String hex = "";

            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
            wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, network.profileName);
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

            foreach (Wlan.WlanAvailableNetwork network in wlanBssEntries)
            {
                networks.Add(network);
            }

            return networks;
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

        public void DoMeasure(object param)
        {
            SynchronizationContext _context = (SynchronizationContext)param;

            _context.Send(OnMessageGet, "Измерение началось...");

            while (MeasurePermission)
            {
                _context.Send(OnMessageGet, "Получаю значение...");
                try
                {
                    string value = "";

                    var req = (HttpWebRequest)WebRequest.Create(Url);
                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                    using (StreamReader stream = new StreamReader(
                         resp.GetResponseStream(), Encoding.UTF8))
                    {
                        value = stream.ReadToEnd();
                    }

                    _context.Send(OnMessageGet, "Успешно");
                    _context.Send(OnDimensionGet, value);
                    
                }
                catch (Exception err)
                {
                    _context.Send(OnErrorGet, "Ошибка");
                }
                Thread.Sleep(1000);
            } 
        }

        private void OnDimensionGet(object param)
        {
            if (param != null)
                if(DimensionGet != null) DimensionGet((string)param);
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

}
