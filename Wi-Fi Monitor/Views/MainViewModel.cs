using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeWifi;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Wi_Fi_Monitor.Models;

namespace Wi_Fi_Monitor.Views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Network selectedNetwork;

        public ObservableCollection<Network> Networks { get; set; }

        public Network SelectedNetwork
        {
            get { return selectedNetwork; }
            set
            {
                selectedNetwork = value;
                OnPropertyChanged("SelectedNetwork");
            }
        }

        public MainViewModel()
        {
            Networks = new ObservableCollection<Network>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
