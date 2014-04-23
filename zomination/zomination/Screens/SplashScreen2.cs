using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using FAZEngine;

namespace zomination
{
    class SplashScreen2 : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D splash1;
        Cue bgm;
        int timeElapsedInDisplay = 0;
        bool displayDone = false;
        int DisplayHoldTime;
        InputAction skipAction = new InputAction(new Buttons[] { Buttons.Start, Buttons.A }, new Keys[] { Keys.Space, Keys.Enter }, false);

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreen2(Cue bgm)
        {
            this.bgm = bgm;
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }


        /// <summary>
        /// Loads graphics contents for splash screen.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "main");

                splash1 = content.Load<Texture2D>(@"textures\menu\splash2");
                //introVideo = content.Load<Video>(@"movies\LOGO_5M");

                //videoPlayer = new VideoPlayer();
                //videoPlayer.Play(introVideo);

                DisplayHoldTime = 3500;
                //DisplayHoldTime = (int)introVideo.Duration.TotalMilliseconds;
            }
        }


        /// <summary>
        /// Unload graphics contents for splash screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the splash screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            timeElapsedInDisplay += gameTime.ElapsedGameTime.Milliseconds;
            if (timeElapsedInDisplay > DisplayHoldTime)
            {
                if (!displayDone)
                {
                    // Now the screen should transition off
                    displayDone = true;
                    this.ExitScreen();
                }
                else
                {
                    // Check transition status, when it's fully off, add menu and background screens.
                    if (this.TransitionPosition > 0.95)
                    {
                        ScreenManager.RemoveScreen(this);
                        ScreenManager.AddScreen(new PressStartScreen(bgm), null);
                    }
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex player;
            if (skipAction.Evaluate(input, null, out player))
            {
                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(new PressStartScreen(bgm), null);
            }
            base.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Draws the splash screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            Vector2 posImage = new Vector2((GlobalHelper.GameWidth - splash1.Width) / 2, (GlobalHelper.GameHeight - splash1.Height) / 2);

            spriteBatch.Draw(splash1, posImage,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            Vector2 measure = ScreenManager.Font.MeasureString("3LT Game Studio");
            Vector2 posText = new Vector2((GlobalHelper.GameWidth - measure.X) / 2, GlobalHelper.GameHeight / 2 + 130);

            spriteBatch.DrawString(ScreenManager.Font, "3LT Game Studio", posText, Color.FromNonPremultiplied(255, 255, 255, (int)(255*TransitionAlpha)));

            spriteBatch.End();
        }

        #endregion
    }
}
