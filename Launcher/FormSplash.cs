using System;
using System.Drawing;
using System.Windows.Forms;

namespace Launcher
{
    public partial class FormSplash : Form
    {
        public FormSplash()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void FormSplash_Load(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(33, 33, 33);
            TransparencyKey = Color.FromArgb(33, 33, 33);
            //BackColor = Color.DarkGray;
            //TransparencyKey = Color.DarkGray;
            //timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void FormSplash_MouseClick(object sender, MouseEventArgs e)
        {
            this.Dispose();
        }
    }
}
