using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace UpdateHelper
{
    public partial class UpdatingForm : Form
    {
        #region DLL Functions for Window Drag Move
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        bool retryUsed = false;

        public UpdatingForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            functionTimer.Stop();
            functionTimer.Enabled = false;
            UpdateLauncher();
            //this.Dispose();
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

        /// <summary>
        /// Updates the launcher.
        /// </summary>
        public void UpdateLauncher()
        {
            if (!File.Exists("launcher.update"))
            {
                MessageBox.Show("Some files are missing. You can manually fix this by downloading the update on 3LT website.", "Unable to apply updates.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Dispose();
            }

            try
            {
                if (File.Exists("Launcher.exe")) File.Delete("Launcher.exe");
                File.Move("launcher.update", "Launcher.exe");
            }
            catch
            {
                if (!retryUsed)
                {
                    Thread.Sleep(2000);
                    functionTimer.Enabled = true;
                    functionTimer.Start();
                    retryUsed = true;
                    return;
                }
                else
                {
                    MessageBox.Show("An error occurred when applying updates. You can manually fix this by downloading the update on 3LT website.", "Unable to apply updates.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                }
            }
            
            // restart launcher
            try
            {
                Process.Start("Launcher.exe");
                this.Dispose();
            }
            catch
            {
                MessageBox.Show("Some files are missing.", "Unable to restart launcher.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Dispose();
            }
        }
    }
}
