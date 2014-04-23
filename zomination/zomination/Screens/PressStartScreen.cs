using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using FAZEngine;

namespace zomination
{
    class PressStartScreen : GameScreen
    {
        ContentManager content;
        Texture2D title;
        SpriteFont font;
        ChasingCamera2D mirrorCamera;
        float alpha = 0;
        float alphaIncrement = 0.02f;
        const string pressStartString = "Press START to continue";
        bool pressed = false;

        Cue bgm;
        InputAction pressStartAction;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PressStartScreen(Cue bgm)
        {
            this.bgm = bgm;
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pressStartAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.A },
                new Keys[] { Keys.Enter },
                true);

            mirrorCamera = new ChasingCamera2D();
            mirrorCamera.Pos = new Vector2(GlobalHelper.GameWidth / 2, GlobalHelper.GameHeight / 2);
            //mirrorCamera.maxChaseSpeed *= 4;
        }

        /// <summary>
        /// Loads graphics contents for press start screen.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "main");

                title = content.Load<Texture2D>(@"textures\menu\title");
                font = content.Load<SpriteFont>(@"fonts\menuEntry");
                if (bgm == null || !bgm.IsPlaying)
                {
                    bgm = ((GameMain)ScreenManager.Game).AudioEM.GetCue("introBg1");
                    bgm.Play();
                }
            }
        }

        /// <summary>
        /// Unload graphics contents for press start screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the splash screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                               bool coveredByOtherScreen)
        {
            // Update mirror
            Viewport viewport = ScreenManager.Viewport;
            mirrorCamera.Update(gameTime, new Vector2(viewport.Width / 2 * 0.7f, viewport.Height / 2), viewport);
            mirrorCamera.Shake(1000, 1f);

            // Update alpha for press start string
            alpha += alphaIncrement;
            if (alpha > 1)
            {
                alpha = 1;
                alphaIncrement *= -1;
            }
            else if (alpha < 0)
            {
                alpha = 0;
                alphaIncrement *= -1;
            }

            if (pressed)
            {
                
                this.ExitScreen();
                if (bgm != null) bgm.Stop(AudioStopOptions.Immediate);
                BackgroundScreen bgScreen = new BackgroundScreen();
                ScreenManager.AddScreen(bgScreen, null);
                ScreenManager.AddScreen(new MainMenuScreen(bgScreen), null);
                pressed = false;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the press start screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Color pressStartColor = Color.Red * alpha;

            // Draw mirror
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, mirrorCamera.get_transformation(ScreenManager.GraphicsDevice));

            spriteBatch.Draw(title, new Vector2((GlobalHelper.GameWidth - title.Width) / 2, (GlobalHelper.GameHeight - title.Height) / 2),
                             new Color(TransitionAlpha * 0.3f, TransitionAlpha * 0.3f, TransitionAlpha * 0.3f));

            pressStartColor *= 0.3f;

            spriteBatch.DrawString(font, pressStartString,
                new Vector2(GlobalHelper.GameWidth / 2, GlobalHelper.GameHeight / 2 + 290),
                pressStartColor, 0,
                font.MeasureString(pressStartString) / 2, 1f, SpriteEffects.None, 1f);

            spriteBatch.End();

            // Draw original
            spriteBatch.Begin();

            spriteBatch.Draw(title, new Vector2((GlobalHelper.GameWidth - title.Width) / 2, (GlobalHelper.GameHeight - title.Height) / 2),
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            pressStartColor = Color.Red * alpha;

            spriteBatch.DrawString(font, pressStartString, 
                new Vector2(GlobalHelper.GameWidth / 2, GlobalHelper.GameHeight / 2 + 290),
                pressStartColor, 0, 
                font.MeasureString(pressStartString) / 2, 1f, SpriteEffects.None, 1f);

            spriteBatch.End();
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            PlayerIndex player;
            if (pressStartAction.Evaluate(input, null, out player))
            {
#if XBOX
                if (!SplitScreenHelper.CheckSignIn(player)) return;
#endif
                SplitScreenHelper.MasterPlayerIndex = player;
                pressed = true;
            }
        }
    }
}
