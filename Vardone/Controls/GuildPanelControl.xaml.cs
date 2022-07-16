using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Vardone.Controls.Items;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.Popup;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls
{
    /// <summary>
    ///     Interaction logic for GuildPanelControl.xaml
    /// </summary>
    public partial class GuildPanelControl
    {
        private static GuildPanelControl _instance;
        public static GuildPanelControl GetInstance() => _instance ??= new GuildPanelControl();
        public static void ClearInstance() => _instance = null;
        private GuildPanelControl() => InitializeComponent();

        public Guild CurrentGuild { get; private set; }

        public void UpdateGuild(Guild guild)
        {
            if (guild is null || CurrentGuild is null) return;
            if (guild.GuildId != CurrentGuild.GuildId) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                GuildName.Text = guild.Name;
                AvatarsWorker.UpdateGuildAvatar(guild.GuildId);
                GuildAvatar.Source = AvatarsWorker.GetGuildAvatar(guild.GuildId);
            }, DispatcherPriority.Background);
        }

        private enum ViewButtonPermission
        {
            Owner, Member
        }
        private void SetButtons(ViewButtonPermission viewButtonPermission)
        {
            switch (viewButtonPermission)
            {
                case ViewButtonPermission.Owner:
                    SettingsButton.Visibility = ContextMenu.Visibility = Visibility.Visible;
                    Resources["InviteButtonDock"] = null;
                    break;
                case ViewButtonPermission.Member:
                    SettingsButton.Visibility = ContextMenu.Visibility = Visibility.Collapsed;
                    Resources["InviteButtonDock"] = Dock.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewButtonPermission), viewButtonPermission, null);
            }
        }
        private void LoadChannelsList()
        {
            Application.Current.Dispatcher.Invoke(() => ChannelsList.Children.Clear());
            var guild = MainPage.Client.GetGuilds().FirstOrDefault(p => p.GuildId == CurrentGuild.GuildId);
            var channels = guild?.Channels;
            if (channels is null) return;
            var ownerId = CurrentGuild.Owner.User.UserId;
            foreach (var channel in channels)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    var activeContextMenu = ownerId == MainPage.Client.GetMe().UserId ? GuildChannelItem.ActiveContextMenu.Active : GuildChannelItem.ActiveContextMenu.Disable;
                    ChannelsList.Children.Add(new GuildChannelItem(channel, activeContextMenu));
                }, DispatcherPriority.Background);
            }
        }
        private void SetGuild()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                GuildName.Text = CurrentGuild.Name;
                GuildAvatar.Source = AvatarsWorker.GetGuildAvatar(CurrentGuild.GuildId);
                LoadChannelsList();
                SetButtons(CurrentGuild.Owner.User.UserId == MainPage.Client.GetMe().UserId
                    ? ViewButtonPermission.Owner
                    : ViewButtonPermission.Member);
            }, DispatcherPriority.Render);
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
                CurrentGuild = guild;
                SetGuild();
            }
        }

        public void AddChannel(Channel channel)
        {
            if (channel.Guild.GuildId != CurrentGuild.GuildId) return;
            var uid = MainPage.Client.GetMe().UserId;
            var type = uid == CurrentGuild.Owner.User.UserId
                ? GuildChannelItem.ActiveContextMenu.Active
                : GuildChannelItem.ActiveContextMenu.Disable;

            Application.Current.Dispatcher.BeginInvoke(() => ChannelsList.Children.Add(new GuildChannelItem(channel, type)), DispatcherPriority.Background);
            CurrentGuild = MainPage.Client.GetGuilds().FirstOrDefault(p => p.GuildId == CurrentGuild.GuildId);
        }

        public void RemoveChannel(Channel channel)
        {
            if (channel.Guild.GuildId != CurrentGuild.GuildId) return;
            if (ChatControl.GetInstance().Channel?.ChannelId == channel.ChannelId) ChatControl.GetInstance().CloseChat();
            Application.Current.Dispatcher.Invoke(() =>
            {
                var guildChannelItems = ChannelsList.Children.Cast<GuildChannelItem>().ToList();
                var item = guildChannelItems.FirstOrDefault(p => p.Channel.ChannelId == channel.ChannelId);
                if (item is null) return;
                ChannelsList.Children.RemoveAt(guildChannelItems.IndexOf(item));
            });
        }

        public void UpdateChannel(Channel channel)
        {
            if (channel.Guild.GuildId != CurrentGuild.GuildId) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var item = ChannelsList.Children.Cast<GuildChannelItem>().FirstOrDefault(p => p.Channel.ChannelId == channel.ChannelId);
                if (item is null) return;
                item.UpdateChannel(channel);
            });
        }

        //
        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(GuildPropertiesPage.GetInstance().LoadGuild(CurrentGuild));
        private void ContextMenuNewChannelButtonClicked(object sender, RoutedEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(EditChannelNamePage.GetInstance().Load(new Channel { Guild = CurrentGuild }, EditChannelNamePage.ActionType.Create));
        private void OpenGuildMemberControl(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(GuildMembersPage.GetInstance().Load(CurrentGuild));
    }
}