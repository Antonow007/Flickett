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
using System.Security.Cryptography;


namespace Flickett
{
    /// <summary>
    /// Interaction logic for lgnWindow.xaml
    /// </summary>
    public partial class lgnWindow : Window
    {
        public lgnWindow()
        {
            InitializeComponent();
        }

       
        private void MoveSliderToLeft()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(400, 0, 0, 0);
            animation.To = new Thickness(0, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            Slider.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void MoveSliderToRight()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 0, 0, 0);
            animation.To = new Thickness(400, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            Slider.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void SignUP_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToLeft();
        }

        private void SignInRegister_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToRight();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
           
            WindowState = WindowState.Minimized;
        }

        private void LeftSliderButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToLeft();
        }

        private void RightSliderButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToRight();
        }

        private void LoginUsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginUsernameBox.Text))
            {
                UsernamePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                UsernamePlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginPasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterUsernameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterUsernameBox.Text))
            {
                RegisterUsernamePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterUsernamePlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
            {
                RegisterPasswordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterPasswordPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterRepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterRepeatPasswordBox.Password))
            {
                RegisterRepeatPasswordPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterRepeatPasswordPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterEmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterEmailBox.Text))
            {
                RegisterEmailPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterEmailPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void RegisterPhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(RegisterPhoneBox.Text))
            {
                RegisterPhonePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                RegisterPhonePlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            string username = LoginUsernameBox.Text;
            string password = LoginPasswordBox.Password;

            try
            {
                string connstring = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();


                    string usernameQuery = "SELECT COUNT(*) FROM Users WHERE username = @Username";
                    using (MySqlCommand usernameCmd = new MySqlCommand(usernameQuery, con))
                    {
                        usernameCmd.Parameters.AddWithValue("@Username", username);
                        int usernameCount = Convert.ToInt32(usernameCmd.ExecuteScalar());

                        if (usernameCount > 0)
                        {

                            string passwordQuery = "SELECT COUNT(*) FROM Users WHERE username = @Username AND passwod = @Password";
                            using (MySqlCommand passwordCmd = new MySqlCommand(passwordQuery, con))
                            {
                                passwordCmd.Parameters.AddWithValue("@Username", username);
                                passwordCmd.Parameters.AddWithValue("@Password", HashPassword(password));
                                int passwordCount = Convert.ToInt32(passwordCmd.ExecuteScalar());

                                if (passwordCount > 0)
                                {
                                    LoginPasswordBox.BorderBrush = Brushes.White;
                                    MessageBox.Show($"Login successful: Welcome {username}!");
                                }
                                else
                                {
                                    MessageBox.Show("Wrong password.");
                                    LoginPasswordBox.Clear();
                                    LoginPasswordBox.BorderBrush = Brushes.Red;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("User not found. Please register.");
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
    }
}
