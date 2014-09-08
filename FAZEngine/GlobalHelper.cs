using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine
{
    public static class GlobalHelper
    {
        // Profile
        public static int ActiveProfile
        {
            get;
            set;
        }

        // SampleState for render target
        public static SamplerState SamplerState = SamplerState.LinearWrap;

        // UseGamePad
        public static bool UseGamePad = false;

        // Permanent resolution that the game is rendering on
        public const int GameWidth = 1920;
        public const int GameHeight = 1080;

        /// <summary>
        /// Gets the resolution width at which the game is rendering.
        /// </summary>
        public static int RenderWidth
        {
            get { return WindowWidth; }
        }

        /// <summary>
        /// Gets the resolution height at which the game is rendering.
        /// </summary>
        public static int RenderHeight
        {
            get { return (int)((float)WindowWidth / GameWidth * GameHeight); }
        }

        public static int WindowWidth;
        public static int WindowHeight;

        /// <summary>
        /// A get only property indicating whether the game is rendering at native resolution.
        /// </summary>
        public static bool IsRenderNative
        {
            get { return (GameWidth == WindowWidth && GameHeight == WindowHeight); }
        }

        public static Vector2 RenderOrigin
        {
            get { return new Vector2(0, (GlobalHelper.WindowHeight - GlobalHelper.RenderHeight) / 2); }
        }
    }
}
