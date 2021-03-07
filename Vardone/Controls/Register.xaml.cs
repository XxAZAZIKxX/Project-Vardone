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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : UserControl
    {
        public Register()
        {
            InitializeComponent();

        }
        private void hlme(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            hyplin.Foreground = (Brush)bc.ConvertFrom("#8f34eb");

            new Main().ShowDialog();
        }
        private void hlml(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            hyplin.Foreground = (Brush)bc.ConvertFrom("#34ebe5");
        }
    }
}
