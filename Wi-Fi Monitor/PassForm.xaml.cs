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

namespace Wi_Fi_Monitor
{
    /// <summary>
    /// Логика взаимодействия для PassForm.xaml
    /// </summary>

 
    public partial class PassForm : Window
    {

        public PassForm()
        {
            InitializeComponent();
        }

        public string Data
        {
            get
            {
                return Password_box.Text;
            }
        }
        private void Enter_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
