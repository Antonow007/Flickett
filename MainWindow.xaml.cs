using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
       
     
        public void LoginButton_Click(object sender, RoutedEventArgs e) 
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            try
            {
                string hashedPass = MainWindow.HashPassword(password);


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