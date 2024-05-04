using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static Flickett.MainPage;

namespace Flickett
{
    public partial class SchedulePanel : Window
    {

        public SchedulePanel(MovieViewModel movieData)
        {
            InitializeComponent();
            //this.MouseDown += Window_MouseDown;


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

        //private void MoveSliderMenuToRight()
        //{
        //    ThicknessAnimation animation = new ThicknessAnimation();
        //    animation.From = new Thickness(0, 0, 0, 0);
        //    animation.To = new Thickness(200, 0, 0, 0);
        //    animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
        //    animation.EasingFunction = new QuadraticEase();

        //    SlideMenu.BeginAnimation(Grid.MarginProperty, animation);
        //}

        //private void MoveSliderMenuToLeft()
        //{
        //    if (SlideMenu.Margin.Left == 200)
        //    {
        //        ThicknessAnimation animation = new ThicknessAnimation();
        //        animation.From = new Thickness(200, 0, 0, 0);
        //        animation.To = new Thickness(0, 0, 0, 0);
        //        animation.Duration = new Duration(TimeSpan.FromSeconds(0.4));
        //        animation.EasingFunction = new QuadraticEase();

        //        SlideMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
        //    }

        //}

        private void hamburgerButton_Click_1(object sender, RoutedEventArgs e)
        {
            //MoveSliderMenuToRight();

        }

        //private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    if (!IsMouseOverUIElement(SlideMenu, e.GetPosition(this)))
        //    {
        //        MoveSliderMenuToLeft();
        //    }
        //}

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
