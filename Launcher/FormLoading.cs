using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Launcher
{
    public partial class FormLoading : Form
    {
        #region DLL Functions for Window Drag Move
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        public delegate void LoadFunction(Form thisForm);
        LoadFunction loadStuff;

        public FormLoading(LoadFunction loadStuff)
        {
            this.loadStuff = loadStuff;
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            functionTimer.Stop();
            functionTimer.Enabled = false;
            loadStuff(this);
            this.Dispose();
        }

        /// <summary>
        /// For moving the window by mouse dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoading_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
