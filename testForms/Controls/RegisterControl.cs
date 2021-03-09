using System;
using System.Windows.Forms;
using testForms.Forms;
using VardoneEntities.Models;
using VardoneEntities.Models.GeneralModels;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Base;

namespace testForms.Controls
{
    public partial class RegisterControl : UserControl
    {
        private static RegisterControl _instance;
        public static RegisterControl GetInstance() => _instance ??= new RegisterControl();
        private RegisterControl() => InitializeComponent();

        private void register_btn_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username_tb.Text) || string.IsNullOrWhiteSpace(email_tb.Text) ||
                string.IsNullOrWhiteSpace(password_tb.Text))
            {
                MessageBox.Show(@"Заполните все поля", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool register;
            try
            {
                register = BaseApi.RegisterUser(new RegisterUserModel
                {
                    Username = username_tb.Text, Email = email_tb.Text, Password = password_tb.Text
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            if (register is not true)
            {
                MessageBox.Show(@"Что-то пошло не так", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Main.GetInstance().Controls.Remove(this);
            Main.GetInstance().Controls.Add(LoginControl.GetInstance());
        }
    }
}