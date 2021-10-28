using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for GuildItem.xaml
    /// </summary>
    public partial class GuildItem
    {
        private static readonly List<GuildItem> Items = new();
        private readonly Guild _guild;
        private bool _isActive;

        private bool IsActive
        {
            get => _isActive;
            set
            {
                foreach (var item in Items.Where(item => item.IsActive && item != this)) item.IsActive = false;

                GuildHover.Visibility = value ? Visibility.Visible : Visibility.Hidden;
                _isActive = value;
            }
        }
        public GuildItem([NotNull] Guild guild)
        {
            InitializeComponent();
            _guild = guild;
            Avatar.ImageSource = AvatarsWorker.GetGuildAvatar(_guild.GuildId);
            Items.Add(this);
        }


        ~GuildItem() => Items.Remove(this);


        private void AvatarClicked(object sender, MouseButtonEventArgs e)
        {
            GuildHover.Visibility = Visibility.Visible;
            MainPage.GetInstance().OpenGuild(_guild);
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
    }
}
