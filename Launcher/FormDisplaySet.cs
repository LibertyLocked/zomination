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
    public partial class FormDisplaySet : Form
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

        public FormDisplaySet()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void FormLicense_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists("configs"))
                Directory.CreateDirectory("configs");

            settingsIni = new IniFile(FileNames.SettingsIni);

            if (!File.Exists(FileNames.SettingsIni))
            {
                checkBox1.Checked = true;
                checkBox2.Checked = true;
                checkBox1_CheckedChanged(this, null);
            }
            else
            {
                checkBox1.Checked = string.IsNullOrEmpty(settingsIni.IniReadValue("Resolution", "Width"));
                checkBox2.Checked = bool.Parse(settingsIni.IniReadValue("Options", "Fullscreen"));
                resText1.Text = settingsIni.IniReadValue("Resolution", "Width");
                resText2.Text = settingsIni.IniReadValue("Resolution", "Height");
                if (string.IsNullOrEmpty(resText1.Text)) checkBox1_CheckedChanged(this, null);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            // validate resolution
            if (!checkBox1.Checked)
            {
                int width, height;
                if (int.TryParse(resText1.Text, out width) && int.TryParse(resText2.Text, out height))
                {
                    if ((float)width / height > (float)16 / 9)
                    {
                        MsgBoxF.Show("Unsupported resolution.", "Resolutions that are wider than 16:9 is not supported.");
                        return;
                    }
                }
                else
                {
                    MsgBoxF.Show("Invalid resolution.", "Please enter only numbers for custom resolution.");
                    return;
                }
            }

            // save settings
            if (checkBox1.Checked)
            {
                settingsIni.IniWriteValue("Resolution", "Width", "");
                settingsIni.IniWriteValue("Resolution", "Height", "");
            }
            else
            {
                settingsIni.IniWriteValue("Resolution", "Width", resText1.Text);
                settingsIni.IniWriteValue("Resolution", "Height", resText2.Text);
            }

            settingsIni.IniWriteValue("Options", "Fullscreen", checkBox2.Checked.ToString());

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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            resText1.Enabled = !checkBox1.Checked;
            resText2.Enabled = !checkBox1.Checked;
            resText1.Text = resText1.Enabled ? "" : "AUTO";
            resText2.Text = resText2.Enabled ? "" : "AUTO";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
