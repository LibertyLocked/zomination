#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using FAZEngine;
#endregion

namespace zomination
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        private const float StartingX = 160;
        private const float StartingY = 400;
        private const float TitleEntrySpacing = 90;
        private ChasingCamera2D camera;
        private ChasingCamera2D camera2;

        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        InputAction menuUp;
        InputAction menuDown;
        InputAction menuSelect;
        InputAction menuCancel;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true);
            menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true);
            menuSelect = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.Enter },
                true);
            menuCancel = new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.Escape },
                true);

            camera = new ChasingCamera2D();
            camera2 = new ChasingCamera2D();
            camera.maxChaseSpeed = 1200;
            //camera.Pos = new Vector2(1920 / 2, 1080 / 2);
            camera2.maxChaseSpeed = 1200;
            //camera2.Pos = new Vector2(1920 / 2, 1080 / 2);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Move to the previous menu entry?
            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                // shake camera slightly
                camera.Shake(150f, 0.06f);
            }

            // Move to the next menu entry?
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;

                // shake camera slightly
                camera.Shake(150f, 0.06f);
            }

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, StartingY + TitleEntrySpacing);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                // each entry is to be centered horizontally
                //position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                position.X = StartingX;

                if (ScreenState == ScreenState.TransitionOn)
                {
                    position.X -= transitionOffset * 512;
                    position.Y -= transitionOffset * 512;
                }
                else
                {
                    position.X += transitionOffset * 512;
                    position.Y += transitionOffset * 512;
                }

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update menu cameras
            Viewport viewport = ScreenManager.Viewport;
            camera.Update(gameTime, new Vector2(viewport.Width / 2, viewport.Height / 2), viewport);
            camera2.Update(gameTime, new Vector2(viewport.Width / 2 * 0.7f, viewport.Height / 2), viewport);
            camera2.Shake(1000, 1f);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont titleFont = ScreenManager.Font2;
            SpriteFont mirrorFont = ScreenManager.Font3;

            //spriteBatch.Begin();
            
            // Draw it twice using 2 different cameras.
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera2.get_transformation(graphics));
            DrawMenuScreen(gameTime, spriteBatch, mirrorFont, 0.2f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.get_transformation(graphics));
            DrawMenuScreen(gameTime, spriteBatch, titleFont, 1);
            spriteBatch.End();
        }


        private void DrawMenuScreen(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont title, float transAlpha)
        {
            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime, transAlpha);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(StartingX, StartingY);
            Vector2 titleOrigin = new Vector2(0, title.MeasureString(menuTitle).Y / 2);
            Color titleColor = new Color(255, 0, 0) * TransitionAlpha * transAlpha;
            float titleScale = 1.00f;

            titlePosition.Y += transitionOffset * 100;
            //titlePosition.X -= transitionOffset * 100;

            spriteBatch.DrawString(title, menuTitle, titlePosition, titleColor, transitionOffset,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

        }

        #endregion

    }
}
