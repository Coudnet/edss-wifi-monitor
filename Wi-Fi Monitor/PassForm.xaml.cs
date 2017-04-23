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


        private void Enter_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            if (main != null)
            {
                main.pass = Password_box.Text;
            }
            this.Close();
        }

        private void Exit_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
