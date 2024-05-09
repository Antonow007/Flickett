using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static Flickett.MainPage;

namespace Flickett
{
    public partial class SchedulePanel : Window
    {

        private MovieViewModel movieData;

        public SchedulePanel(MovieViewModel movieData)
        {
            InitializeComponent();
          
            this.movieData = movieData;

            this.MouseDown += Window_MouseDown;
            ShowHallDAta();

            for (int i = 0; i < 24; i++)
            {
                hoursComboBox.Items.Add(i.ToString("00"));
            }


            for (int i = 0; i < 60; i += 10)
            {
                minutesComboBox.Items.Add(i.ToString("00"));
            }


            hoursComboBox.SelectedIndex = 0;
            minutesComboBox.SelectedIndex = 0;

            TitleTextBlock.Text = movieData.Title;
            OverViewTextBox.Text = movieData.Overview;
            GenresAndDurationTextBox.Text = movieData.GenreWithDuration;

            if (!string.IsNullOrEmpty(movieData.PosterUrl))
            {
                BitmapImage posterImage = new BitmapImage(new Uri(movieData.PosterUrl));
                PosterBox.Fill = new ImageBrush(posterImage);
            }
        }

       
        public TimeSpan GetSelectedTime()
        {
            int hours = int.Parse(hoursComboBox.SelectedItem.ToString());
            int minutes = int.Parse(minutesComboBox.SelectedItem.ToString());
            return new TimeSpan(hours, minutes, 0);
        }

        private void ShowHallDAta()
        {
            
            string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = "SELECT HallName FROM Halls";

                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string hallName = reader["HallName"].ToString();
                        SelectHall.Items.Add(hallName);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
  
        private void AddScheduleButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                
                string selectedMovieId = movieData.MovieId;
                DateTime selectedDate = datePicker.SelectedDate ?? DateTime.MinValue;
                TimeSpan selectedTime = GetSelectedTime();
                string selectedHall = SelectHall.SelectedItem as string;

                
                string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
                string query = "INSERT INTO Screenings (MovieId, ScreeningDate, ScreeningTime, HallId) VALUES (@MovieId, @ScreeningDate, @ScreeningTime, @HallId);";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MovieId", selectedMovieId);
                    command.Parameters.AddWithValue("@ScreeningDate", selectedDate);
                    command.Parameters.AddWithValue("@ScreeningTime", selectedTime);
                    command.Parameters.AddWithValue("@HallId", GetHallId(selectedHall));

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Screening added successfully!");
                    }
                    else
                    {
                        MessageBox.Show("Failed to add screening.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }

        private int GetHallId(string hallName)
        {
            int hallId = -1;

            string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
            string query = "SELECT HallId FROM Halls WHERE HallName = @HallName;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@HallName", hallName);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    hallId = Convert.ToInt32(result);
                }
            }

            return hallId;
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

        private void ManageHallsButton_Click(object sender, RoutedEventArgs e)
        {
            ManageHalls halls = new ManageHalls();
            halls.Show();
            this.Close();
        }


    }
}
