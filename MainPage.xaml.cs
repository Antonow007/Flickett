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

namespace Flickett
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        public MainPage()
        {
            InitializeComponent();
            this.MouseDown += Window_MouseDown;
        }


        private void MoveSliderMenuToRight() 
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(-200, 0, 0, 0);
            animation.To = new Thickness(0, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            SlideMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
        }

        private void MoveSliderMenuToLeft()
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 0, 0, 0);
            animation.To = new Thickness(-200, 0, 0, 0);
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            animation.EasingFunction = new QuadraticEase();

            SlideMenu.BeginAnimation(FrameworkElement.MarginProperty, animation);
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


    }
}
