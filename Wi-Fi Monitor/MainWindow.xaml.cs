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
using Wi_Fi_Monitor.Views;
using System.Threading;

namespace Wi_Fi_Monitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel View = new MainViewModel();

        Device EDSSDevice;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = View;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                View.Networks.Clear();
                WriteConsoleBlock("Выполняю поиск сетей...");
                var networks = Device.FindNetworks();
                foreach (var network in networks)
                {
                    View.Networks.Add(new Network(network));
                }
                WriteConsoleBlock("Сети обнаружены...");
            }
            catch(Exception err)
            {
                WriteConsoleBlock(String.Format("Ошибка! {0}", err.Message));
            }

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (View.SelectedNetwork == null)
            {
                WriteConsoleBlock("Ошибка! Выберите сеть!");
            }
            else
            {
                try
                {
                    WriteConsoleBlock("Подключаюсь...");
                    EDSSDevice = new Device(View.SelectedNetwork);
                    WriteConsoleBlock("Подключено!");
                    EDSSDevice.ErrorGet += WriteError;
                    EDSSDevice.MessageGet += WriteMessage;
                    StartMeasureButton.IsEnabled = true;
                    StopMeasureButton.IsEnabled = true;
                }
                catch(Exception err)
                {
                    WriteConsoleBlock(String.Format("Ошибка! {0}", err.Message));
                }
            }
            
        }

        private void StartMeasureButton_Click(object sender, RoutedEventArgs e)
        {
        
            EDSSDevice.DimensionGet += this.WriteDimension;
            EDSSDevice.StartMeasure();
        }

        private void StopMeasureButton_Click(object sender, RoutedEventArgs e)
        {
            EDSSDevice.StopMeasure();
            EDSSDevice.DimensionGet -= this.WriteDimension;
        }

        private void WriteDimension(string value)
        {
            if (String.Equals(value, "null")) value = "отсутствует!";

            DimensionsBlock.Text += String.Format("Значение {0}\n", value);
        }

        private void WriteMessage(string message)
        {
            WriteConsoleBlock(message);
        }

        private void WriteError(string error)
        {
            WriteConsoleBlock(String.Format("Ошибка! {0}", error));
        }

        private void WriteConsoleBlock(string msg)
        {
            DateTime localDate = DateTime.Now;
            ConsoleBlock.Text += String.Format("{0}: {1}\n", localDate.ToLongTimeString(), msg);
        }
    }
}
