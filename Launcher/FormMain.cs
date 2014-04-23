using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using DRM;

namespace Launcher
{
    public partial class FormMain : Form
    {
        #region DLL Functions for Window Drag Move
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        int activeProfile;

        public FormMain()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        /// <summary>
        /// Prepare to load the main form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            // show splash screen
            using (FormSplash splash = new FormSplash())
            {
                splash.ShowDialog();
            }

            // check if configs folder exists
            if (!Directory.Exists("configs"))
            {
                Directory.CreateDirectory("configs");
            }

            // load feed
            UpdateModule.CheckFeed();

            // check profiles & settings
            CheckAndFixSettings();

            // main form transparancy set
            this.BackColor = Color.FromArgb(33, 33, 33);
            this.TransparencyKey = Color.FromArgb(33, 33, 33);
        }

        /// <summary>
        /// When launch button is clicked, prepare to launch the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void launchButton_Click(object sender, EventArgs e)
        {
            CheckAndFixSettings();

            try
            {
                System.Diagnostics.Process.Start(FileNames.GameExe);
            }
            catch
            {
                MsgBoxF.Show("Unable to launch the game.", "Some files are missing. Please reinstall the game.");
            }
        }

        private void motdButton_Click(object sender, EventArgs e)
        {
            UpdateModule.CheckFeed();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateModule.CheckUpdates();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        /// <summary>
        /// For moving the window by mouse dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        /// <summary>
        /// Go to Profile Management.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsButton_Click(object sender, EventArgs e)
        {
            new FormProfileCreate().ShowDialog();
        }

        private void coopButton_Click(object sender, EventArgs e)
        {

        }

        private void CheckAndFixSettings()
        {
            IniFile settingsIni = new IniFile(FileNames.SettingsIni);
            if (!int.TryParse(settingsIni.IniReadValue("Profiles", "Active"), out activeProfile))
            {
                new FormProfileCreate().ShowDialog();
                activeProfile = int.Parse(settingsIni.IniReadValue("Profiles", "Active"));
            }

            // if Options doesn't exist, create default
            if (string.IsNullOrEmpty(settingsIni.IniReadValue("Options", "Fullscreen")) ||
                string.IsNullOrEmpty(settingsIni.IniReadValue("Options", "UseGamePad")) ||
                string.IsNullOrEmpty(settingsIni.IniReadValue("Options", "MSAA")))
            {
                settingsIni.IniWriteValue("Options", "Fullscreen", "True");
                settingsIni.IniWriteValue("Options", "MSAA", "4");
                settingsIni.IniWriteValue("Options", "UseGamePad", "False");
            }
        }

        private void displaySetButton_Click(object sender, EventArgs e)
        {
            new FormDisplaySet().ShowDialog();
        }

        private void modEditorButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FormModEditor().ShowDialog();
            this.Show();
        }
    }
}
