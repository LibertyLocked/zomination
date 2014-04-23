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
    class CoopLobbyScreen : MenuScreen
    {
        #region Fields

        Cue bgm;
        MenuEntry player1MenuEntry;
        MenuEntry player2MenuEntry;
        MenuEntry start;

        PlayerIndex? p1Index = null;
        PlayerIndex? p2Index = null;

        string p1Name, p2Name;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CoopLobbyScreen(Cue bgm)
            : base("Splitscreen Lobby")
        {
            this.bgm = bgm;

            // Create our menu entries.
            player1MenuEntry = new MenuEntry(string.Empty);
            player2MenuEntry = new MenuEntry(string.Empty);
            start = new MenuEntry("Start Game");

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            player1MenuEntry.Selected += Player1MenuEntrySelected;
            player2MenuEntry.Selected += Player2MenuEntrySelected;
            start.Selected += StartMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(player1MenuEntry);
            MenuEntries.Add(player2MenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            player1MenuEntry.Text = "Player One: " + (p1Index == null ? "[Press A]" : p1Name);
            player2MenuEntry.Text = "Player Two: " + (p2Index == null ? "[Press A]" : p2Name);
            if (!MenuEntries.Contains(start) && (p1Index != null && p2Index != null)) MenuEntries.Insert(2, start);
            else if (MenuEntries.Contains(start) && (p1Index == null || p2Index == null)) MenuEntries.Remove(start);
        }

        public override void Activate(bool instancePreserved)
        {
            // Set values

            SetMenuEntryText();

            base.Activate(instancePreserved);
        }

        #endregion

        #region Handle Input

        void Player1MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            Gamer prof = SplitScreenHelper.GetGamerProfile(e.PlayerIndex);
            if (prof == null)
            {
                try
                {
                    Guide.ShowSignIn(4, false);
                }
                catch { }
                return;
            }
#endif
            if (p2Index == e.PlayerIndex)
            {
                p1Index = p2Index;
                p2Index = null;
            }
            p1Index = e.PlayerIndex;
#if WINDOWS
            p1Name = "Controller " + e.PlayerIndex.ToString();
#else
            p1Name = prof.Gamertag;
#endif
            SetMenuEntryText();
        }

        void Player2MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
#if XBOX
            Gamer prof = SplitScreenHelper.GetGamerProfile(e.PlayerIndex);
            if (prof == null)
            {
                try
                {
                    Guide.ShowSignIn(4, false);
                }
                catch { }
                return;
            }
#endif
            if (p1Index == e.PlayerIndex)
            {
                p2Index = p1Index;
                p1Index = null;
            }
            p2Index = e.PlayerIndex;
#if WINDOWS
            p2Name = "Controller " + e.PlayerIndex.ToString();
#else
            p2Name = prof.Gamertag;
#endif
            SetMenuEntryText();
        }

        void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            // check lobby stats
            if (p1Index != null && p2Index != null)
            {
                if (bgm != null && bgm.IsPlaying) bgm.Stop(AudioStopOptions.Immediate);
                LoadingScreen.Load(ScreenManager, true, SplitScreenHelper.MasterPlayerIndex,
                       new CoopGameplayScreen((PlayerIndex)p1Index, (PlayerIndex)p2Index));
            }

            SetMenuEntryText();
        }

        #endregion
    }
}
