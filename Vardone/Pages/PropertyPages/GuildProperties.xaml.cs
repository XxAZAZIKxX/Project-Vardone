using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Interaction logic for GuildProperties.xaml
    /// </summary>
    public partial class GuildProperties
    {
        private static GuildProperties _instance;
        public static GuildProperties GetInstance() => _instance ??= new GuildProperties();
        public GuildProperties() => InitializeComponent();

        private void Change_Avatar(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
        }
    }
}
