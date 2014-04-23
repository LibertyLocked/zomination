using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Launcher
{
    public partial class FormMsgbox : Form
    {
        #region DLL Functions for Window Drag Move
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        private string title;
        private string detail;
        private string button1;
        private string button2;
        private string url;
        private EventHandler clickEvent1;

        public FormMsgbox(string title, string detail)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.title = title;
            this.detail = detail;
            this.button2 = "CLOSE";
            InitializeComponent();
        }

        public FormMsgbox(string title, string detail, string button1, string button2, string url)
        {
            this.button1 = button1;
            this.button2 = button2;
            this.url = url;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.title = title;
            this.detail = detail;
            InitializeComponent();
        }

        public FormMsgbox(string title, string detail, string button1, string button2, EventHandler clickEvent1)
        {
            this.button1 = button1;
            this.button2 = button2;
            this.clickEvent1 = clickEvent1;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.title = title;
            this.detail = detail;
            InitializeComponent();
        }

        private void FormMessagebox_Load(object sender, EventArgs e)
        {
            // display button1 when there is text for it
            closeButton.Text = button2;
            if (button1 != null && button1 != "")
            {
                confirmButton.Text = button1;
                confirmButton.Enabled = true;
                confirmButton.Visible = true;
            }

            // hide button2 when there is no text for it
            if (button2 == null || button2 == "")
            {
                closeButton.Enabled = false;
                closeButton.Visible = false;
            }

            // set event for button1 when customized
            if (clickEvent1 != null)
            {
                this.confirmButton.Click -= confirmButton_Click;
                this.confirmButton.Click += clickEvent1;
            }

            // change window size according to the size of detail message label!
            int origDetailLabelHeight = messageLabel2.Height;
            int origFormHeight = this.Height;
            messageLabel1.Text = title;
            messageLabel2.Text = detail;
            int currDetailLabelHeight = messageLabel2.Height;

            this.Height = origFormHeight + (currDetailLabelHeight - origDetailLabelHeight);
        }

        /// <summary>
        /// This is to move the window when mouse dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMessagebox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonEffects_MouseEnter(object sender, EventArgs e)
        {
            Label button = (Label)sender;
            button.Font = new Font(button.Font, FontStyle.Underline | FontStyle.Bold);
        }

        private void buttonEffects_MouseLeave(object sender, EventArgs e)
        {
            Label button = (Label)sender;
            button.Font = new Font(button.Font, FontStyle.Regular);
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(url);
            }
            catch
            { }
            this.Dispose();
        }
    }
}
