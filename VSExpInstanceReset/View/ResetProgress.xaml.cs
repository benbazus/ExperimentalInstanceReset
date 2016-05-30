using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VSExpInstanceReset.View
{
    public partial class ResetProgressView : Window
    {
        public ResetProgressView()
        {
            InitializeComponent();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            Close();
        }


    }
}
