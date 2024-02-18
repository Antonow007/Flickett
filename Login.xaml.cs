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
            Register reg = new Register();
            reg.Show();
            this.Close();
        } 

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

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















    }
}
