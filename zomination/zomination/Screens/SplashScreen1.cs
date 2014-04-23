using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using GameStateManagement;
using FAZEngine;

namespace zomination
{
    class SplashScreen1 : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D splash1;
        //Video introVideo;
        //VideoPlayer videoPlayer;
        //Texture2D videoTexture;
        int timeElapsedInDisplay = 0;
        bool displayDone = false;
        int DisplayHoldTime;
        InputAction skipAction = new InputAction(new Buttons[]{Buttons.Start, Buttons.A}, new Keys[] { Keys.Space, Keys.Enter}, false);
        Cue bgm;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreen1()
        {
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


                bgm = ((GameMain)ScreenManager.Game).AudioEM.GetCue("introBg1");
                bgm.Play();

                splash1 = content.Load<Texture2D>(@"textures\menu\splash1");
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
                        ScreenManager.AddScreen(new SplashScreen2(bgm), null);
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
            // Only call GetTexture if a video is playing or paused
            //if (videoPlayer.State != MediaState.Stopped)
            //    videoTexture = videoPlayer.GetTexture();

            // Drawing to the rectangle will stretch the 
            // video to fill the screen
            //Point videoSize = new Point(introVideo.Width * 2, introVideo.Height * 2);
            //Rectangle screen = new Rectangle((ScreenManager.GraphicsDevice.Viewport.Width - videoSize.X) / 2,
            //    (ScreenManager.GraphicsDevice.Viewport.Height - videoSize.Y) / 2,
            //    videoSize.X, videoSize.Y);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            // Draw the video, if we have a texture to draw.
            //if (videoTexture != null)
            //{
            //    spriteBatch.Draw(videoTexture, screen, Color.White);
            //}

            spriteBatch.Draw(splash1, new Vector2((GlobalHelper.GameWidth - splash1.Width) / 2, (GlobalHelper.GameHeight - splash1.Height) / 2),
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }

        #endregion
    }
}
