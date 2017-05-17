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
using System.Windows.Shapes;
using System.Configuration;

namespace Wi_Fi_Monitor
{
    /// <summary>
    /// Логика взаимодействия для SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : Window
    {
        public SettingsForm()
        {
            InitializeComponent();
            IPBox.Text = ConfigurationManager.AppSettings["server"];
            FreqBox.Text = ConfigurationManager.AppSettings["freq"];
            TimeoutBox.Text = ConfigurationManager.AppSettings["timeout"];
        }



        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Configuration.Configuration currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            currentConfig.AppSettings.Settings["server"].Value = IPBox.Text;
            currentConfig.AppSettings.Settings["freq"].Value = FreqBox.Text;
            currentConfig.AppSettings.Settings["timeout"].Value = TimeoutBox.Text;
            currentConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            this.Close();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
