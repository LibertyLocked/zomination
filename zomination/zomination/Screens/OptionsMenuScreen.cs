#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
#endregion

namespace zomination
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry fullscreenMenuEntry;
        MenuEntry msaaMenuEntry;
        MenuEntry useGamePadMenuEntry;
        MenuEntry friendlyFireMenuEntry;
        MenuEntry shareMoneyMenuEntry;
        MenuEntry shareRevivesMenuEntry;

        bool fullscreen;
        int msaa;

        public static bool friendlyFire = false;
        public static bool shareMoney = false;
        public static bool shareRevives = false;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            fullscreenMenuEntry = new MenuEntry(string.Empty);
            msaaMenuEntry = new MenuEntry(string.Empty);
            useGamePadMenuEntry = new MenuEntry(string.Empty);
            friendlyFireMenuEntry = new MenuEntry(string.Empty);
            shareMoneyMenuEntry = new MenuEntry(string.Empty);
            shareRevivesMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            fullscreenMenuEntry.Selected += FullscreenMenuEntrySelected;
            msaaMenuEntry.Selected += MSAAMenuEntrySelected;
            useGamePadMenuEntry.Selected += UseGamePadMenuEntrySelected;
            friendlyFireMenuEntry.Selected += FriendlyFireMenuEntrySelected;
            shareMoneyMenuEntry.Selected += ShareMoneyMenuEntrySelected;
            shareRevivesMenuEntry.Selected += ShareRevivesMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
#if WINDOWS
            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(useGamePadMenuEntry);
            MenuEntries.Add(msaaMenuEntry);
#endif
            MenuEntries.Add(friendlyFireMenuEntry);
            MenuEntries.Add(shareRevivesMenuEntry);
            MenuEntries.Add(shareMoneyMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            fullscreenMenuEntry.Text = "Fullscreen: " + (fullscreen ? "On" : "Off");
            msaaMenuEntry.Text = "MSAA: " + (msaa == 0 ? "Off" : msaa + "x");
            useGamePadMenuEntry.Text = "Use GamePad: " + (GlobalHelper.UseGamePad ? "On" : "Off");
            friendlyFireMenuEntry.Text = "Friendly Fire: " + (friendlyFire ? "On" : "Off");
            shareMoneyMenuEntry.Text = "Share Money: " + (shareMoney ? "On" : "Off");
            shareRevivesMenuEntry.Text = "Share Lives: " + (shareRevives ? "On" : "Off");
        }

        public override void Activate(bool instancePreserved)
        {
            // Set values
            fullscreen = ScreenManager.GraphicsDevice.PresentationParameters.IsFullScreen;
            msaa = ScreenManager.GraphicsDevice.PresentationParameters.MultiSampleCount;

            SetMenuEntryText();

            base.Activate(instancePreserved);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Fullscreen menu entry is selected.
        /// </summary>
        void FullscreenMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            fullscreen = !fullscreen;
            ((GameMain)ScreenManager.Game).Graphics.IsFullScreen = fullscreen;
            ((GameMain)ScreenManager.Game).Graphics.ApplyChanges();

            SetMenuEntryText();
        }

        void MSAAMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (msaa == 0) msaa = 2;
            else if (msaa < 8) msaa *= 2;
            else msaa = 0;
            ScreenManager.GraphicsDevice.PresentationParameters.MultiSampleCount = msaa;

            //((GameMain)ScreenManager.Game).Graphics.ApplyChanges();
            ScreenManager.GraphicsDevice.Reset(ScreenManager.GraphicsDevice.PresentationParameters);

            SetMenuEntryText();
        }

        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void UseGamePadMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GlobalHelper.UseGamePad = !GlobalHelper.UseGamePad;
            ScreenManager.Game.IsMouseVisible = !GlobalHelper.UseGamePad;

            SetMenuEntryText();
        }

        void FriendlyFireMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            friendlyFire = !friendlyFire;
            SetMenuEntryText();
        }

        void ShareMoneyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            shareMoney = !shareMoney;
            if (shareMoney)
            {
                ScreenManager.AddScreen(new MessageBoxScreen("With share money enabled\nHold RB to transfer money", false), e.PlayerIndex);
            }
            SetMenuEntryText();
        }

        void ShareRevivesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            shareRevives = !shareRevives;
            SetMenuEntryText();
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // save settings to ini file
#if WINDOWS
            FAZEngine.IniFile settingFile = new FAZEngine.IniFile(@".\configs\settings.ini");
            settingFile.IniWriteValue("Options", "Fullscreen", fullscreen.ToString());
            settingFile.IniWriteValue("Options", "MSAA", msaa.ToString());
            settingFile.IniWriteValue("Options", "UseGamePad", GlobalHelper.UseGamePad.ToString());
#endif
            base.OnCancel(playerIndex);
        }

        #endregion
    }
}
