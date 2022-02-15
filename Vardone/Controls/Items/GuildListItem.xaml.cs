using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        private static readonly List<GuildListItem> GuildItems = new();
        public Guild Guild { get; private set; }
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                foreach (var item in GuildItems.Where(item => item.IsActive && item != this)) item.IsActive = false;

                GuildHover.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _isActive = value;
            }
        }
        public GuildListItem(Guild guild)
        {
            InitializeComponent();
            this.Guild = guild;
            Avatar.ImageSource = AvatarsWorker.GetGuildAvatar(this.Guild.GuildId);
            GuildItems.Add(this);
            var currentUserId = MainPage.Client.GetMe().UserId;

            if (currentUserId != guild.Owner.User.UserId) SettingsButton.Visibility = Visibility.Collapsed;
            else LeaveGuildButton.Visibility = Visibility.Collapsed;
        }

        ~GuildListItem() => GuildItems.Remove(this);

        public void UpdateGuild(Guild guild)
        {
            if (Guild.GuildId != guild.GuildId) return;
            Guild = guild;
            AvatarsWorker.UpdateGuildAvatar(guild.GuildId);
            Application.Current.Dispatcher.BeginInvoke(() => Avatar.ImageSource = AvatarsWorker.GetGuildAvatar(guild.GuildId), DispatcherPriority.Background);
        }

        private void AvatarClicked(object sender, MouseButtonEventArgs e)
        {
            GuildHover.Visibility = Visibility.Visible;
            MainPage.GetInstance().OpenGuild(Guild);
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
            if (GuildItems is null) return;
            foreach (var item in GuildItems) item.IsActive = false;
        }

        private void LeaveGuildButtonClicked(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы точно хотите покинуть сервер \"{Guild.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult != MessageBoxResult.Yes) return;

            MainPage.Client.LeaveGuild(Guild.GuildId);
        }

        private void SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(GuildPropertiesPage.GetInstance().LoadGuild(Guild));
            AvatarClicked(null, null);
        }

        private void InviteMembersButtonClicked(object sender, RoutedEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(GuildMembersPage.GetInstance().Load(Guild));
            AvatarClicked(null, null);
        }
    }
}
