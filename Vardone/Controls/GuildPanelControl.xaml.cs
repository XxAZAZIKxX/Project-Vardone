using System;
using System.Windows;
using System.Windows.Input;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for GuildPanelControl.xaml
    /// </summary>
    public partial class GuildPanelControl
    {
        private static GuildPanelControl _instance;
        public static GuildPanelControl GetInstance() => _instance ??= new GuildPanelControl();
        private GuildPanelControl() => InitializeComponent();

        private Guild _guild;

        private void SetGuild()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildName.Text = _guild.Name;
                if (_guild.Base64Avatar is not null) GuildAvatar.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(_guild.Base64Avatar));
                ChannelsList.Children.Clear();
                foreach (var guildChannel in _guild.Channels) ChannelsList.Children.Add(new GuildChannelItem(guildChannel));
            });
        }

        public void ChangeGuild(Guild guild)
        {
            if (guild is null)
            {
                GuildName.Text = string.Empty;
                GuildAvatar.Source = null;
                ChannelsList.Children.Clear();
            }
            else
            {
                _guild = guild;
                SetGuild();
            }
        }

        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(GuildProperties.GetInstance());
        }
    }
}
