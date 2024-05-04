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

namespace Flickett
{
    public partial class SchedulePanel : Window
    {
        public SchedulePanel()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
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

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            lgnWindow Login = new lgnWindow();
            Login.Show();
            this.Close();
        }
    }
}
