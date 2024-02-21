using Flickett.View.UserControls;
using MySql.Data.MySqlClient;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            UsenameTextBox.txtInput.TextChanged += UsernameTextBox_TextChanged;
            PasswordTextBox.txtInput.TextChanged += PasswordTextBox_TextChanged;
            RepeatPassTextBox.txtInput.TextChanged += RepeatPassTextBox_TextChanged;
        }

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FlNames.txtInput.BorderBrush = System.Windows.Media.Brushes.Red;
            UsernameErrorBox.Text = "This Username has already taken";
        }


        bool check = false;


        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            

                string username = UsenameTextBox.txtInput.Text;
                try
                {

                    string connstring = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
                    using (MySqlConnection con = new MySqlConnection(connstring))
                    {
                        con.Open();
                        string sql = "SELECT passwod FROM Users WHERE username = @Username";
                        using (MySqlCommand cmd = new MySqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            object result = cmd.ExecuteScalar();

                            if (result == null)
                            {
                                UsernameErrorBox.Text = "";
                                UsenameTextBox.txtInput.BorderBrush= System.Windows.Media.Brushes.White;
                                check = true;
                            }
                            else
                            {
                                UsernameErrorBox.Text = "Username has already been taken";
                               
                                check = false;
                            }

                        }
                    }
                }
                catch (MySqlException)
                {
                    MessageBox.Show("Username already exists !");
                }
            

        }


        private void PasswordTextBox_TextChanged( object sender, TextChangedEventArgs e) 
        {

            if (PasswordTextBox.txtInput.Text.Length < 6)
            {
                PasswordErrorBox.Text = "Password must be at least 6 characters";
                
                check = true;
            }
            else
            {
                PasswordErrorBox.Text = "";

                check = false;


            }
        }

        private void RepeatPassTextBox_TextChanged(object sender, TextChangedEventArgs e) 
        {

            if (RepeatPassTextBox.txtInput.Text != PasswordTextBox.txtInput.Text && RepeatPassTextBox.txtInput.Text.Length > 0)
            {
                RepeatPassErrorBox.Text = "Passwords not match!";
                check = false;
            }
            else
            {
                RepeatPassErrorBox.Text = "";
                check = true;
            }






        }


    }
}
