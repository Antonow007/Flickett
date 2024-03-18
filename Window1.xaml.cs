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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animation = new ThicknessAnimation();
            animation.From = new Thickness(0, 0, 0, 0); // Start from original position
            animation.To = new Thickness(200, 0, 0, 0); // Adjust this value based on how much you want to slide
            animation.Duration = TimeSpan.FromSeconds(0.4); // Duration of the animation

            // Set the target property to animate (in this case, the Grid's margin)
            myGrid.BeginAnimation(Grid.MarginProperty, animation);
        }
    }
}

