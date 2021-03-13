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

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для PropertiesPage.xaml
    /// </summary>
    /// private static MainPage _instance;
    public partial class PropertiesPage : Page
    {
        private static PropertiesPage _instance;
        public static PropertiesPage GetInstance() => _instance ??= new PropertiesPage(); 
        private PropertiesPage()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);
    }
}
