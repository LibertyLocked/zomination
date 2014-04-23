#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using GameStateManagement;
#endregion

namespace zomination
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        InputAction controllerResumeAction;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry howToPlayMenuEntry = new MenuEntry("How to Play");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry quitGameMenuEntry = new MenuEntry("Exit to Menu");

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(howToPlayMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

            controllerResumeAction = new InputAction(new Buttons[] { Buttons.Start }, null, true);
        }

        public override void Activate(bool instancePreserved)
        {
//#if XBOX
//            if (Guide.IsTrialMode)
//                ScreenManager.AddScreen(new UnlockFullVerScreen(), ControllingPlayer);
//#endif
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input)
        {
            PlayerIndex playerIndex;

            if (controllerResumeAction.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }

            base.HandleInput(gameTime, input);
        }


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ConfirmExitToMenuScreen(), ControllingPlayer);
        }

        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), ControllingPlayer);
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
    }
}
