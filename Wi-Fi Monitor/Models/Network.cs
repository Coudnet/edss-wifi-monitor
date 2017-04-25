using NativeWifi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wi_Fi_Monitor.Models
{
    public class Network : INotifyPropertyChanged
    {
        private string profileName;
        private uint signalQuality;
        private string cipherAlgorithm;
        private string ssid;
        private bool securityEnabled;

        public string ProfileName
        {
            get { return profileName; }
            set
            {
                profileName = value;
                OnPropertyChanged("ProfileName");
            }
        }
        public uint SignalQuality
        {
            get { return signalQuality; }
            set
            {
                signalQuality = value;
                OnPropertyChanged("SignalQuality");
            }
        }

        public string CipherAlgorithm
        {
            get { return cipherAlgorithm; }
            set
            {
                cipherAlgorithm = value;
                OnPropertyChanged("CipherAlgorithm");
            }
        }

        public string SSID
        {
            get { return ssid; }
            set
            {
                ssid = value;
                OnPropertyChanged("SSID");
            }
        }

        public bool SecurityEnabled
        {
            get { return securityEnabled; }
            set
            {
                securityEnabled = value;
                OnPropertyChanged("SecurityEnabled");
            }
        }

        public Network(Wlan.WlanAvailableNetwork network)
        {
            SSID = System.Text.ASCIIEncoding.ASCII.GetString(network.dot11Ssid.SSID).Trim((char)0);
            ProfileName = network.profileName;
            SignalQuality = network.wlanSignalQuality;
            CipherAlgorithm = network.dot11DefaultCipherAlgorithm.ToString().Trim((char)0);
            SecurityEnabled = network.securityEnabled;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
