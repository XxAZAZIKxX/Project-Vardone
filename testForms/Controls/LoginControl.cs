using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using testForms.Forms;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;

namespace testForms.Controls
{
    public partial class LoginControl : UserControl
    {
        private static LoginControl _instance;
        public static LoginControl GetInstance() => _instance ??= new LoginControl();
        private LoginControl()
        {
            InitializeComponent();
        }

        private void login_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username_tb.Text) || string.IsNullOrWhiteSpace(password_tb.Text))
            {
                MessageBox.Show(@"Заполните поля",@"Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);return;
            }

            string userToken;
            try
            {
                userToken = BaseApi.GetUserToken(username_tb.Text, password_tb.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(@"Неверные данные",@"Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            Main.GetInstance().Client = new VardoneClient(username_tb.Text, userToken);
            Main.GetInstance().Controls.Remove(this);
        }

        private void register_lbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Main.GetInstance().Controls.Remove(this);
            Main.GetInstance().Controls.Add(RegisterControl.GetInstance());
        }
    }
}
