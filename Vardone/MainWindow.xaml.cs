using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;
using WinForms = System.Windows.Forms;

namespace Vardone
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _maximized;
        private int _normalWidth;
        private int _normalHeight;
        private int _normalX;
        private int _normalY;

        private static MainWindow _instance;
        public static MainWindow GetInstance() => _instance;
        public static readonly string DLL_PATH = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string PATH = System.IO.Path.GetDirectoryName(DLL_PATH);
        public MainWindow()
        {
            InitializeComponent();
            _instance = this;
            var token = JsonTokenWorker.GetToken();
            if (token is null) MainFrame.Navigate(AuthorizationPage.GetInstance());
            else
            {
                if (VardoneBaseApi.CheckToken(token.UserId, token.Token))
                    LoadApp(new VardoneClient(token.UserId, token.Token));
                else MainFrame.Navigate(AuthorizationPage.GetInstance());
            }
        }

        public void LoadApp(VardoneClient client)
        {
            JsonTokenWorker.SetToken(new UserTokenModel { UserId = client.UserId, Token = client.Token });
            MainPage.GetInstance().Load(client);
            MainFrame.Navigate(MainPage.GetInstance());
        }

        private void DockPanelMouseLeftButtonDown(object sender, MouseEventArgs mouseEventArgs)
        {
            try { DragMove(); }
            catch
            {
                // ignored
            }
        }

        private void ThumbBottomRightCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10) Width += e.HorizontalChange;
            if (Height + e.VerticalChange > 10) Height += e.VerticalChange;
        }

        private void ThumbTopRightCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10) Width += e.HorizontalChange;
            if (!(Top + e.VerticalChange > 10)) return;
            Top += e.VerticalChange;
            Height -= e.VerticalChange;
        }

        private void ThumbTopLeftCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }

            if (!(Top + e.VerticalChange > 10)) return;
            Top += e.VerticalChange;
            Height -= e.VerticalChange;
        }

        private void ThumbBottomLeftCorner_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Left + e.HorizontalChange > 10)
            {
                Left += e.HorizontalChange;
                Width -= e.HorizontalChange;
            }

            if (Height + e.VerticalChange > 10) Height += e.VerticalChange;
        }

        private void ThumbRight_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10) Width += e.HorizontalChange;
        }

        private void ThumbLeft_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (e.HorizontalChange > 0 && Math.Abs(Width - MinWidth) < 0.01) return;
            if (!(Left + e.HorizontalChange > 10)) return;
            Left += e.HorizontalChange;
            Width -= e.HorizontalChange;

        }

        private void ThumbBottom_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange > 10) Height += e.VerticalChange;
        }

        private void ThumbTop_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (!(Top + e.VerticalChange > 10)) return;
            Top += e.VerticalChange;
            Height -= e.VerticalChange;
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            if (_maximized)
            {
                Width = _normalWidth;
                Height = _normalHeight;
                Left = _normalX;
                Top = _normalY;
                _maximized = false;
                Max.Content = "◻";
                Thumbs();
            }
            else
            {
                Max.Content = "❐";
                _normalX = (int)Left;
                _normalY = (int)Top;
                _normalHeight = (int)Height;
                _normalWidth = (int)Width;
                Left = WinForms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Left;
                Top = WinForms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Top;
                Width = WinForms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Width;
                Height = WinForms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea.Height;
                _maximized = true;
                Thumbs();
            }
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        private void Thumbs()
        {
            if (_maximized)
            {
                ThumbBottom.Visibility = Visibility.Collapsed;
                ThumbLeft.Visibility = Visibility.Collapsed;
                ThumbTop.Visibility = Visibility.Collapsed;
                ThumbRight.Visibility = Visibility.Collapsed;
                ThumbTopLeftCorner.Visibility = Visibility.Collapsed;
                ThumbTopRightCorner.Visibility = Visibility.Collapsed;
                ThumbBottomLeftCorner.Visibility = Visibility.Collapsed;
                ThumbBottomRightCorner.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThumbBottom.Visibility = Visibility.Visible;
                ThumbLeft.Visibility = Visibility.Visible;
                ThumbTop.Visibility = Visibility.Visible;
                ThumbRight.Visibility = Visibility.Visible;
                ThumbTopLeftCorner.Visibility = Visibility.Visible;
                ThumbTopRightCorner.Visibility = Visibility.Visible;
                ThumbBottomLeftCorner.Visibility = Visibility.Visible;
                ThumbBottomRightCorner.Visibility = Visibility.Visible;
            }
        }
    }
}