﻿using Flickett.View.UserControls;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Flickett
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            UsenameTextBox.txtInput.TextChanged += UsernameTextBox_TextChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FlNames.txtInput.BorderBrush = System.Windows.Media.Brushes.Red;
            UsernameErrorBox.Text = "This Username has aleready taken";
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }
    }
}
