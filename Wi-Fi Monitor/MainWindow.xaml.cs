using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using NativeWifi;
using Wi_Fi_Monitor.Models;

namespace Wi_Fi_Monitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            WiFiNetworks.Items.Clear();
            List<Wlan.WlanAvailableNetwork> networks = Device.FindNetworks();
            foreach (Wlan.WlanAvailableNetwork network in networks)
            {
                WiFiNetworks.Items.Add(System.Text.ASCIIEncoding.ASCII.GetString(network.dot11Ssid.SSID).Trim((char)0));

            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WlanClient client = new WlanClient();
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    Wlan.WlanAvailableNetwork[] wlanBssEntries = wlanIface.GetAvailableNetworkList(0);
                    foreach (Wlan.WlanAvailableNetwork network in wlanBssEntries)
                    {
                        String profileName = System.Text.ASCIIEncoding.ASCII.GetString(network.dot11Ssid.SSID).Trim((char)0);

                        // подключаемся именно к выбранной сети
                        if (WiFiNetworks.SelectedItem.ToString().Equals(profileName)) //Text.Equals(profileName))
                        {
                            String strTemplate = "";
                            String authentication = "";
                            String encryption = "";
                            String key = "";
                            String profileXml = "";
                            String hex = "";

                            ConsoleBlock.Text = "Connect...\n";
                            switch ((int)network.dot11DefaultAuthAlgorithm)
                            {
                                case 1: // Open
                                    ConsoleBlock.Text += "Open";
                                    break;
                                case 2: // SHARED_KEY
                                    ConsoleBlock.Text += "SHARED";
                                    break;
                                case 3: // WEP
                                    ConsoleBlock.Text += "Wep";
                                    break;
                                case 4: // WPA_PSK
                                    ConsoleBlock.Text += "WPA";
                                    strTemplate = Properties.Resources.WPAPSK;

                                    authentication = "WPAPSK";

                                    encryption = network.dot11DefaultCipherAlgorithm.ToString().Trim((char)0);

                                    key = "0000000000";

                                    profileXml = String.Format(strTemplate, profileName, authentication, encryption, key);

                                    wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                                    wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);


                                    break;

                                case 5: // WPA_NONE
                                    ConsoleBlock.Text += "WPA_NONE";
                                    break;
                                case 6: // RSNA
                                    ConsoleBlock.Text += "RSNA";
                                    break;
                                case 7: // RSNA_PSK  
                                    ConsoleBlock.Text += "WPA2PSK";
                                    strTemplate = Properties.Resources.WPA2PSK;
                                    authentication = "WPA2PSK";
                                    encryption = network.dot11DefaultCipherAlgorithm.ToString().Trim((char)0);
                                    key = "76543210";
                                    profileXml = String.Format(strTemplate, profileName, authentication, key);

                                    wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                                    wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
