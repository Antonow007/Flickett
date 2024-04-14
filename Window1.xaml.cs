using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private const string ApiKey = "186d8bc2a505456d116a65559e5d0788"; // Replace with your TMDb API key

        public Window1()
        {
            InitializeComponent();
            LoadMovies();
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

                        foreach (dynamic movieData in data.results)
                        {
                            string title = movieData.title;
                            string overview = movieData.overview;
                            string posterUrl = $"https://image.tmdb.org/t/p/w500{movieData.poster_path}";
                            int? runtime = movieData.runtime; // Note the use of int? for nullable int

                            // Check if runtime is null before assigning
                            int actualRuntime = runtime ?? 0; // Default to 0 if runtime is null

                            AddMovieItem(title, overview, posterUrl, actualRuntime);
                        }
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

        private void AddMovieItem(string title, string overview, string posterUrl, int runtime)
        {
            // Create movie item dynamically
            Border border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                Margin = new Thickness(5)
            };

            StackPanel stackPanel = new StackPanel();

            TextBlock titleTextBlock = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold
            };
            stackPanel.Children.Add(titleTextBlock);

            TextBlock runtimeTextBlock = new TextBlock
            {
                Text = $"Runtime: {runtime} min",
                FontWeight = FontWeights.Bold
            };
            stackPanel.Children.Add(runtimeTextBlock);

            TextBlock overviewTextBlock = new TextBlock
            {
                Text = overview,
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(overviewTextBlock);

            Image posterImage = new Image
            {
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(posterUrl)),
                Width = 150,
                Height = 220
            };
            stackPanel.Children.Add(posterImage);

            border.Child = stackPanel;
            MoviePanel.Children.Add(border);
        }
    }
}





 