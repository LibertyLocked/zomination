using System;
using System.Collections.Generic;
using System.Linq;
using FAZEngine;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace zomination
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;    // this is for FPS counter
        AudioEM audioEM;

        ScreenManager screenManager;

        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public AudioEM AudioEM
        {
            get { return audioEM; }
        }

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "main";

#if XBOX
            // Create Gamer Service Component
            SignedInGamer.SignedOut += new EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
            GamerServicesComponent gamerServiceComp = new GamerServicesComponent(this);
            Components.Add(gamerServiceComp);
#if DEBUG
            Guide.SimulateTrialMode = true;
#endif
#endif

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // On Windows and Xbox we just add the initial screens
            AddInitialScreens();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Applying graphics settings

#if WINDOWS
            // ini settings file
            FAZEngine.IniFile settingsIni = new FAZEngine.IniFile(@".\configs\settings.ini");

            // fullscreen
            graphics.IsFullScreen = bool.Parse(settingsIni.IniReadValue("Options", "Fullscreen"));

            // resolution settings
            int sWidth = 0, sHeight = 0;
            if (int.TryParse(settingsIni.IniReadValue("Resolution", "Width"), out sWidth) &&
                int.TryParse(settingsIni.IniReadValue("Resolution", "Height"), out sHeight))
            {
                GlobalHelper.WindowWidth = sWidth;
                GlobalHelper.WindowHeight = sHeight;
            }
            else
            {
                GlobalHelper.WindowWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                GlobalHelper.WindowHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            }

            // use gamepad settings
            GlobalHelper.UseGamePad = bool.Parse(settingsIni.IniReadValue("Options", "UseGamePad"));
            IsMouseVisible = !GlobalHelper.UseGamePad;
#else
            // START UP SETTINGS FOR XBOX
            GlobalHelper.UseGamePad = true;
            GlobalHelper.WindowWidth = GraphicsDevice.DisplayMode.Width;
            GlobalHelper.WindowHeight = GraphicsDevice.DisplayMode.Height;
            // if it's in 4:3 but "widescreen" at the same time, just use a fake 16:9 resolution
            if (GraphicsDevice.DisplayMode.AspectRatio == (float)4 / 3 && GraphicsDevice.Adapter.IsWideScreen)
            {
                GlobalHelper.WindowWidth = 1280;
                GlobalHelper.WindowHeight = 720;
            }
#endif
            // msaa settings is done when preparing
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            graphics.PreferredBackBufferWidth = GlobalHelper.WindowWidth;
            graphics.PreferredBackBufferHeight = GlobalHelper.WindowHeight;
            //graphics.PreferMultiSampling = true;
            //GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            //graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            // After loading graphics settings, load audio engine manager
            audioEM = new AudioEM(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
#if XBOX
            Guide.NotificationPosition = NotificationPosition.TopRight;
#endif

#if DEBUG
            FrameRateCounter fpsCounter = new FrameRateCounter(this, spriteBatch);
            Components.Add(fpsCounter);
#endif
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            AudioEM.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new SplashScreen1(), null);
        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
#if WINDOWS
            FAZEngine.IniFile settingsIni = new FAZEngine.IniFile(@".\configs\settings.ini");
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = int.Parse(settingsIni.IniReadValue("Options", "MSAA"));
#else
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
#endif
        }

#if XBOX
        void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
        {
            LoadingScreen.Load(screenManager, false, null, new PressStartScreen(null));
            // reload audioEM = new AudioEM(this);
            audioEM = new AudioEM(this);
        }
#endif
    }
}
