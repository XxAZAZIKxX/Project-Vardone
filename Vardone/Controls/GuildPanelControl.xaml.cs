using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Vardone.Controls.ItemControls;
using Vardone.Core;
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

        public Guild guild;

        private void SetGuild()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildName.Text = guild.Name;
                if (guild.Base64Avatar is not null) GuildAvatar.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(guild.Base64Avatar));
                ChannelsList.Children.Clear();
                foreach (var guildChannel in guild.Channels) ChannelsList.Children.Add(new GuildChannelItem(guildChannel));
            });
        }

        public void ChangeGuild([NotNull] Guild guild)
        {
            this.guild = guild;
            SetGuild();
        }

        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
