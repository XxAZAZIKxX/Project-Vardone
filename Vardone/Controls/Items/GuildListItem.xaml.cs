using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
   
    /// <summary>
    /// Interaction logic for GuildListItem.xaml
    /// </summary>
    public partial class GuildListItem
    {
        private static readonly List<GuildListItem> Items = new();
        public readonly Guild guild;
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
        public GuildListItem([NotNull] Guild guild)
        {
            InitializeComponent();
            this.guild = guild;
            Avatar.ImageSource = AvatarsWorker.GetGuildAvatar(this.guild.GuildId);
            Items.Add(this);
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
    }
}
