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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace Flickett
{
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
            Register reg = new Register();
            reg.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {

                string hashedPass = Login.HashPassword(password);


                string connstring = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();
                    string sql = "SELECT username,passwod FROM Users WHERE username = @Username AND passwod=@Password";
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("Password", hashedPass);
                        object result = cmd.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show("User not found. Please register.");
                        }
                        else
                        {
                            string hashedPassword = HashPassword(password);

                            bool passwordMatches = VerifyPassword(password, hashedPassword);


                            if (passwordMatches)
                            {
                                MessageBox.Show($"Login successful: Welcome {username}!");



                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.");
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }



        public static string HashPassword(string password)
        {


            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);


            using (SHA256 sha256 = SHA256.Create())
            {

                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", String.Empty);

                return hashedPassword.Length > 50 ? hashedPassword.Substring(0, 50) : hashedPassword;
            }




        }

        static bool VerifyPassword(string password, string hashedPassword)
        {
            string userInputHashed = HashPassword(password);
            return string.Equals(userInputHashed, hashedPassword, StringComparison.OrdinalIgnoreCase);
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UsernameTextBox.Text.Length > 0)
            {
                loginUsernamePlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                loginUsernamePlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Length > 0)
            {
                loginPasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                loginPasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void loginPasswordClear_Click(object sender, RoutedEventArgs e)
        {
           
            PasswordBox.Clear();
            PasswordBox.Focus();
        }

        private void loginUsernameClear_Click(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.Clear();
            UsernameTextBox.Focus();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
