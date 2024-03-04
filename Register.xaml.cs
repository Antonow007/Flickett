using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
            EmailTextBox.txtInput.TextChanged += EmailTextBox_TextChanged;
            PhoneTextBox.txtInput.TextChanged += PhoneTextBox_TextChanged;

        }

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
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
                            UsenameTextBox.txtInput.BorderBrush = System.Windows.Media.Brushes.White;
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





        private void EmailTextBox_TextChanged(object sender, EventArgs e)
        {



            string email = EmailTextBox.txtInput.Text.Trim();

            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            if (string.IsNullOrWhiteSpace(email))
            {
                EmailErrorBox.Text = "";
                return;
            }

            if (Regex.IsMatch(email, pattern))
            {
                EmailErrorBox.Text = "";
                check = true;
            }
            else
            {
                EmailErrorBox.Text = "Invalid email address!";
                check = false;
            }




        }


        private void PhoneTextBox_TextChanged(Object sender, EventArgs e)
        {


            string phoneNumber = PhoneTextBox.txtInput.Text.Trim();

            string pattern = @"^(\+359|0)8[7-9][0-9]{7}$";

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                PhoneErrorBox.Text = "";
                return;
            }

            if (Regex.IsMatch(phoneNumber, pattern))
            {
                PhoneErrorBox.Text = "";
                check = true;
            }
            else
            {
                PhoneErrorBox.Text = "Invalid phone number!";
                check = false;
            }

        }

        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassBox.Password.Length > 0)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }

            if (PassBox.Password.Length < 6 && PassBox.Password.Length != 0)
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RepeatPassBox.Password.Length > 0)
            {
                RepeatPassPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                RepeatPassPlaceholder.Visibility = Visibility.Visible;
            }


            if (RepeatPassBox.Password != PassBox.Password && RepeatPassBox.Password.Length > 0)
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
