using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brush = System.Windows.Media.Brush;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for Auth.xaml
    /// </summary>
    public partial class Auth : UserControl
    {
        public Auth() => InitializeComponent();

        private void hlme(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            hyplin.Foreground = (Brush)bc.ConvertFrom("#8f34eb");
        }
        private void hlml(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            hyplin.Foreground = (Brush)bc.ConvertFrom("#34ebe5");
        }  
        private void md_hl(object sender, RoutedEventArgs e)
        {
           
           
        }
    }
}
