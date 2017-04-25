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
using System.IO;
using System.Windows.Forms;


namespace Wi_Fi_Monitor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel View = new MainViewModel();
        public string pass;
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
                    if (View.SelectedNetwork.SecurityEnabled)
                    {
                        PassForm passform = new PassForm();
                        passform.ShowDialog();
                        pass = passform.Data;
                    }
                    WriteConsoleBlock("Подключаюсь...");
                    Device.Connect(View.SelectedNetwork, pass);

                    EDSSDevice = new Device();
                    WriteConsoleBlock("Подключено!");

                    EDSSDevice.ErrorGet += WriteError;
                    EDSSDevice.MessageGet += WriteMessage;
                    StartMeasureButton.IsEnabled = true;
                    StopMeasureButton.IsEnabled = true;
                    SaveButton.IsEnabled = true;
                    CleanButton.IsEnabled = true;
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

        private void WriteDimension(Dimension dim)
        {
            DimensionsBlock.Text += String.Format("Значение {0}\n", dim.value);
            DimensionsBlockScroll.ScrollToBottom();
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
            ConsoleBlockScroll.ScrollToBottom();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile1 = new SaveFileDialog();
            saveFile1.DefaultExt = "*.txt";
            saveFile1.Filter = "Text files|*.txt";
            if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                saveFile1.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveFile1.FileName, true))
                {
                    sw.WriteLine(DimensionsBlock.Text);
                    sw.Close();
                    WriteConsoleBlock("Данные сохранены в " + saveFile1.FileName);
                }
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            DimensionsBlock.Text = "";
            WriteConsoleBlock("Очищено");
        }

        private void AlreadyReadyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Device.IsConnectedToNetwork()) throw new Exception("Вы не подключены!");
                EDSSDevice = new Device();
                WriteConsoleBlock("Подключено!");

                EDSSDevice.ErrorGet += WriteError;
                EDSSDevice.MessageGet += WriteMessage;
                StartMeasureButton.IsEnabled = true;
                StopMeasureButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
                CleanButton.IsEnabled = true;
            }
            catch (Exception err)
            {
                WriteConsoleBlock(String.Format("Ошибка! {0}", err.Message));
            }
        }
    }
}
