using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{

    /// <summary>
    /// Interaction logic for GuildListItem.xaml
    /// </summary>
    public partial class GuildListItem
    {
        private static readonly List<GuildListItem> Items = new();
        public Guild guild;
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                foreach (var item in Items.Where(item => item.IsActive && item != this)) item.IsActive = false;

                GuildHover.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _isActive = value;
            }
        }
        public GuildListItem(Guild guild)
        {
            InitializeComponent();
            this.guild = guild;
            Avatar.ImageSource = AvatarsWorker.GetGuildAvatar(this.guild.GuildId);
            Items.Add(this);
            var currentUserId = MainPage.Client.GetMe().UserId;

            if (currentUserId != guild.Owner.User.UserId) SettingsButton.Visibility = Visibility.Collapsed;
            else LeaveGuildButton.Visibility = Visibility.Collapsed;
        }


        ~GuildListItem() => Items.Remove(this);


        private void AvatarClicked(object sender, MouseButtonEventArgs e)
        {
            GuildHover.Visibility = Visibility.Visible;
            MainPage.GetInstance().OpenGuild(guild);
            IsActive = true;
        }

        private void GuildButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsActive) GuildHover.Visibility = Visibility.Visible;
        }

        private void GuildButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsActive) GuildHover.Visibility = Visibility.Hidden;
        }

        public static void ClearAllHovers()
        {
            if (Items is null) return;
            foreach (var item in Items) item.IsActive = false;
        }

        private void LeaveGuildButtonClicked(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы точно хотите покинуть сервер \"{guild.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            MainPage.Client.LeaveGuild(guild.GuildId);
            MainPage.GetInstance().LoadGuilds();
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(GuildPropertiesPage.GetInstance().LoadGuild(guild));
            AvatarClicked(null, null);
        }

        private void InviteMembersButtonClicked(object sender, RoutedEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(GuildMembersPage.GetInstance().Load(guild));
            AvatarClicked(null, null);
        }
    }
}
