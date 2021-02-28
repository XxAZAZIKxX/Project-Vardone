using System.Windows.Forms;
using testForms.Controls;
using VardoneLibrary.Core;

namespace testForms.Forms
{
    public partial class Main : Form
    {
        private static Main _instance;
        public VardoneClient Client { get; set; }
        public static Main GetInstance() => _instance ??= new Main();
        private Main() => InitializeComponent();

        private void Main_Load(object sender, System.EventArgs e)
        {
            Controls.Add(LoginControl.GetInstance());
        }

        public void Login()
        {

        }
    }
}
