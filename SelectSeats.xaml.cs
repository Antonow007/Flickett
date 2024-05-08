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
using static Flickett.MainPage;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for SelectSeats.xaml
    /// </summary>
    public partial class SelectSeats : Window
    {
        string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";

        private MovieViewModel movie;
        private string screeningTime;

        public SelectSeats(MovieViewModel movie, string screeningTime)
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.movie = movie;
            this.screeningTime = screeningTime;
        }

        private int GetHallCapacity(string movieId, string screeningTime)
        {
            int hallCapacity = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Capacity FROM Halls " +
                                   "INNER JOIN Screenings ON Halls.HallId = Screenings.HallId " +
                                   "WHERE Screenings.MovieId = @MovieId AND Screenings.ScreeningTime = @ScreeningTime";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    command.Parameters.AddWithValue("@ScreeningTime", screeningTime);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        hallCapacity = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting hall capacity: {ex.Message}");
            }

            return hallCapacity;
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










    }
}

