﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Internal;
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

        public Guild currentGuild;

        private void SetGuild()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildName.Text = currentGuild.Name;
                GuildAvatar.Source = AvatarsWorker.GetGuildAvatar(currentGuild.GuildId);
                UpdateChannelsList();
                SetButtons(currentGuild.Owner.User.UserId == MainPage.Client.GetMe().UserId
                    ? ViewButtonPermission.Owner
                    : ViewButtonPermission.Member);
            });
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
            });
            var guild = MainPage.Client.GetGuilds().FirstOrDefault(p => p.GuildId == currentGuild.GuildId);
            var channels = guild?.Channels;
            if (channels is null) return;
            var ownerId = currentGuild.Owner.User.UserId;
            foreach (var channel in channels)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var activeContextMenu = ownerId == MainPage.Client.GetMe().UserId ? GuildChannelItem.ActiveContextMenu.Active : GuildChannelItem.ActiveContextMenu.Disable;
                    ChannelsList.Children.Add(new GuildChannelItem(channel, activeContextMenu));
                });
            }
        }

        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(GuildPropertiesPage.GetInstance().LoadGuild(currentGuild));

        private void ContextMenuNewChannelButtonClicked(object sender, RoutedEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(EditChannelNamePage.GetInstance().Load(new Channel { Guild = currentGuild }, EditChannelNamePage.ActionType.Create));

        private void OpenGuildMemberControl(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(GuildMembersPage.GetInstance().Load(currentGuild));
    }
}