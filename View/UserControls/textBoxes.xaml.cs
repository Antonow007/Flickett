using System.Windows;
using System.Windows.Controls;

namespace Flickett.View.UserControls
{
    /// <summary>
    /// Interaction logic for textBoxes.xaml
    /// </summary>
    public partial class textBoxes : UserControl
    {


        public textBoxes()
        {
            InitializeComponent();
        }



        private string placeholder;

        public string Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;
                tbPlaceholder.Text = placeholder;
            }
        }






        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
            }
        }
    }
}
