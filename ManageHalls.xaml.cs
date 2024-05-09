
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MySql.Data.MySqlClient;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for ManageHalls.xaml
    /// </summary>
    public partial class ManageHalls : Window
    {
        public ManageHalls()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
        }


        private void MoveSliderMenuToRight()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(-201, 2, 851, -2);
            animation.To = new Thickness(0, 2, 650, -2);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            animation.EasingFunction = new QuadraticEase();

            SlideMenu.BeginAnimation(Grid.MarginProperty, animation);
        }

        private void MoveSliderMenuToLeft()
        {


            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 2, 650, -2);
            animation.To = new Thickness(-201, 2, 851, -2);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            animation.EasingFunction = new QuadraticEase();

            SlideMenu.BeginAnimation(Grid.MarginProperty, animation);


        }

        private void hamburgerButton_Click_1(object sender, RoutedEventArgs e)
        {
            MoveSliderMenuToRight();

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (!IsMouseOverUIElement(SlideMenu, e.GetPosition(this)))
            {
                MoveSliderMenuToLeft();
            }
        }

        private bool IsMouseOverUIElement(UIElement element, Point mousePosition)
        {
            return element.InputHitTest(mousePosition) != null;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            lgnWindow Login = new lgnWindow();
            Login.Show();
            this.Close();
        }

        private void HallNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(HallNameBox.Text))
            {
                HallNamePlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                HallNamePlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void CapacityBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(CapacityBox.Text))
            {
                CapacityPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                CapacityPlaceholder.Visibility = Visibility.Hidden;
            }
        }

        private void CategoryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(CategoryBox.Text))
            {
                CategoryPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                CategoryPlaceholder.Visibility = Visibility.Hidden;
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
            string hallName = HallNameBox.Text;
            int capacity;
            bool isCapacityValid = int.TryParse(CapacityBox.Text, out capacity);
            string category = CategoryBox.Text;
            int ticketTypeid = 0;

            if (string.IsNullOrWhiteSpace(hallName) || !isCapacityValid || string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Please enter valid data for all fields.");
                return;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Halls (HallName, Capacity, Category,TicketTypeId) VALUES (@HallName, @Capacity, @Category,@TicketTypeId)";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@HallName", hallName);
                command.Parameters.AddWithValue("@Capacity", capacity);
                command.Parameters.AddWithValue("@Category", category);
                if (category=="3d" || category=="3D")
                {
                    ticketTypeid = 25;
                }
                else if (category == "2d" || category == "2D")
                {
                    ticketTypeid = 24;
                }
                else if (category == "imax" || category == "IMAX")
                {
                    ticketTypeid = 23;
                }
                else if (category == "4dx" || category == "4DX")
                {
                    ticketTypeid = 26;
                }
                command.Parameters.AddWithValue("@TicketTypeId", ticketTypeid);
                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Data added successfully.");
                       
                        HallNameBox.Clear();
                        CapacityBox.Clear();
                        CategoryBox.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Data insertion failed.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }


    }
}

