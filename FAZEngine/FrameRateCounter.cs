using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace FAZEngine
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FrameRateCounter : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
#if WINDOWS
        Process proc;
#endif

        private double memUsed = 0;
        private double memUsedCounter = 0;
        private int frameRate = 0;
        private int frameCounter = 0;

        private TimeSpan elapsedTime = TimeSpan.Zero;

        private const int refreshRatePerSec = 2;
        private const int expectedFps = 60;
        private const double warningWhenFpsIsBelow = 0.9;
        private const double warning2WhenFpsIsBelow = 0.6;

        public FrameRateCounter(Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            LoadGraphicsContent(true);
            this.spriteBatch = spriteBatch;
#if WINDOWS
            proc = Process.GetCurrentProcess();
#endif
        }

        protected void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                spriteFont = Game.Content.Load<SpriteFont>(@"fonts\debug");
            }
        }

        public override void Update(GameTime gameTime)
        {
#if WINDOWS
            proc.Refresh();
            memUsedCounter = Math.Round((double)proc.PrivateMemorySize64 / 1024 / 1024, 0);
#endif

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1.0 / refreshRatePerSec))
            {
                elapsedTime -= TimeSpan.FromSeconds(1.0 / refreshRatePerSec);
                frameRate = frameCounter * refreshRatePerSec;
                frameCounter = 0;
                memUsed = memUsedCounter;
                memUsedCounter = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
#if WINDOWS
            string fpsString = string.Format("{0} ({1} MB) {2} Profile: {3}", frameRate, string.Format("{0:n0}", memUsed), Mouse.GetState(), GlobalHelper.ActiveProfile);
#else
            string fpsString = string.Format("{0}", frameRate);
#endif
            //string fpsString = string.Format("{0}", frameRate);

            Color colorToDraw;
            if (frameRate < expectedFps * warning2WhenFpsIsBelow)
            {
                colorToDraw = Color.Red;
            }
            else if (frameRate < expectedFps * warningWhenFpsIsBelow)
            {
                colorToDraw = Color.Yellow;
            }
            else
            {
                colorToDraw = Color.DarkGreen;
            }

            spriteBatch.Begin();

            // draw framerate and allocated memory
            spriteBatch.DrawString(spriteFont, fpsString, new Vector2(0,
                GlobalHelper.WindowHeight - spriteFont.MeasureString(fpsString).Y), colorToDraw);

            spriteBatch.End();
        }
    }
}
