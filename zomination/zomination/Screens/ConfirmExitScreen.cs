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
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using FAZEngine;
#endregion

namespace zomination
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class ConfirmExitScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ConfirmExitScreen()
            : base("Are you sure you want to exit?")
        {
            MenuEntry yesEntry = new MenuEntry("Yes");
            MenuEntry back = new MenuEntry("No");

            // Hook up menu event handlers.
            yesEntry.Selected += ExitConfirmed;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(yesEntry);
            MenuEntries.Add(back);
            
        }

        #endregion

        #region Handle Input

        void ExitConfirmed(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}
