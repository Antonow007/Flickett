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
using System.Text.RegularExpressions;


namespace Flickett
{
    /// <summary>
    /// Interaction logic for lgnWindow.xaml
    /// </summary>
    /// 
    public partial class lgnWindow : Window
    {

        bool isUsernameValid = false;
        bool isEmailValid = false;
        bool isPhoneValid = false;
        bool isPasswordValid = false;
        bool isRepeatPasswordValid = false;


        private readonly string connstring = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";


        public lgnWindow()
        {
            InitializeComponent();
        }

        //Login Part

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
                                    MessageBox.Show($"Login successful: Welcome {username}!");
                                }
                                else
                                {
                                    LoginPasswordErrorBox.Text = "Wrong Password!";
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

        //Register Part

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


            string username = RegisterUsernameBox.Text;

            try
            {

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
                            RegisterUsernameBox.BorderBrush = System.Windows.Media.Brushes.White;
                            isUsernameValid = true;
                        }
                        else
                        {
                            UsernameErrorBox.Text = "Username has already been taken";
                            RegisterUsernameBox.BorderBrush = System.Windows.Media.Brushes.Red;
                            isUsernameValid = false;
                        }

                    }
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("Database error !");
            }

            UpdateRegisterButtonState();
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
            if (RegisterPasswordBox.Password.Length < 6 && RegisterPasswordBox.Password.Length != 0)
            {
                PasswordErrorBox.Text = "Password must be at least 6 characters";
                RegisterPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                isPasswordValid = false;
            }
            else
            {
                PasswordErrorBox.Text = "";
                RegisterPasswordBox.BorderBrush = System.Windows.Media.Brushes.White;
                isPasswordValid = true;
            }

            UpdateRegisterButtonState();
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
            if (RegisterRepeatPasswordBox.Password != RegisterPasswordBox.Password)
            {
                RepeatPasswordErrorBox.Text = "Passwords not match!";
                RegisterRepeatPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                isRepeatPasswordValid = false;
            }
            else
            {
                RepeatPasswordErrorBox.Text = "";
                RegisterRepeatPasswordBox.BorderBrush = System.Windows.Media.Brushes.White;
                isRepeatPasswordValid = true;
            }

            UpdateRegisterButtonState();
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

            string email = RegisterEmailBox.Text.Trim();
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            if (Regex.IsMatch(email, pattern))
            {
                EmailErrorBox.Text = "";
                RegisterEmailBox.BorderBrush = System.Windows.Media.Brushes.White;
                isEmailValid = true;
            }
            else
            {
                EmailErrorBox.Text = "Invalid email address!";
                RegisterEmailBox.BorderBrush = System.Windows.Media.Brushes.Red;
                isEmailValid = false;
            }

            UpdateRegisterButtonState();

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

            string phoneNumber = RegisterPhoneBox.Text.Trim();
            string pattern = @"^(\+359|0)8[7-9][0-9]{7}$";

            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                PhoneErrorBox.Text = "";
                return;
            }

            if (Regex.IsMatch(phoneNumber, pattern))
            {
                PhoneErrorBox.Text = "";
                RegisterPhoneBox.BorderBrush = System.Windows.Media.Brushes.White;
                isPhoneValid = true;
            }
            else
            {
                PhoneErrorBox.Text = "Invalid phone number!";
                RegisterPhoneBox.BorderBrush = System.Windows.Media.Brushes.Red;
                isPhoneValid = false;
            }

            UpdateRegisterButtonState();
        }

        private void UpdateRegisterButtonState()
        {
            RegisterButton.IsEnabled = isUsernameValid && isEmailValid && isPhoneValid && isPasswordValid && isRepeatPasswordValid;
        }

        private bool AllFieldsFilled()
        {
            return
                   !string.IsNullOrWhiteSpace(RegisterUsernameBox.Text) &&
                   !string.IsNullOrWhiteSpace(RegisterPasswordBox.Password) &&
                   !string.IsNullOrWhiteSpace(RegisterRepeatPasswordBox.Password) &&
                   !string.IsNullOrWhiteSpace(RegisterEmailBox.Text) &&
                   !string.IsNullOrWhiteSpace(RegisterPhoneBox.Text);
        }

        private bool IsValidRegistration()
        {
            return isUsernameValid &&
                isPasswordValid &&
                isRepeatPasswordValid &&
                isEmailValid &&
                isPhoneValid;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidRegistration() || !AllFieldsFilled())
            {
                MessageBox.Show("User registration failed:please check all fields !");
                return;
            }




            string username = RegisterUsernameBox.Text;
            string password = RegisterPasswordBox.Password;
            string email = RegisterEmailBox.Text;
            string phone = RegisterPhoneBox.Text;

            try
            {
                string hashPassword = HashPassword(password);

                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();
                    string sql = "INSERT INTO Users (username,passwod,email,phone) VALUES (@Username,@Passwod,@Email,@Phone)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Passwod", hashPassword);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MoveSliderToRight();
                        }
                        else
                        {
                            MessageBox.Show("User registration failed.");
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
