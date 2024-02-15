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

namespace Flickett
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }


        private void ForgotPasswordClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Do Forgot password thing");
        }


        private void SignUp_Click(object sender, RoutedEventArgs e) 
        {
            MessageBox.Show("Open Sign Up Page");
        }

       
    }
}
