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
            UsernameTextBox.txtInput.TextChanged += UsernameTextBox_TextChanged;
            EmailTextBox.txtInput.TextChanged += EmailTextBox_TextChanged;
            PhoneTextBox.txtInput.TextChanged += PhoneTextBox_TextChanged;
            FlNames.txtInput.TextChanged += FlNames_TextChanged;

        }

        private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateRegisterButtonState()
        {
            RegisterButton.IsEnabled = check;
        }


        string connstring = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";

        bool check = false;


        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {



            string username = UsernameTextBox.txtInput.Text;
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
                            UsernameTextBox.txtInput.BorderBrush = System.Windows.Media.Brushes.White;
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
                MessageBox.Show("Database error !");
            }

            UpdateRegisterButtonState();

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


            UpdateRegisterButtonState();

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

            UpdateRegisterButtonState();
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


            UpdateRegisterButtonState();


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

            UpdateRegisterButtonState();
        }

        private void FlNames_TextChanged(object sender, RoutedEventArgs e)
        {
            if (FlNames.txtInput.Text.Length > 0)
            {

                string[] names = FlNames.txtInput.Text.Split(' ');

                if (names.Length >= 2)
                {
                    FLNameErrorBox.Text = "";
                    check = true;
                }
                else
                {
                    FLNameErrorBox.Text = "Please enter both first and last names.";
                    check = false;
                }

            }
            else
            {
                FLNameErrorBox.Text = "";
                check = false;
            }


            UpdateRegisterButtonState();
        }

        private void PassbtnClear_Click(object sender, RoutedEventArgs e)
        {
            PassBox.Clear();
            PassBox.Focus();
        }

        private void RepeatPassbtnClear_Click(object sender, RoutedEventArgs e)
        {
            RepeatPassBox.Clear();
            RepeatPassBox.Focus();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!check) 
            {
                MessageBox.Show("User registration failed: please check all fields !");
                return;
            }

            string[] names = FlNames.txtInput.Text.Split(' ');

            string firstName = names[0];
            string lastName = names.Length > 1 ? names[1] : "";
            string username = UsernameTextBox.txtInput.Text;
            string password = PassBox.Password;
            string email = EmailTextBox.txtInput.Text;
            string phone = PhoneTextBox.txtInput.Text;

            try
            {
                string hashPassword = Login.HashPassword(password);

                using (MySqlConnection con = new MySqlConnection(connstring))
                {
                    con.Open();
                    string sql = "INSERT INTO Users (username,passwod,first_name,last_name,email,phone) VALUES (@Username,@Passwod,@First_Name,@Last_Name,@Email,@Phone)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Passwod", hashPassword);
                        cmd.Parameters.AddWithValue("@First_Name", firstName);
                        cmd.Parameters.AddWithValue("@Last_Name", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Login login = new Login();
                            login.Show();
                            this.Hide();
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
    }
}