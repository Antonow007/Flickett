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
    /// Interaction logic for SelectTickets.xaml
    /// </summary>
    public partial class SelectTickets : Window
    {
        string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
        private MovieViewModel movie;
        private string screeningTime;
        private int totalQuantity = 0;

        public SelectTickets(MovieViewModel movie, string screeningTime)
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.movie = movie;
            this.screeningTime = screeningTime;
            DataContext = movie;

            movie.ScreeningTime = screeningTime;
            movie.ScreeningDate = DateTime.Today.ToString("yyyy-MM-dd");

            (int hallId, int screeningId) = GetScheduleDetails(movie.MovieId, screeningTime);

            (decimal basePrice, string hallName) = GetBasePriceAndHallNameForHall(hallId);
            movie.basePrice = basePrice;
            movie.HallName = hallName;

        }




        private (int, int) GetScheduleDetails(string movieId, string screeningTime)
        {
            int hallId = 0;
            int screeningId = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string screeningDate = DateTime.Today.ToString("yyyy-MM-dd");

                    string query = "SELECT HallId, ScreeningId FROM Screenings WHERE MovieId = @MovieId AND ScreeningTime = @ScreeningTime AND ScreeningDate = @ScreeningDate";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MovieId", movieId);
                    command.Parameters.AddWithValue("@ScreeningTime", screeningTime);
                    command.Parameters.AddWithValue("@ScreeningDate", screeningDate);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            hallId = reader.GetInt32("HallId");
                            screeningId = reader.GetInt32("ScreeningId");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting schedule details: {ex.Message}");
            }

            return (hallId, screeningId);
        }

        private string GetHallNameForHall(int hallId)
        {
            string hallName = string.Empty;
            string hallQuery = "SELECT HallName FROM Halls WHERE HallId = @HallId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand hallCommand = new MySqlCommand(hallQuery, connection);
                    hallCommand.Parameters.AddWithValue("@HallId", hallId);
                    object hallResult = hallCommand.ExecuteScalar();

                    if (hallResult != null)
                    {
                        hallName = hallResult.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error getting hall name: {ex.Message}");
                }
            }

            return hallName;
        }


        private (decimal, string) GetBasePriceAndHallNameForHall(int hallId)
        {
            string ticketTypeQuery = "SELECT TicketTypeId FROM Halls WHERE HallId = @HallId";
            string basePriceQuery = "SELECT BasePrice FROM TicketTypes WHERE TicketTypeId = @TicketTypeId";

            int ticketTypeId = 0;
            decimal basePrice = 0;
            string hallName = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                 
                    MySqlCommand ticketTypeCommand = new MySqlCommand(ticketTypeQuery, connection);
                    ticketTypeCommand.Parameters.AddWithValue("@HallId", hallId);
                    object ticketTypeResult = ticketTypeCommand.ExecuteScalar();
                    ticketTypeId = Convert.ToInt32(ticketTypeResult);

                 
                    MySqlCommand basePriceCommand = new MySqlCommand(basePriceQuery, connection);
                    basePriceCommand.Parameters.AddWithValue("@TicketTypeId", ticketTypeId);
                    object basePriceResult = basePriceCommand.ExecuteScalar();
                    basePrice = Convert.ToDecimal(basePriceResult);

                   
                    hallName = GetHallNameForHall(hallId);

                    return (basePrice, hallName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error getting base price and hall name: {ex.Message}");
                    return (0, string.Empty); 
                }
            }
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


        int regularQuantity = 0;
        int kidsQuantity = 0;
        int adultQuantity = 0;
        int studentQuantity = 0;

        private void RegularTicketQuantityBoxPlus_Click(object sender, RoutedEventArgs e)
        {
            if (regularQuantity > 0)
            {
                regularQuantity--;
                RegularTicketQuantityBox.Text = $"{regularQuantity}";
                UpdateTotalQuantity();
            }



        }

        private void RegularTicketQuantityBoxMinus_Click(object sender, RoutedEventArgs e)
        {
            if (regularQuantity < 10)
            {
                regularQuantity++;
                RegularTicketQuantityBox.Text = $"{regularQuantity}";
                UpdateTotalQuantity();
            }


        }

        private void KidsTicketQuantityBoxPlus_Click(object sender, RoutedEventArgs e)
        {
            if (kidsQuantity > 0)
            {
                kidsQuantity--;
                KidsTicketQuantityBox.Text = $"{kidsQuantity}";
                UpdateTotalQuantity();
            }

        }

        private void KidsTicketQuantityBoxMinus_Click(object sender, RoutedEventArgs e)
        {
            if (kidsQuantity < 10)
            {
                kidsQuantity++;
                KidsTicketQuantityBox.Text = $"{kidsQuantity}";
                UpdateTotalQuantity();
            }
        }

        private void AdultTicketQuantityBoxPlus_Click(object sender, RoutedEventArgs e)
        {
            if (adultQuantity > 0)
            {
                adultQuantity--;
                AdultTicketQuantityBox.Text = $"{adultQuantity}";
                UpdateTotalQuantity();
            }
        }

        private void AdultTicketQuantityBoxMinus_Click(object sender, RoutedEventArgs e)
        {
            if (adultQuantity < 10)
            {
                adultQuantity++;
                AdultTicketQuantityBox.Text = $"{adultQuantity}";
                UpdateTotalQuantity();
            }
        }

        private void StudentTicketQuantityBoxPlus_Click(object sender, RoutedEventArgs e)
        {
            if (studentQuantity > 0)
            {
                studentQuantity--;
                StudentTicketQuantityBox.Text = $"{studentQuantity}";
                UpdateTotalQuantity();
            }


        }

        private void StudentTicketQuantityBoxMinus_Click(object sender, RoutedEventArgs e)
        {
            if (studentQuantity < 10)
            {
                studentQuantity++;
                StudentTicketQuantityBox.Text = $"{studentQuantity}";
                UpdateTotalQuantity();
            }
        }

        private void UpdateTotalQuantity()
        {
            totalQuantity = regularQuantity + kidsQuantity + adultQuantity + studentQuantity;
        }

        private void SelectTicketBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainPage main = new MainPage();
            main.Show();
            this.Close();
        }

        private void SelectTicketNextButton_Click(object sender, RoutedEventArgs e)
        {
            string username = "";
            SelectSeats seats = new SelectSeats(movie, screeningTime, totalQuantity);
            seats.Show();
            this.Close();
        }



       


    }

}
