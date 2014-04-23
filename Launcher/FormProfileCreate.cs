using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using DRM;

namespace Launcher
{
    /// <summary>
    /// This form verifies the product key and saves it to an ini file.
    /// </summary>
    public partial class FormProfileCreate : Form
    {
        #region DLL Functions for Window Drag Move
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        IniFile settingsIni;
        bool setup = false;
        int activeProfile;

        public FormProfileCreate()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void FormLicense_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists("configs"))
            {
                Directory.CreateDirectory("configs");
                setup = true;
            }
            if (!File.Exists(FileNames.SettingsIni))
                setup = true;

            settingsIni = new IniFile(FileNames.SettingsIni);

            if (!int.TryParse(settingsIni.IniReadValue("Profiles", "Active"), out activeProfile) || activeProfile < 1 || activeProfile > 4)
                setup = true;

            if (setup) InitialSetup();
            else
            {
                // select the right radio button
                radioButton1.Checked = activeProfile == 1;
                radioButton2.Checked = activeProfile == 2;
                radioButton3.Checked = activeProfile == 3;
                radioButton4.Checked = activeProfile == 4;
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            settingsIni.IniWriteValue("Profiles", "Active", activeProfile.ToString());
            if (!SaveProfileName(activeProfile)) return;
            this.Dispose();
        }

        /// <summary>
        /// For moving the window by mouse dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLicense_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                profile1Textbox.Text = settingsIni.IniReadValue("Profiles", "Profile1");
                activeProfile = 1;
            }
            else
                SaveProfileName(1);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                profile1Textbox.Text = settingsIni.IniReadValue("Profiles", "Profile2");
                activeProfile = 2;
            }
            else
                SaveProfileName(2);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                profile1Textbox.Text = settingsIni.IniReadValue("Profiles", "Profile3");
                activeProfile = 3;
            }
            else
                SaveProfileName(3);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                profile1Textbox.Text = settingsIni.IniReadValue("Profiles", "Profile4");
                activeProfile = 4;
            }
            else
                SaveProfileName(4);
        }

        /// <summary>
        /// When no profile setting is created
        /// </summary>
        private void InitialSetup()
        {
            settingsIni.IniWriteValue("Profiles", "Active", "1");
            settingsIni.IniWriteValue("Profiles", "Profile1", "Profile1");
            settingsIni.IniWriteValue("Profiles", "Profile2", "Profile2");
            settingsIni.IniWriteValue("Profiles", "Profile3", "Profile3");
            settingsIni.IniWriteValue("Profiles", "Profile4", "Profile4");
            radioButton1.Checked = true;
            activeProfile = 1;
        }

        private bool SaveProfileName(int profileIndex)
        {
            if (string.IsNullOrWhiteSpace(profile1Textbox.Text))
            {
                MsgBoxF.Show("Invalid profile name.", "Please choose a different profile name.");
                return false;
            }
            else
            {
                settingsIni.IniWriteValue("Profiles", "Profile" + profileIndex, profile1Textbox.Text);
                return true;
            }
        }
    }
}
