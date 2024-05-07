using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using static Flickett.MainPage;
using System.Collections;

namespace Flickett
{

    public partial class MainPage : Window
    {

        private const string ApiKey = "186d8bc2a505456d116a65559e5d0788";
        private string userRole;



        public MainPage(string role)
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
            this.userRole = role;
            bool isAdmin = (userRole == "admin");
            if (isAdmin)
            {
                LoadMovies();
            }
            else
            {
                LoadMoviesFromDb();
            }
            DataContext = this;

        }





        private async void LoadMovies()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
                    string nowPlayingEndpoint = "movie/now_playing";

                    HttpResponseMessage response = await client.GetAsync($"{nowPlayingEndpoint}?api_key={ApiKey}&language=en-US&page=1");

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);

                        List<MovieViewModel> movies = new List<MovieViewModel>();

                        var movieTasks = new List<Task<MovieViewModel>>();

                        foreach (dynamic movieData in data.results)
                        {
                            movieTasks.Add(GetMovieDetails(client, movieData));
                        }

                        var movieResults = await Task.WhenAll(movieTasks);

                        foreach (var movie in movieResults)
                        {
                            movie.IsAdmin = true;
                            movies.Add(movie);
                        }

                        // Bind the movie data to the ItemsControl
                        MovieItemsControl.ItemsSource = movies;
                        MovieItemsControl.DataContext = this; // Set DataContext to the instance of MainPage
                    }
                    else
                    {
                        MessageBox.Show("Error loading movies. Please try again later.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}");
            }
        }

        public class MovieViewModel
        {
            public string MovieId { get; set; }
            public string Title { get; set; }
            public string Overview { get; set; }
            public string PosterUrl { get; set; }
            public string Genre { get; set; }
            public int Duration { get; set; }
            public List<string> ScreeningTimes { get; set; }
            public string GenreWithDuration => $"{Duration} min | {Genre}";
            public bool IsAdmin { get; set; }




        }


        private async Task<MovieViewModel> GetMovieDetails(HttpClient client, dynamic movieData)
        {
            string id = movieData.id;
            string title = movieData.title;
            string overview = movieData.overview;
            string posterUrl = $"https://image.tmdb.org/t/p/w500{movieData.poster_path}";


            HttpResponseMessage response = await client.GetAsync($"movie/{id}?api_key={ApiKey}&language=en-US");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic movieDetails = JsonConvert.DeserializeObject(json);
                int duration = movieDetails.runtime;

                string genre = ""; // Initialize genre as empty string
                foreach (dynamic genreData in movieDetails.genres)
                {
                    genre += genreData.name + ", "; // Concatenate genre names
                }
                genre = genre.TrimEnd(',', ' '); // Remove trailing comma and space

                return new MovieViewModel
                {
                    MovieId = id,
                    Title = title,
                    Overview = overview,
                    PosterUrl = posterUrl,
                    Duration = duration,
                    Genre = genre,
                };
            }
            else
            {
                throw new Exception($"Failed to fetch details for movie with ID {id}.");
            }
        }



        private async Task AddMovieToDatabase(MovieViewModel movie)
        {
            string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO Movies (ApiMovieId, Title, Overview, PosterUrl, Genre, Duration) VALUES (@ApiMovieId, @Title, @Overview, @PosterUrl, @Genre, @Duration)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ApiMovieId", movie.MovieId);
                    command.Parameters.AddWithValue("@Title", movie.Title);
                    command.Parameters.AddWithValue("@Overview", movie.Overview);
                    command.Parameters.AddWithValue("@PosterUrl", movie.PosterUrl);
                    command.Parameters.AddWithValue("@Genre", movie.Genre);
                    command.Parameters.AddWithValue("@Duration", movie.Duration);



                    await command.ExecuteNonQueryAsync();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error saving movie to database: {ex.Message}");
            }
        }

        private async void AddMovieButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked button
            Button clickedButton = sender as Button;
            if (clickedButton == null)
                return;




            // Get the MovieViewModel object corresponding to the clicked button
            MovieViewModel movieToAdd = clickedButton.DataContext as MovieViewModel;
            if (movieToAdd == null)
                return;

            try
            {
                // Call method to save the movie to the database
                await AddMovieToDatabase(movieToAdd);

                MessageBox.Show("Movie added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving movie to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MovieSchedule_Click(object sender, RoutedEventArgs e)
        {


            Button clickedButton = sender as Button;
            if (clickedButton == null)
                return;

            MovieViewModel movieToAdd = clickedButton.DataContext as MovieViewModel;
            if (movieToAdd == null)
                return;

            SchedulePanel schPanel = new SchedulePanel(movieToAdd);
            schPanel.Show();
            this.Close();


        }


        private void LoadMoviesFromDb()
        {
            try
            {
                string connectionString = "server=localhost;uid=root;pwd=Antonow7;database=cinemadb;SslMode=None;";
                List<MovieViewModel> movies = new List<MovieViewModel>();

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = " SELECT m.*, TIME_FORMAT(s.ScreeningTime, '%H:%i') AS ScreeningTime FROM Movies m    INNER JOIN Screenings s ON m.ApiMovieId = s.MovieId    WHERE DATE(s.ScreeningDate) = CURRENT_DATE()";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string id = reader["ApiMovieId"].ToString();
                        string title = reader["Title"].ToString();
                        string overview = reader["Overview"].ToString();
                        string posterUrl = reader["PosterUrl"].ToString();
                        int duration = Convert.ToInt32(reader["Duration"]);
                        string genre = reader["Genre"].ToString();
                        string screeningTime = reader["ScreeningTime"].ToString();

                        
                        var movie = movies.FirstOrDefault(m => m.MovieId == id);
                        if (movie == null)
                        {
                            movie = new MovieViewModel
                            {
                                MovieId = id,
                                Title = title,
                                Overview = overview,
                                PosterUrl = posterUrl,
                                Duration = duration,
                                Genre = genre,
                                IsAdmin = false,
                                ScreeningTimes = new List<string>()
                            };
                            movies.Add(movie);
                        }

                       
                        movie.ScreeningTimes.Add(screeningTime);
                    }

                    reader.Close();
                }

                
                MovieItemsControl.ItemsSource = movies;
                MovieItemsControl.DataContext = this; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies from database: {ex.Message}");
            }
        }














        private void MoveSliderMenuToRight()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 0, 0, 0);
            animation.To = new Thickness(200, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            animation.EasingFunction = new QuadraticEase();

            SlideMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void MoveSliderMenuToLeft()
        {
            if (SlideMenu.Margin.Left == 200)
            {
                ThicknessAnimation animation = new ThicknessAnimation();
                animation.From = new Thickness(200, 0, 0, 0);
                animation.To = new Thickness(0, 0, 0, 0);
                animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
                animation.EasingFunction = new QuadraticEase();

                SlideMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
            }

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

        private void MoviePoster_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                var movieId = button.Tag; // Retrieve the movie ID from the Tag property
                                          // Perform actions based on the clicked movie
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