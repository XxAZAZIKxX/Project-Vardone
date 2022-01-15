using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vardone.Controls.Items.Buttons
{
    /// <summary>
    /// Логика взаимодействия для CheckBoxButton.xaml
    /// </summary>
    public partial class CheckBoxButton
    {
        private bool _isActive;
        private readonly Action<bool> _change;
        public bool IsActive
        {
            get => _isActive;
            set => ChangeCheckBox(value);
        }

        public CheckBoxButton(bool active, in Action<bool> action)
        {
            InitializeComponent();
            IsActive = active;
            _change = action;
        }

        private void ChangeCheckBoxClick(object sender, MouseButtonEventArgs e) => IsActive = !IsActive;

        private void ChangeCheckBox(bool active)
        {
            _isActive = active;
            if (active)
            {
                CircleActive.Visibility = Visibility.Visible;
                CircleUnActive.Visibility = Visibility.Collapsed;
            }
            else
            {
                CircleActive.Visibility = Visibility.Hidden;
                CircleUnActive.Visibility = Visibility.Visible;
            }
            _change?.Invoke(IsActive);
        }
    }
}
