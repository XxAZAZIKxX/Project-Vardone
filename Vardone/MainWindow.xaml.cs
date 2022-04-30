using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Core;
using Vardone.Pages;
using VardoneLibrary.Core.Client;
using VardoneLibrary.Core.Client.Base;
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
        public static MainWindow GetInstance() => _instance ??= new MainWindow();

        private static readonly string DllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string PATH = System.IO.Path.GetDirectoryName(DllPath);

        public readonly NotificationManager notificationManager = new(Dispatcher.CurrentDispatcher);
        private WinForms.NotifyIcon _trayIcon;

        private MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();
            _instance = this;
            TryLogin();
        }
        private void InitializeTrayIcon()
        {
            _trayIcon = new WinForms.NotifyIcon
            {
                Visible = true,
                Text = "Vardone",
                Icon = new Icon(PATH + @"\resources\contentRes\va.ico"),
                ContextMenuStrip = new WinForms.ContextMenuStrip()
            };
            _trayIcon.MouseClick += TrayIconOnMouseClick;

            _trayIcon.ContextMenuStrip.Items.Add("Открыть").Click += TrayOpenClick;
            _trayIcon.ContextMenuStrip.Items.Add("Закрыть").Click += TrayCloseClick;
        }
        //Methods
        public static void FlushMemory()
        {
            var prs = Process.GetCurrentProcess();
            try
            {
                prs.MinWorkingSet = (IntPtr)(300000);
            }
            catch
            {
                // ignored
            }
        }
        private void TryLogin()
        {
            var token = ConfigWorker.GetToken();
            if (VardoneBaseApi.CheckToken(ref token)) LoadApp(new VardoneClient(token));
            else MainFrame.Navigate(AuthorizationPage.GetInstance());
        }
        public void LoadApp(VardoneClient client)
        {
            ConfigWorker.SetToken(client.Token);
            MainFrame.Navigate(MainPage.GetInstance().Load(client));
        }
        private void CloseApp()
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            Application.Current.Shutdown(0);
        }
        //Events
        private void TrayIconOnMouseClick(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button != WinForms.MouseButtons.Left) return;
            if (ShowInTaskbar) CloseBtnClick(null, null);
            else TrayOpenClick(null, null);
        }
        private void TrayOpenClick(object sender, EventArgs e)
        {
            Show();
            Focus();
            Topmost = true;
            Topmost = false;
            ShowInTaskbar = true;
        }
        private void TrayCloseClick(object sender, EventArgs e) => CloseApp();

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            HideAppInTaskbar();
            notificationManager.Show(new NotificationContent
            {
                Title = "Vardone был свернут в трей",
                Message = "Иконку можно найти в трее",
                Type = NotificationType.Information
            }, "", TimeSpan.FromSeconds(5), () => TrayOpenClick(null, null));
            FlushMemory();
        }

        public void HideAppInTaskbar()
        {
            ShowInTaskbar = false;
            Hide();
        }

        //Resize controls
        private void DockPanelMouseLeftButtonDown(object sender, MouseEventArgs mouseEventArgs)
        {
            try
            {
                DragMove();
            }
            catch
            {
                // ignored
            }
        }
        private void ThumbBottomRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var dragDeltaEventArgs = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
            ThumbRight_DragDelta(null, dragDeltaEventArgs);
            ThumbBottom_DragDelta(null, dragDeltaEventArgs);
        }
        private void ThumbTopRightCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var dragDeltaEventArgs = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
            ThumbTop_DragDelta(null, dragDeltaEventArgs);
            ThumbRight_DragDelta(null, dragDeltaEventArgs);
        }
        private void ThumbTopLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var dragDeltaEventArgs = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
            ThumbTop_DragDelta(null, dragDeltaEventArgs);
            ThumbLeft_DragDelta(null, dragDeltaEventArgs);
        }
        private void ThumbBottomLeftCorner_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var dragDeltaEventArgs = new DragDeltaEventArgs(e.HorizontalChange, e.VerticalChange);
            ThumbBottom_DragDelta(null, dragDeltaEventArgs);
            ThumbLeft_DragDelta(null, dragDeltaEventArgs);
        }
        private void ThumbRight_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Width + e.HorizontalChange > 10) Width += e.HorizontalChange;
        }
        private void ThumbLeft_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange > 0 && Math.Abs(Width - MinWidth) < 0.01) return;
            if (!(Left + e.HorizontalChange > 10)) return;
            try
            {
                Width -= e.HorizontalChange;
                Left += e.HorizontalChange;
            }
            catch
            {
                // ignored
            }
        }
        private void ThumbBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Height + e.VerticalChange > 10) Height += e.VerticalChange;
        }
        private void ThumbTop_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(Top + e.VerticalChange > 10)) return;
            if (e.VerticalChange > 0 && Math.Abs(Height - MinHeight) < 0.01) return;
            try
            {
                Height -= e.VerticalChange;
                Top += e.VerticalChange;
            }
            catch
            {
                // ignored
            }
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
        private void Thumbs()
        {
            var visibility = _maximized ? Visibility.Collapsed : Visibility.Visible;
            ThumbBottom.Visibility = visibility;
            ThumbLeft.Visibility = visibility;
            ThumbTop.Visibility = visibility;
            ThumbRight.Visibility = visibility;
            ThumbTopLeftCorner.Visibility = visibility;
            ThumbTopRightCorner.Visibility = visibility;
            ThumbBottomLeftCorner.Visibility = visibility;
            ThumbBottomRightCorner.Visibility = visibility;
        }
    }
}