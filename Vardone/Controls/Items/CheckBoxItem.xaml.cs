using System;
using Vardone.Controls.Items.Buttons;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Логика взаимодействия для CheckBoxItem.xaml
    /// </summary>
    public partial class CheckBoxItem
    {
        public CheckBoxItem(string name, bool isActive, in Action<bool> checkBoxAction)
        {
            InitializeComponent();
            NameItem.Content = name;
            Button.Children.Add(new CheckBoxButton(isActive, checkBoxAction));
        }
    }
}
