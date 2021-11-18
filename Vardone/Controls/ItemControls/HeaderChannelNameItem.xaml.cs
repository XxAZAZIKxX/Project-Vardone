using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for HeaderChannelNameItem.xaml
    /// </summary>
    public partial class HeaderChannelNameItem
    {
        public HeaderChannelNameItem(Channel channel)
        {
            InitializeComponent();
            ChannelNameLabel.Content = channel.Name;
        }
    }
}
