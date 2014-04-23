using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UpdateHelper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Please use \"Check Updates\" in game launcher.", "Want to update your game?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdatingForm());
        }
    }
}
