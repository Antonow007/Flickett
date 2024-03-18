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
    /// <summary>
    /// Interaction logic for lgnWindow.xaml
    /// </summary>
    public partial class lgnWindow : Window
    {
        public lgnWindow()
        {
            InitializeComponent();
        }

       

        private void MoveSliderToLeft()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(400, 0, 0, 0);
            animation.To = new Thickness(0, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            Slider.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void MoveSliderToRight()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 0, 0, 0);
            animation.To = new Thickness(400, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            Slider.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void SignUP_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToLeft();
        }

        private void SignInRegister_Click(object sender, RoutedEventArgs e)
        {
            MoveSliderToRight();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
           
            WindowState = WindowState.Minimized;
        }
    }
}
