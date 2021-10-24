using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Vardone.Pages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for GuildChannelItem.xaml
    /// </summary>
    public partial class GuildChannelItem
    {
        public Channel channel;
        public GuildChannelItem([NotNull] Channel channel)
        {
            InitializeComponent();
            this.channel = channel;
            ChannelName.Text = this.channel.Name;
        }

        private void OpenChannel(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().chatControl.LoadChat(channel);
    }
}
