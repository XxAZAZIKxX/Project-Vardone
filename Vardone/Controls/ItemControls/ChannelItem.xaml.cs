using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for ChannelItem.xaml
    /// </summary>
    public partial class ChannelItem
    {
        public ChannelItem(Channel channel)
        {
            InitializeComponent();
            ChannelNameLabel.Content = channel.Name;
        }
    }
}
