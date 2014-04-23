#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using GameStateManagement;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Environment;
using zomination.Peds;
using zomination.Vehicles;
using zomination.Weapons;
using zomination.ShopScreens;
using ProjectMercury;
using ProjectMercury.Renderers;
#endregion

namespace zomination
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    public class SurvivalGameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        public SpriteFont gameFont;
        public SpriteFont debugFont;
        InputState inputC;

        public World world;
        public Cowboy cowboy;
        public ChasingCamera2D p1Cam;
        public Vector2 p1CamFocus;
        float p1MaxSpeed = 200;
        float p1MaxCamLook = 250;

        // HUD DISPLAY
        StatusDisplay statusDisplay;

        // ALL THE TEXTURES
        public Texture2D blank;
        public Texture2D cowboySheetTexture, zombieSheetTexture, zombieSheetTexture2, evilCowboySheetTexture;
        public Texture2D bobcatTexture, bobcatWheelsTexture;
        public Texture2D crosshairTexture, lazerTexture;

        // ALL THE SOUNDS
        WaveBank weaponWaves;
        SoundBank weaponSounds;
        Cue actionBgm;

        // INPUT ACTIONS
        InputAction pauseAction;

        // PARTICLE SYSTEM
        SpriteBatchRenderer particleRenderer;

        // TEMPORARY SPAWNERS
        int timeSinceLastZombie = 0;
        int timeToSpawnZombie = 5000;

        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SurvivalGameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "main");

                // Start the action background music FIRST
                actionBgm = ((GameMain)ScreenManager.Game).AudioEM.GetCue("actionBg1");
                actionBgm.Play();

                // LOAD FONTS & BLANK TEXTURE
                debugFont = content.Load<SpriteFont>(@"fonts\debug");
                gameFont = content.Load<SpriteFont>(@"fonts\game1");
                blank = content.Load<Texture2D>(@"textures\blank");

                // LOAD SOUNDS
                AudioEngine audioEngine = ((GameMain)ScreenManager.Game).AudioEM.AudioEngine;
                weaponWaves = new WaveBank(audioEngine, @"main\audio\weaponWaves.xwb");
                weaponSounds = new SoundBank(audioEngine, @"main\audio\weaponSounds.xsb");

                // LOAD WEAPONS
                Texture2D bulletPath1 = content.Load<Texture2D>(@"textures\weapons\bulletPath1");
                Texture2D bulletPath2 = content.Load<Texture2D>(@"textures\weapons\bulletPath2");
                lazerTexture = content.Load<Texture2D>(@"textures\weapons\lazer");
                crosshairTexture = content.Load<Texture2D>(@"textures\weapons\crosshair");

                // LOAD VEHICLES
                bobcatTexture = content.Load<Texture2D>(@"textures\vehicles\pickup_nfw");
                bobcatWheelsTexture = content.Load<Texture2D>(@"textures\vehicles\pickup_w");

                // LOAD PED SPRITE TEXTURES
                cowboySheetTexture = content.Load<Texture2D>(@"textures\peds\cowboy1");
                zombieSheetTexture = content.Load<Texture2D>(@"textures\peds\zombie1");
                zombieSheetTexture2 = content.Load<Texture2D>(@"textures\peds\zombie2");
                evilCowboySheetTexture = content.Load<Texture2D>(@"textures\peds\zombiecowboy");

                // LOAD PARTICLE EFFECTS
                particleRenderer = new SpriteBatchRenderer { GraphicsDeviceService = (IGraphicsDeviceService)ScreenManager.Game.Services.GetService(typeof(IGraphicsDeviceService)), };
                particleRenderer.LoadContent(content);
                ParticleEffect bloodBlastEffect = content.Load<ParticleEffect>(@"particles\bloodSplit1");
                ParticleEffect deathBlastEffect = content.Load<ParticleEffect>(@"particles\deathBlast1");
                ParticleEffect powerUpBlastEffect = content.Load<ParticleEffect>(@"particles\powerUpBlast");
                bloodBlastEffect.LoadContent(content); bloodBlastEffect.Initialise();
                deathBlastEffect.LoadContent(content); deathBlastEffect.Initialise();
                powerUpBlastEffect.LoadContent(content); powerUpBlastEffect.Initialise();

                // LOAD MAP TEXTURES
                Vector2 worldSize = new Vector2(4000, 3000);
                Texture2D tileBackgroundTexture = content.Load<Texture2D>(@"textures\map\map" + random.Next(2).ToString());
                Texture2D healthPowerUpTexture = content.Load<Texture2D>(@"textures\powerups\health");
                Texture2D ammoPowerUpTexture = content.Load<Texture2D>(@"textures\powerups\ammo");
                Texture2D moneyPowerUpTexture = content.Load<Texture2D>(@"textures\powerups\money");
                Texture2D revivePowerUpTexture = content.Load<Texture2D>(@"textures\powerups\revive");
                Texture2D shieldPowerUpTexture = content.Load<Texture2D>(@"textures\powerups\shield");
                Texture2D weaponShop = content.Load<Texture2D>(@"textures\map\weaponShop");
                Texture2D modShop = content.Load<Texture2D>(@"textures\map\modShop");
                
                // Construct world
                world = new World(ScreenManager.Game, worldSize, tileBackgroundTexture, new Texture2D[] { bulletPath1, bulletPath2 }, new Texture2D[] { healthPowerUpTexture, ammoPowerUpTexture, moneyPowerUpTexture, revivePowerUpTexture, shieldPowerUpTexture }, new Texture2D[] { weaponShop, modShop }, weaponSounds, new ParticleEffect[] { bloodBlastEffect, deathBlastEffect, powerUpBlastEffect });
                
                // Construct Essential Peds
                cowboy = new Cowboy(new Vector2(1000, 1000), new Vector2(0, -1), cowboySheetTexture, world);

                // give guns to player cowboy
                cowboy.GetGun(new M1911(cowboy, world));
                world.playerList.Add(cowboy);
                cowboy.AddMoney(90);
                world.LoadPresetMap(0, blank); // LOADS PRESET DEBUG MAP
                p1Cam = new ChasingCamera2D();
                p1Cam.maxChaseSpeed = 1200;
                inputC = new InputState();

                // LOAD HUD & ITS ELEMENTS (MUST BE DONE LAST!)
                Texture2D barTexture = content.Load<Texture2D>(@"textures\hud\healthBar");
                Texture2D hudM1911 = content.Load<Texture2D>(@"textures\hud\m1911");
                Texture2D hudAk74u = content.Load<Texture2D>(@"textures\hud\ak74u");
                Texture2D hudM4a1 = content.Load<Texture2D>(@"textures\hud\m4a1");
                Texture2D hudR870mcs = content.Load<Texture2D>(@"textures\hud\r870mcs");
                Texture2D hudM60 = content.Load<Texture2D>(@"textures\hud\m60");
                Texture2D screenBlood = content.Load<Texture2D>(@"textures\hud\screenBlood");
                SpriteFont hudFont = content.Load<SpriteFont>(@"fonts\hud");
                statusDisplay = new StatusDisplay(cowboy, new Vector2(500, 108), barTexture, blank, new Texture2D[] { hudM1911, hudAk74u, hudM4a1, hudR870mcs, hudM60 }, screenBlood, hudFont);

#if XBOX

#else
                FAZEngine.ScriptEngine.ScriptLoader.LoadModScripts(this);
#endif

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            actionBgm.Stop(AudioStopOptions.Immediate);
            content.Unload();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // HANDLE GAME OVER LOGIC HERE
                if (cowboy.Health <= 0 && cowboy.Revives <= 0 && world.SpeedCoeff == 1)
                    ScreenManager.AddScreen(new GameOverScreen((PlayerIndex)ControllingPlayer, null, cowboy, null, actionBgm), ControllingPlayer);

                // Update world and its elements
                world.Update(gameTime);
                // Update inputC (circular dead zone input state)
                inputC.Update(GamePadDeadZone.Circular);
                // Update player1 camera
                if (cowboy.CarDriving == null && cowboy.Health > 0)
                {
                    // normal cam
                    p1Cam.Update(gameTime, p1CamFocus, ScreenManager.Viewport);
                    p1Cam.Rotation = 0;
                    p1Cam.Zoom = 1;
                    // clamp camera within world
                    p1Cam.Pos = Vector2.Clamp(p1Cam.Pos,
                        Vector2.Zero + new Vector2(ScreenManager.Viewport.Width, ScreenManager.Viewport.Height) / 2 - new Vector2(100, 100),
                        world.WorldSize - new Vector2(ScreenManager.Viewport.Width, ScreenManager.Viewport.Height) / 2 + new Vector2(100, 100));
                }
                else if (cowboy.Health > 0)
                {
                    // driving cam
                    p1Cam.Update(gameTime, cowboy.CarDriving.Position + cowboy.CarDriving.CarDirection * ((cowboy.CarDriving.Gear == FAZEngine.Vehicles.Gearbox.D) ? 750 : -750), ScreenManager.Viewport);
                    p1Cam.Rotation = -cowboy.CarDriving.CarRotation;
                    p1Cam.Zoom = 0.5f;
                }
                else
                {
                    // death cam
                    p1Cam.Update(gameTime, cowboy.Position, ScreenManager.Viewport);
                    p1Cam.Zoom = 1.5f;
                    p1Cam.Rotation = 0.2f;
                }

                // Update status display
                statusDisplay.Update(gameTime, p1Cam);

                // TEMPORARY SPAWNERS
                if (cowboy.Health > 0)
                    timeSinceLastZombie += (int)(gameTime.ElapsedGameTime.Milliseconds * world.SpeedCoeff);
                if (timeSinceLastZombie > timeToSpawnZombie || world.enemyList.Count < 12)
                {
                    timeSinceLastZombie -= timeToSpawnZombie;
                    if (world.enemyList.Count < 120)
                    {
                        int numZombieSpawns = Math.Max(3, (cowboy.KillCount) / 100 % 30);
                        for (int i = 0; i < numZombieSpawns; i++)
                        {
                            Texture2D textureToUse = random.Next(2) == 0 ? zombieSheetTexture : zombieSheetTexture2;
                            Zombie zom = new Zombie(cowboy.Position + Vector2Extensions.RotateVector(cowboy.Direction, (float)random.NextDouble() * MathHelper.TwoPi) * 1700, random.Next(90, 180), textureToUse, world);
                            zom.ChangeMaxHealth(100 + (cowboy.KillCount) / 15);
                            world.enemyList.Add(zom);
                        }
                        if (random.Next(11) == 0)
                        {
                            // check if there are evil cowboys in enemylist
                            int evilCowboysInWorld = 0;
                            foreach (Enemy e in world.enemyList)
                            {
                                EvilCowboy ec = e as EvilCowboy;
                                if (ec != null) evilCowboysInWorld++;
                            }
                            if (evilCowboysInWorld < 2)
                            {
                                EvilCowboy evilCowboy = new EvilCowboy(cowboy.Position + Vector2Extensions.RotateVector(cowboy.Direction, (float)random.NextDouble() * MathHelper.TwoPi) * 1700, evilCowboySheetTexture, world);
                                evilCowboy.ChangeMaxHealth(evilCowboy.MaxHealth + (cowboy.KillCount) / 3);
                                world.enemyList.Add(evilCowboy);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
            GamePadState lastGamePadState = input.LastGamePadStates[playerIndex];
            GamePadState gamePadStateC = inputC.CurrentGamePadStates[playerIndex];  // gps with circular deadzone
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            KeyboardState lastKeyboardState = input.LastKeyboardStates[playerIndex];
            MouseState mouseState = input.CurrentMouseState;
            MouseState lastMouseState = input.LastMouseState;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // when cowboy's not in a vehicle
                if (cowboy.CarDriving == null && cowboy.Health > 0)
                {
                    if (GlobalHelper.UseGamePad)
                    {
                        Vector2 p1LStick = gamePadState.ThumbSticks.Left;
                        Vector2 p1Movement = new Vector2(p1LStick.X, -p1LStick.Y);
                        if (p1Movement != Vector2.Zero) p1Movement.Normalize();
                        cowboy.Move(p1Movement * p1MaxSpeed);

                        // GAMEPAD WEAPON CONTROLS
                        if (cowboy.GunsOwned.Count > 0)
                        {
                            if (gamePadState.IsButtonDown(Buttons.Y) && lastGamePadState.IsButtonUp(Buttons.Y))
                                cowboy.SwapGun();
                            if (gamePadState.IsButtonDown(Buttons.LeftShoulder))
                                cowboy.ReloadGun();
                            if (cowboy.GunsOwned[cowboy.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Auto)
                            {
                                if (gamePadState.IsButtonDown(Buttons.RightTrigger))
                                    cowboy.HitGunTrigger();
                            }
                            else if (cowboy.GunsOwned[cowboy.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Single)
                            {
                                if (gamePadState.IsButtonDown(Buttons.RightTrigger) && lastGamePadState.IsButtonUp(Buttons.RightTrigger))
                                    cowboy.HitGunTrigger();
                            }
                        }
                    }
                    else
                    {
                        Vector2 keyboardMoveVec = Vector2.Zero;
                        if (keyboardState.IsKeyDown(Keys.W)) keyboardMoveVec += new Vector2(0, -1);
                        if (keyboardState.IsKeyDown(Keys.S)) keyboardMoveVec += new Vector2(0, 1);
                        if (keyboardState.IsKeyDown(Keys.A)) keyboardMoveVec += new Vector2(-1, 0);
                        if (keyboardState.IsKeyDown(Keys.D)) keyboardMoveVec += new Vector2(1, 0);
                        if (keyboardMoveVec != Vector2.Zero) keyboardMoveVec.Normalize();
                        cowboy.Move(keyboardMoveVec * p1MaxSpeed);

                        // MOUSE/KEYBOARD WEAPON CONTROLS
                        if (cowboy.GunsOwned.Count > 0)
                        {
                            if (mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue)
                                cowboy.SwapGun(cowboy.GunEquipped + 1);
                            if (mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue)
                                cowboy.SwapGun(cowboy.GunEquipped - 1);
                            if (keyboardState.IsKeyDown(Keys.R))
                                cowboy.ReloadGun();
                            if (cowboy.GunsOwned[cowboy.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Auto)
                            {
                                if (mouseState.LeftButton == ButtonState.Pressed)
                                    cowboy.HitGunTrigger();
                            }
                            else if (cowboy.GunsOwned[cowboy.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Single)
                            {
                                if (mouseState.LeftButton == ButtonState.Pressed &&
                                    lastMouseState.LeftButton == ButtonState.Released)
                                    cowboy.HitGunTrigger();
                            }
                        }
                    }
                }
                // when cowboy's in a vehicle
                else if (cowboy.Health > 0)
                {
                    if (GlobalHelper.UseGamePad)
                    {
                        if (gamePadState.Triggers.Left != 0) cowboy.HitCarBrake(gamePadState.Triggers.Left);
                        else cowboy.HitCarGas(gamePadState.Triggers.Right);
                        cowboy.TurnCarWheels(gamePadState.ThumbSticks.Left.X);
                        if (gamePadState.IsButtonDown(Buttons.DPadLeft) && lastGamePadState.IsButtonUp(Buttons.DPadLeft))
                            cowboy.ShiftCarGear((FAZEngine.Vehicles.Gearbox)(((int)cowboy.CarDriving.Gear + 1) % 2));
                    }
                    else
                    {
                        if (keyboardState.IsKeyDown(Keys.S)) cowboy.HitCarBrake(1);
                        if (keyboardState.IsKeyDown(Keys.W)) cowboy.HitCarGas(1);
                        if (keyboardState.IsKeyDown(Keys.A)) cowboy.TurnCarWheels(-1);
                        if (keyboardState.IsKeyDown(Keys.D)) cowboy.TurnCarWheels(1);
                        if (keyboardState.IsKeyDown(Keys.LeftShift) && lastKeyboardState.IsKeyUp(Keys.LeftShift))
                        {
                            cowboy.ShiftCarGear((FAZEngine.Vehicles.Gearbox)(((int)cowboy.CarDriving.Gear + 1) % 2));
                        }
                        // fix wheels
                        if (keyboardState.IsKeyUp(Keys.A) && keyboardState.IsKeyUp(Keys.D)) cowboy.TurnCarWheels(0);
                        if (keyboardState.IsKeyUp(Keys.W) && keyboardState.IsKeyUp(Keys.S)) cowboy.HitCarGas(0);
                    }
                }

                if (GlobalHelper.UseGamePad)
                {
                    // Right thumbstick to aim / look around
                    // Use gpsC instead (for better accuracy)
                    Vector2 p1RStick = gamePadStateC.ThumbSticks.Right;
                    float p1RStickLength = MathHelper.Clamp(p1RStick.Length(), 0, 1);
                    Vector2 p1CamLook = new Vector2(p1RStick.X, -p1RStick.Y);
                    if (p1CamLook != Vector2.Zero) p1CamLook.Normalize();
                    cowboy.Aim(p1CamLook);
                    p1CamFocus = cowboy.Position + p1CamLook * p1MaxCamLook * p1RStickLength;
                }
                else
                {
                    // use mouse to look around
                    Vector2 p1CamLook = Vector2.Zero;
                    Vector2 mouseOrigin = new Vector2(mouseState.X, mouseState.Y) - GlobalHelper.RenderOrigin;
                    p1CamLook = new Vector2(mouseOrigin.X * GlobalHelper.GameWidth / GlobalHelper.RenderWidth - GlobalHelper.GameWidth / 2 + p1Cam.Pos.X - cowboy.Position.X,
                       mouseOrigin.Y * GlobalHelper.GameHeight / GlobalHelper.RenderHeight - GlobalHelper.GameHeight / 2 + p1Cam.Pos.Y - cowboy.Position.Y);
                    if (p1CamLook.Length() > p1MaxCamLook)
                    {
                        p1CamLook.Normalize();
                        p1CamLook *= p1MaxCamLook;
                    }
                    cowboy.Aim(p1CamLook);
                    p1CamFocus = cowboy.Position + p1CamLook;
                }

                // TESTING ONLY: allow user to spawn stuff
                // using keyboard
#if DEBUG
                if (keyboardState.IsKeyDown(Keys.F1) && lastKeyboardState.IsKeyUp(Keys.F1))
                    world.basicCarList.Add(new Bobcat(cowboy.Position + cowboy.Direction * 500,
                        Vector2Extensions.VectorToRadians(cowboy.Direction), bobcatTexture, bobcatWheelsTexture, world));
                if (keyboardState.IsKeyDown(Keys.F2) && lastKeyboardState.IsKeyUp(Keys.F2))
                    world.enemyList.Add(new Zombie(cowboy.Position + cowboy.Direction * 500,
                        100, zombieSheetTexture, world));
                if (keyboardState.IsKeyDown(Keys.F3) && lastKeyboardState.IsKeyUp(Keys.F3))
                    world.enemyList.Add(new EvilCowboy(cowboy.Position + cowboy.Direction * 1000, cowboySheetTexture, world));
                if (keyboardState.IsKeyDown(Keys.F4))
                    cowboy.AddMoney(100);
#endif
                // RESPAWN PLAYER automatically
                if (!world.playerList.Contains(cowboy) && cowboy.Revives > 0 && world.SpeedCoeff == 1)
                {
                    cowboy.Lost1Revive();
                    cowboy.Position = new Vector2(random.Next((int)world.WorldSize.X), random.Next((int)world.WorldSize.Y));
                    cowboy.GainHealth(1000);
                    cowboy.GetMaxAmmo(true);
                    cowboy.SetInvincible(3000);
                    world.playerList.Add(cowboy);
                }
                if (keyboardState.IsKeyDown(Keys.D2) || (gamePadState.IsButtonDown(Buttons.DPadLeft) && lastGamePadState.IsButtonUp(Buttons.DPadLeft)))
                {
                    //if (cowboy.Health > 0 && cowboy.InShop != null)
                    //{
                    //    if (cowboy.InShop.Name == "WeaponShop")
                    //        ScreenManager.AddScreen(new WeaponShopScreen(0, cowboy, world), ControllingPlayer);
                    //    else if (cowboy.InShop.Name == "ModShop")
                    //        ScreenManager.AddScreen(new WeaponModScreen(0, cowboy, cowboy.GunsOwned[cowboy.GunEquipped], world), ControllingPlayer);
                    //}
                    if (cowboy.Health > 0)
                    {
                        ScreenManager.AddScreen(new WeaponShopScreen(0, cowboy, world), ControllingPlayer);
                    }
                }

                if (keyboardState.IsKeyDown(Keys.D3) || (gamePadState.IsButtonDown(Buttons.DPadRight) && lastGamePadState.IsButtonUp(Buttons.DPadRight)))
                {
                    if (cowboy.Health > 0)
                    {
                        ScreenManager.AddScreen(new WeaponModScreen(0, cowboy, cowboy.GunsOwned[cowboy.GunEquipped], world), ControllingPlayer);
                    }
                }

                // Get in/out car controls
                if (cowboy.Health > 0 && ((gamePadState.IsButtonDown(Buttons.Y) && lastGamePadState.IsButtonUp(Buttons.Y)) ||
                    (keyboardState.IsKeyDown(Keys.F) && lastKeyboardState.IsKeyUp(Keys.F))))
                {
                    if (cowboy.CarDriving == null) cowboy.GetInCar();
                    else cowboy.GetOutCar();
                }

                // Cam zoom level controls
                //if (gamePadState.DPad.Down == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Down)) p1Cam.Zoom = 0.5f;

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Draw under CAMERA
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, p1Cam.get_transformation(ScreenManager.Viewport));

            // draw world
            world.Draw(spriteBatch);
            
            // draw cowboy's crosshair
            if (cowboy.CarDriving == null && cowboy.Health > 0)
            {
                Vector2 crosshairSize = new Vector2(crosshairTexture.Width, crosshairTexture.Height);
                spriteBatch.Draw(crosshairTexture, cowboy.Position + cowboy.Direction * 200,
                    null,
                    cowboy.GunsOwned[cowboy.GunEquipped].ClipAmmo > 0 && !cowboy.GunsOwned[cowboy.GunEquipped].Reloading ?
                    Color.YellowGreen : Color.Red,
                    cowboy.GunsOwned[cowboy.GunEquipped].Reloading ?
                    (float)Math.PI * cowboy.GunsOwned[cowboy.GunEquipped].ReloadPosition : 0,
                    crosshairSize / 2,
                    cowboy.GunsOwned[cowboy.GunEquipped].RecoilClimb * 15 + 1,
                    SpriteEffects.None, 1f);

                if (cowboy.GunsOwned[cowboy.GunEquipped].HasLazer)
                    spriteBatch.Draw(lazerTexture, cowboy.Position + cowboy.Direction * 48, null,
                        Color.White, Vector2Extensions.VectorToRadians(cowboy.Direction),
                        new Vector2(lazerTexture.Width / 2, lazerTexture.Height), 1,
                        SpriteEffects.None, 1f);
            }

            spriteBatch.End();

            // DRAW PARTICLE SYSTEM
            spriteBatch.Begin();
            foreach (ParticleEffect p in world.particleEffects)
            {
                Matrix camMatrix = p1Cam.get_transformation(ScreenManager.Viewport);
                particleRenderer.RenderEffect(p , ref camMatrix);
            }
            spriteBatch.End();

            // DRAW UNDER NORMAL VIEW
            spriteBatch.Begin();

            // draw status display
            statusDisplay.Draw(spriteBatch, ScreenManager.Viewport);

            // draw a string "press B to revive" in the middle
            if (cowboy.Health <= 0)
            {
                string reviveStr;
                if (cowboy.Revives > 0)
                    reviveStr = cowboy.Revives + " li" + (cowboy.Revives <= 1 ? "fe" : "ves") + " left" +
                        (world.SpeedCoeff == 1 ? "\n[Press B] to respawn." : "");
                else
                    reviveStr = "NO LIFE LEFT!";
                Vector2 measure = gameFont.MeasureString(reviveStr);
                Vector2 posToDrawStr = new Vector2(GlobalHelper.GameWidth, GlobalHelper.GameHeight) / 2 - measure / 2;
                spriteBatch.Draw(blank, new Rectangle((int)posToDrawStr.X, (int)posToDrawStr.Y, (int)measure.X, (int)measure.Y), Color.FromNonPremultiplied(0, 0, 0, 140));
                spriteBatch.DrawString(gameFont, reviveStr, posToDrawStr, Color.White);
            }

            // output debug messages
#if DEBUG
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(p1Cam.Pos.ToString() +
                "\nResolution: " + GlobalHelper.RenderWidth + " x " + GlobalHelper.RenderHeight +
                "\nNativeRendering: " + GlobalHelper.IsRenderNative + 
                "\nMSAA: " + ScreenManager.GraphicsDevice.PresentationParameters.MultiSampleCount +
                "\nTexture Filter: " + (GlobalHelper.IsRenderNative ? "N/A" : GlobalHelper.SamplerState.ToString()) + 
                "\nzoom: " + p1Cam.Zoom + "x" +
                "\nworld_speedCoeff: " + world.SpeedCoeff.ToString("p1") +
                "\nworld_carsCount: " + world.basicCarList.Count +
                "\nworld_bulletCount: " + world.bulletList.Count +
                "\nworld_enemyCount: " + world.enemyList.Count +
                "\np1_pos: {" + cowboy.Position.X + "," + cowboy.Position.Y + "}" +
                "\np1_v: {" + cowboy.Speed.X + "," + cowboy.Speed.Y + "}" +
                "\np1_dir: {" + cowboy.Direction.X + "," + cowboy.Direction.Y + "}" +
                "\np1_killCount: " + cowboy.KillCount + 
                "\np1_health: " + cowboy.Health + "/" + cowboy.MaxHealth +
                "\np1_equippedGunRecoil: " + cowboy.GunsOwned[cowboy.GunEquipped].RecoilClimb +
                "\nequipped_gun_ammo: " + cowboy.GunsOwned[cowboy.GunEquipped].ClipAmmo + "/" + cowboy.GunsOwned[cowboy.GunEquipped].ReserveAmmo +
                (cowboy.CarDriving == null ? "" :
                "\ncar_pos: {" + cowboy.CarDriving.Position.X + "," + cowboy.CarDriving.Position.Y + "}" +
                "\ncar_speed: " + cowboy.CarDriving.GetSpeed() +
                "\ncar_acc: " + cowboy.CarDriving.GetAcceleration() +
                "\ncar_cDir: {" + cowboy.CarDriving.CarDirection.X + "," + cowboy.CarDriving.CarDirection.Y + "}" +
                "\ncar_wDir: {" + cowboy.CarDriving.WheelsDirection.X + "," + cowboy.CarDriving.WheelsDirection.Y + "}" +
                "\ncar_gear: " + cowboy.CarDriving.Gear +
                "\ncar_health: " + cowboy.CarDriving.Health + "/" + cowboy.CarDriving.MaxHealth) +
                "\nScriptEngine: " + Scripts.Count + " loaded:\n");
            if (Scripts.Count > 0)
                foreach (FAZEngine.ScriptEngine.Script s in Scripts)
                    sb.AppendLine("[" + Scripts.IndexOf(s) + "] " + s.FileName);

            spriteBatch.DrawString(debugFont, sb.ToString(),
                Vector2.Zero, Color.YellowGreen);
#endif
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion
    }
}
