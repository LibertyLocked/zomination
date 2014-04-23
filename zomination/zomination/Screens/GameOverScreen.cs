using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Peds;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace zomination
{
    class GameOverScreen : MenuScreen
    {
        PlayerIndex p1Index;
        PlayerIndex? p2Index;
        Player p1, p2;
        Cue bgm;

        public GameOverScreen(PlayerIndex p1Index, PlayerIndex? p2Index, Player p1, Player p2, Cue actionBgm)
            : base("Game Over")
        {
            this.p1Index = p1Index;
            this.p2Index = p2Index;
            this.p1 = p1;
            this.p2 = p2;

            if (actionBgm.IsPlaying) actionBgm.Stop(AudioStopOptions.Immediate);

            MenuEntry share = new MenuEntry("Share Highscore");
            MenuEntry restart = new MenuEntry("Restart Game");
            MenuEntry back = new MenuEntry("Exit to Menu");

            share.Selected += ShareSelected;
            restart.Selected += RestartSelected;
            back.Selected += BackSelected;

            MenuEntries.Add(restart);
            MenuEntries.Add(share);
            MenuEntries.Add(back);
        }

        public override void Activate(bool instancePreserved)
        {
            bgm = ((GameMain)ScreenManager.Game).AudioEM.GetCue("failedBg1");
            bgm.Play();
        }

        void BackSelected(object sender, PlayerIndexEventArgs e)
        {
            bgm.Stop(AudioStopOptions.Immediate);
            BackgroundScreen bgScreen = new BackgroundScreen();
            LoadingScreen.Load(ScreenManager, false, null, bgScreen,
                                               new MainMenuScreen(bgScreen));
        }

        void RestartSelected(object sender, PlayerIndexEventArgs e)
        {           
            bgm.Stop(AudioStopOptions.Immediate);
            if (p2Index == null)
            {
                // restart solo
                LoadingScreen.Load(ScreenManager, false, p1Index, null, new SurvivalGameplayScreen());
            }
            else
            {
                // restart Splitscreen
                LoadingScreen.Load(ScreenManager, false, null, null, new CoopGameplayScreen(p1Index, (PlayerIndex)p2Index));
            }
        }

        void ShareSelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            string message = "";
            if (p2 == null)
                message = "I killed " + p1.KillCount + " zombie" + (p1.KillCount > 1 ? "s" : "");
            else
                message = "We killed " + (p1.KillCount + p2.KillCount) + " zombie" + ((p1.KillCount + p2.KillCount) > 1 ? "s" : "");
            message += " in Zomination from XBIG! How many have you killed?";
            try
            {
                Guide.ShowComposeMessage(e.PlayerIndex, message, null);
            }
            catch { }
#endif
        }

        protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            return;
        }
    }
}
