using System;
using FAZEngine;
using System.Diagnostics;

namespace zomination
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameMain game = new GameMain())
            {
#if WINDOWS
                // set profile name
                int active;
                if (!int.TryParse(new FAZEngine.IniFile(@".\configs\settings.ini").IniReadValue("Profiles", "Active"), out active))
                {
                    try
                    {
                        Process.Start("launcher.exe");
                    }
                    catch
                    {
                        //MsgBoxF.Show("The game cannot start.", "Some files are missing. Please reinstall the game. Error code: " + ErrCodes.GameLauncherMissing);
                    }
                    return;
                }
                // start game
                GlobalHelper.ActiveProfile = active;
#else
                GlobalHelper.ActiveProfile = 1;
#endif
                game.Run();
            }
        }
    }
}

