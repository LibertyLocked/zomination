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
    class UnlockFullVerScreen : MenuScreen
    {
        #region Fields

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public UnlockFullVerScreen()
            : base("Unlock full game to access all features!")
        {
            // Create our menu entries.
            MenuEntry purchaseEntry = new MenuEntry("Unlock Full Game");
            MenuEntry back = new MenuEntry("Continue with Trial");

            // Hook up menu event handlers.
            purchaseEntry.Selected += PurchaseMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(purchaseEntry);
            MenuEntries.Add(back);
        }

        #endregion

        #region Handle Input

        void PurchaseMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            try
            {
                // wrap guide show in try catch
                Guide.ShowMarketplace(e.PlayerIndex);
            }
            catch { }
            this.ExitScreen();
#endif
        }

        #endregion
    }
}
