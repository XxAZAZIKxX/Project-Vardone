using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for BanReasonPage.xaml
    /// </summary>
    public partial class BanReasonPage
    {
        private static BanReasonPage _instance;
        public static BanReasonPage GetInstance() => _instance ??= new BanReasonPage();

        private string MainLabelText => "Забанить участника " + member?.User.Username;

        public Member member;

        private BanReasonPage() => InitializeComponent();

        public BanReasonPage Load(Member member)
        {
            Reset();
            this.member = member;
            MainLabel.Content = MainLabelText;
            return this;
        }

        private void BackToMainPage(object sender, MouseButtonEventArgs e)
        {
            GuildMembersPage.GetInstance().Frame.Navigate(null);
            Reset();
        }

        private void Reset()
        {
            BanReasonTb.Text = string.Empty;
            member = null;
            MainLabel.Content = string.Empty;
        }

        private void BanButton(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.Client.BanGuildMember(member.User.UserId,
                    member.Guild.GuildId, BanReasonTb.Text.Trim() == string.Empty
                        ? null
                        : BanReasonTb.Text.Trim());

                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Пользователь {member.User.Username} был успешно забанен"
                });
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Error,
                    Title = "Ошибка",
                    Message = "Что-то пошло не так"
                });
            }
            GuildMembersPage.GetInstance().Frame.Navigate(null);
            Reset();
        }
    }
}
