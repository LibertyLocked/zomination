#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using FAZEngine;
#endregion

namespace zomination
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        BackgroundScreen bgScreen;
        MenuEntry unlockFullMenuEntry = new MenuEntry("Unlock Full Game");
        Cue bgm;

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        /// <param name="bgScreen">Background screen associated with main menu screen</param>
        public MainMenuScreen(BackgroundScreen bgScreen)
            : base("Main Menu")
        {
            this.bgScreen = bgScreen;

            // Create our menu entries.
            MenuEntry playSurvivalMenuEntry = new MenuEntry("Singleplayer");
            MenuEntry playSplitScreenMenuEntry = new MenuEntry("Splitscreen");
            MenuEntry howToPlayMenuEntry = new MenuEntry("How to Play");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry creditsMenuEntry = new MenuEntry("Credits");
            MenuEntry exitMenuEntry = new MenuEntry("Exit Game");

            // Hook up menu event handlers.
            playSurvivalMenuEntry.Selected += PlaySurivialMenuEntrySelected;
            playSplitScreenMenuEntry.Selected += PlaySplitScreenMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            creditsMenuEntry.Selected += CreditsMenuEntrySelected;
            howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
            unlockFullMenuEntry.Selected += UnlockFullMenuEntrySelected;
            exitMenuEntry.Selected += ExitMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(playSurvivalMenuEntry);
            MenuEntries.Add(playSplitScreenMenuEntry);
            MenuEntries.Add(howToPlayMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(creditsMenuEntry);
#if XBOX
            if (Guide.IsTrialMode) MenuEntries.Add(unlockFullMenuEntry);
#endif
            MenuEntries.Add(exitMenuEntry);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (bgm == null)
                {
                    bgm = ((GameMain)ScreenManager.Game).AudioEM.GetCue("menuBg1");
                    bgm.Play();
                }
            }
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlaySurivialMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            if (!SplitScreenHelper.CheckSignIn(e.PlayerIndex)) return;
#endif
            bgm.Stop(AudioStopOptions.Immediate);
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new SurvivalGameplayScreen());
        }

        void PlaySplitScreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            if (Guide.IsTrialMode) 
            { 
                ScreenManager.AddScreen(new UnlockFullVerScreen(), null); 
                return; 
            }
#endif
            ScreenManager.AddScreen(new CoopLobbyScreen(bgm), null);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, go back to splash screen.
        /// Also exit background screen!
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            bgm.Stop(AudioStopOptions.Immediate);
            ScreenManager.AddScreen(new PressStartScreen(null), null);
            bgScreen.ExitScreen();
            base.OnCancel(playerIndex);
        }

        /// <summary>
        /// When the user selects "exit game", pop up a messagebox to confirm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ExitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ConfirmExitScreen(), null);
        }

        void UnlockFullMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new UnlockFullVerScreen(), null);
        }

        void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string msg1 = "Zomination XBIG Team\n\n"+
                "Created by: Wenchao Wang\n" +
                "Graphic Artists: Phil Washy, Xiangyu Wang\n" +
                "Music by: Jon Povirk, Andrew Hartwig\n" +
                "\nSpecial thanks to: \n" + 
                "Neil Debski, Ryan (Tank) Skorski\n\n" +
                "Sound effect credits: bit.ly/1aXUP5n" +
                "\nv1.1";            

            MessageBoxScreen creditsMessageBox = new MessageBoxScreen(msg1, false);

            ScreenManager.AddScreen(creditsMessageBox, e.PlayerIndex);
        }

        void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string msg2 = "Game Controls\n\n" +
                "Left Stick: Move\n" +
                "Right Stick: Aim\n" +
                "Y: Swap weapon\n" +
                "Right Trigger: Fire\n" +
                "Left Bumper: Reload\n" +
                "DPad Left: Buy weapons\n" +
                "DPad Right: Mod current weapon";
            MessageBoxScreen controlsMessageBox = new MessageBoxScreen(msg2, false);

            ScreenManager.AddScreen(controlsMessageBox, e.PlayerIndex);
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
#if XBOX
            if (!Guide.IsTrialMode && MenuEntries.Contains(unlockFullMenuEntry)) MenuEntries.Remove(unlockFullMenuEntry);
#endif
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
