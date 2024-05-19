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
using static Flickett.lgnWindow;
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
        private int totalQuantity;
        private int screeningId;

        public SelectSeats(MovieViewModel movie, string screeningTime ,int totalQuantity)
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.movie = movie;
            this.screeningTime = screeningTime;
            this.totalQuantity = totalQuantity;
            int hallCapacity = GetHallCapacity(movie.MovieId, screeningTime);
            AddSeatsToGrid(hallCapacity);
            
           
            (int hallId, int screeningId) = GetScheduleDetails(movie.MovieId, screeningTime); 
            this.screeningId = screeningId;

            TakenSeats();
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


        private void AddSeatsToGrid(int hallCapacity)
        {
            int maxSeatsPerRow = 11; 
            int seatsAdded = 0; 

           
            for (int row = 10; row >= 2; row--)
            {
                int seatsInThisRow = Math.Min(maxSeatsPerRow, hallCapacity - seatsAdded); 
                int seatNumber = 1; 

                for (int col = 2; col <= seatsInThisRow; col++)
                {
                    Button seatButton = new Button();
                    seatButton.Content = $"{seatNumber}"; 
                    seatButton.Style = FindResource("RoundedButtonStyle") as Style; 
                    seatButton.Click += SeatButton_Click;

                    Grid.SetRow(seatButton, row);
                    Grid.SetColumn(seatButton, col);

                   
                    SeatsLayout.Children.Add(seatButton);

                    seatsAdded++;
                    seatNumber++;

                    if (seatsAdded >= hallCapacity)
                    {
                        break; 
                    }
                }

                if (seatsAdded >= hallCapacity)
                {
                    break; 
                }
            }
        }




        private int selectedSeatsCount = 0;

        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSeatsCount < totalQuantity)
            {
                Button clickedButton = sender as Button;
                if (clickedButton != null)
                {
                    clickedButton.Background = Brushes.Orange;

                    string buttonText = clickedButton.Content.ToString();
                    int seatRow = Grid.GetRow(clickedButton);

                    string realSeatRow = null;
                    foreach (UIElement element in SeatsLayout.Children)
                    {
                        if (element is TextBlock textBlock && Grid.GetRow(textBlock) == seatRow)
                        {
                            realSeatRow = textBlock.Text;
                            break;
                        }
                    }

                   
                    selectedSeatsCount++;
                }
            }
            else
            {
                MessageBox.Show("You have already selected the maximum number of seats.");
            }
        }

        private void TakenSeats()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "Select SeatNumber, RowNumber from tickets where ScreeningId=@ScreeningId";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ScreeningId", screeningId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int seatNumber = reader.GetInt32("SeatNumber");
                            int rowNumber = reader.GetInt32("RowNumber");

                            // Намиране на бутона, представляващ седалката по нейните координати в Grid
                            Button takenSeatButton = SeatsLayout.Children
                                .OfType<Button>()
                                .FirstOrDefault(btn =>
                                    Grid.GetRow(btn) == rowNumber &&
                                    Grid.GetColumn(btn) == seatNumber);

                            // Оцветяване в червено и забрана за кликване
                            if (takenSeatButton != null)
                            {
                                takenSeatButton.Background = Brushes.Red;
                                takenSeatButton.IsEnabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error marking taken seats: {ex.Message}");
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

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            string userId = GlobalVariables.UserId;

           
            if (selectedSeatsCount == 0)
            {
                MessageBox.Show("Please select at least one seat before proceeding to payment.");
                return;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                   
                    foreach (UIElement element in SeatsLayout.Children)
                    {
                        if (element is Button seatButton && seatButton.Background == Brushes.Orange)
                        {
                            string buttonText = seatButton.Content.ToString();
                            int seatRow = Grid.GetRow(seatButton);

                          
                            string realSeatRow = null;
                            foreach (UIElement elem in SeatsLayout.Children)
                            {
                                if (elem is TextBlock textBlock && Grid.GetRow(textBlock) == seatRow)
                                {
                                    realSeatRow = textBlock.Text;
                                    break;
                                }
                            }

                          
                            string query = "INSERT INTO Tickets (UserId, ScreeningId, MovieId, SeatNumber, RowNumber) VALUES (@UserId, @ScreeningId, @MovieId, @SeatNumber, @RowNumber)";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@ScreeningId", screeningId);
                            command.Parameters.AddWithValue("@MovieId",movie.MovieId);
                            command.Parameters.AddWithValue("@SeatNumber", buttonText);
                            command.Parameters.AddWithValue("@RowNumber", realSeatRow);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Tickets successfully purchased!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error purchasing tickets: {ex.Message}");
            }
        }

    }
}


