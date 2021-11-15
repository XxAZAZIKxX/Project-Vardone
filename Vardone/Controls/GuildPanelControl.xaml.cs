using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Internal;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.Popup;
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

        public Guild currentGuild;

        private void SetGuild()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildName.Text = currentGuild.Name;
                GuildAvatar.Source = AvatarsWorker.GetGuildAvatar(currentGuild.GuildId);
                ChannelsList.Children.Clear();
                if (currentGuild.Channels is not null) foreach (var guildChannel in currentGuild.Channels) ChannelsList.Children.Add(new GuildChannelItem(guildChannel));
                
                SettingsButton.Visibility = currentGuild.Owner.UserId == MainPage.Client.GetMe().UserId
                    ? Visibility.Visible
                    : Visibility.Collapsed;
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
                currentGuild = guild;
                SetGuild();
            }
        }

        public void UpdateChannelsList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChannelsList.Children.Clear();
                var channels = MainPage.Client.GetGuilds().FirstOr(p => p.GuildId == currentGuild.GuildId, null!)?.Channels;
                if (channels is null) return;
                foreach (var channel in channels) ChannelsList.Children.Add(new GuildChannelItem(channel));
            });
        }

        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e)
        {
            GuildProperties.GetInstance().LoadGuild(currentGuild);
            MainPage.GetInstance().MainFrame.Navigate(GuildProperties.GetInstance());
        }

        private void ContextMenuGrid(object sender, MouseButtonEventArgs e)
        {
            //cm.IsEnabled = false;
            //cm.IsOpen = false;
        }

        private void NewChannel(object sender, RoutedEventArgs e)
        {

        }

        private void OpenGuildMemberControl(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(AddMember.GetInstance());

    }
}
