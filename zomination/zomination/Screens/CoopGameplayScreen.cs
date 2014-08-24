#region Using Statements
using System;
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
    class CoopGameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont, debugFont;
        InputState inputC;

        public World world;
        public Cowboy p1, p2;
        ChasingCamera2D p1Cam, p2Cam;

        Vector2 p1CamFocus, p2CamFocus;
        float p1MaxSpeed = 200;
        float p1MaxCamLook = 250;
        float p2MaxSpeed = 200;
        float p2MaxCamLook = 250;

        // PlayerIndexs and vieports
        PlayerIndex p1Index, p2Index;
        Viewport p1Viewport, p2Viewport;

        // HUD DISPLAY
        StatusDisplay p1StatDisplay, p2StatDisplay;

        // ALL THE TEXTURES
        Texture2D blank;
        Texture2D cowboySheetTexture, zombieSheetTexture, zombieSheetTexture2, evilCowboySheetTexture;
        Texture2D bobcatTexture, bobcatWheelsTexture;
        Texture2D crosshairTexture, lazerTexture;

        // ALL THE SOUNDS
        WaveBank weaponWaves;
        SoundBank weaponSounds;
        Cue actionBgm;

        // INPUT ACTIONS
        InputAction pauseAction;

        // PARTICLE SYSTEM
        SpriteBatchRenderer particleRenderer;

        // Player Gaining Revive Timer
        int reviveTimer = 0;
        const int timeToRevive = 60000;

        // transfer money Hold button timer
        int transferHoldTimer = 0;
        const int transferHoldTime = 500;

        // TEMPORARY SPAWNERS
        int timeSinceLastZombie = 0;
        const int timeToSpawnZombie = 3000;

        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CoopGameplayScreen(PlayerIndex p1Index, PlayerIndex p2Index)
        {
            this.p1Index = p1Index;
            this.p2Index = p2Index;

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

                // SET UP VIEWPORTS
                Viewport[] viewports = SplitScreenHelper.GetViewports(ScreenManager.Viewport, 2);
                p1Viewport = viewports[0]; p2Viewport = viewports[1];

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
                p1 = new Cowboy(new Vector2(1000, 1000), new Vector2(0, -1), cowboySheetTexture, world);
                p2 = new Cowboy(new Vector2(1000, 1200), new Vector2(0, 1), cowboySheetTexture, world);

                // give guns to players
                p1.GetGun(new M1911(p1, world));
                p2.GetGun(new M1911(p2, world));
                world.playerList.Add(p1);
                world.playerList.Add(p2);
                p1.AddMoney(90);
                p2.AddMoney(90);
                world.LoadPresetMap(0, blank); // LOADS PRESET DEBUG MAP
                p1Cam = new ChasingCamera2D() { maxChaseSpeed = 1200 };
                p2Cam = new ChasingCamera2D() { maxChaseSpeed = 1200 };
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
                p1StatDisplay = new StatusDisplay(p1, new Vector2(500, 108), barTexture, blank, new Texture2D[] { hudM1911, hudAk74u, hudM4a1, hudR870mcs, hudM60 }, screenBlood, hudFont);
                p2StatDisplay = new StatusDisplay(p2, new Vector2(500, 108), barTexture, blank, new Texture2D[] { hudM1911, hudAk74u, hudM4a1, hudR870mcs, hudM60 }, screenBlood, hudFont);

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
                if (p1.Health <= 0 && p1.Revives <= 0 && p2.Health <= 0 && p2.Revives <= 0 && world.SpeedCoeff == 1)
                    ScreenManager.AddScreen(new GameOverScreen(p1Index, p2Index, p1, p2, actionBgm), null);

                // Update world and its elements
                world.Update(gameTime);
                // Update inputC (circular dead zone input state)
                inputC.Update(GamePadDeadZone.Circular);

                // Update playerCameras
                for (int i = 0; i < 2; i++)
                {
                    Player playerActive = (i == 0) ? p1 : p2;
                    ChasingCamera2D playerCam = (i == 0) ? p1Cam : p2Cam;
                    Viewport playerViewport = (i == 0) ? p1Viewport : p2Viewport;
                    if (playerActive.CarDriving == null && playerActive.Health > 0)
                    {
                        // normal cam
                        playerCam.Update(gameTime, (i == 0) ? p1CamFocus : p2CamFocus, (i == 0) ? p1Viewport : p2Viewport);
                        playerCam.Rotation = 0;
                        playerCam.Zoom = 0.8f;
                        // clamp the camera within world 
                        playerCam.Pos = Vector2.Clamp(playerCam.Pos,
                            Vector2.Zero + new Vector2(playerViewport.Width, playerViewport.Height) / playerCam.Zoom / 2 - new Vector2(100, 100),
                            world.WorldSize - new Vector2(playerViewport.Width, playerViewport.Height) / playerCam.Zoom / 2 + new Vector2(100, 100));

                    }
                    else if (playerActive.Health > 0)
                    {
                        // driving cam
                        playerCam.Update(gameTime, playerActive.CarDriving.Position + playerActive.CarDriving.CarDirection * ((playerActive.CarDriving.Gear == FAZEngine.Vehicles.Gearbox.D) ? 600 : -600), (i == 0) ? p1Viewport : p2Viewport);
                        playerCam.Rotation = -playerActive.CarDriving.CarRotation;
                        playerCam.Zoom = 0.4f;
                    }
                    else
                    {
                        // death cam
                        playerCam.Update(gameTime, playerActive.Position, (i == 0) ? p1Viewport : p2Viewport);
                        playerCam.Zoom = 1.4f;
                        if (playerActive.Revives <= 0) playerCam.Rotation += 0.001f;
                        else playerCam.Rotation = 0.2f;
                    }

                    // Update status display
                    if (i == 0) p1StatDisplay.Update(gameTime, p1Cam);
                    else p2StatDisplay.Update(gameTime, p2Cam);
                }

                // Updates Revive Timer if they are out of revives
                if ((p1.Health <= 0 && p1.Revives <= 0) || (p2.Health <= 0 && p2.Revives <= 0))
                {
                    if (p1.Health > 0 || p2.Health > 0)
                        reviveTimer += (int)(gameTime.ElapsedGameTime.Milliseconds * world.SpeedCoeff);
                    if (reviveTimer > timeToRevive)
                    {
                        reviveTimer = 0;
                        if (p1.Health <= 0 && p1.Revives <= 0) p1.Gain1Revive();
                        else p2.Gain1Revive();
                    }
                }

                // SHARE LIVES?
                if (OptionsMenuScreen.shareRevives)
                {
                    if (p1.Revives + p2.Revives >= 1)
                    {
                        if (p1.Revives <= 0 && p2.Revives > 0 && p1.Health <= 0 && p2.Health > 0) { p1.Gain1Revive(); p2.Lost1Revive(); }
                        else if (p2.Revives <= 0 && p1.Revives > 0 && p2.Health <= 0 && p1.Health > 0) { p2.Gain1Revive(); p1.Lost1Revive(); }
                    }
                }

                // TEMPORARY SPAWNERS
                if (p1.Health > 0 || p2.Health > 0)
                    timeSinceLastZombie += (int)(gameTime.ElapsedGameTime.Milliseconds * world.SpeedCoeff);
                if (timeSinceLastZombie > timeToSpawnZombie || world.enemyList.Count < 16)
                {
                    timeSinceLastZombie -= timeToSpawnZombie;
                    if (world.enemyList.Count < 200)
                    {
                        int numZombieSpawns = Math.Max(3, (p1.KillCount + p2.KillCount) / 100 % 30);
                        for (int i = 0; i < numZombieSpawns; i++)
                        {
                            Texture2D textureToUse = random.Next(2) == 0 ? zombieSheetTexture : zombieSheetTexture2;
                            Zombie zom = new Zombie(p1.Position + Vector2Extensions.RotateVector(p1.Direction, (float)random.NextDouble() * MathHelper.TwoPi) * 5000, random.Next(90, 180), textureToUse, world);
                            zom.ChangeMaxHealth(100 + (p1.KillCount + p2.KillCount) / 10);
                            world.enemyList.Add(zom);
                        }
                        if (random.Next(11) == 0)
                        {
                            int evilCowboysInWorld = 0;
                            foreach (Enemy e in world.enemyList)
                            {
                                EvilCowboy ec = e as EvilCowboy;
                                if (ec != null) evilCowboysInWorld++;
                            }
                            if (evilCowboysInWorld < 3)
                            {
                                EvilCowboy evilCowboy = new EvilCowboy(p1.Position + Vector2Extensions.RotateVector(p1.Direction, (float)random.NextDouble() * MathHelper.TwoPi) * 5000, evilCowboySheetTexture, world);
                                evilCowboy.ChangeMaxHealth(evilCowboy.MaxHealth + (p1.KillCount + p2.KillCount) / 3);
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

            HandleInputForPlayer(0, input, gameTime);
            HandleInputForPlayer(1, input, gameTime);
        }

        public void HandleInputForPlayer(int playerNo, InputState input, GameTime gameTime)
        {
            // set everything according to playerIndex
            Player playerActive = (playerNo == 0) ? p1 : p2;
            ChasingCamera2D playerCam = (playerNo == 0) ? p1Cam : p2Cam;

            // real controller index
            PlayerIndex playerIndex = (playerNo == 0) ? p1Index : p2Index;

            //Vector2 pCamFocus = (playerIndex == 0) ? p1CamFocus : p2CamFocus;
            //float pMaxCamLook = (playerIndex == 0) ? p1MaxCamLook : p2MaxCamLook;

            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];
            GamePadState lastGamePadState = input.LastGamePadStates[(int)playerIndex];
            GamePadState gamePadStateC = inputC.CurrentGamePadStates[(int)playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerNo];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, playerIndex, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), playerIndex);
            }
            else
            {
                // when cowboy's not in a vehicle
                if (playerActive.CarDriving == null && playerActive.Health > 0)
                {
                    Vector2 pLStick = gamePadState.ThumbSticks.Left;
                    Vector2 pMovement = new Vector2(pLStick.X, -pLStick.Y);
                    if (pMovement != Vector2.Zero) pMovement.Normalize();
                    playerActive.Move(pMovement * ((playerNo == 0) ? p1MaxSpeed : p2MaxSpeed));

                    // GAMEPAD WEAPON CONTROLS
                    if (playerActive.GunsOwned.Count > 0)
                    {
                        if (gamePadState.IsButtonDown(Buttons.Y) && lastGamePadState.IsButtonUp(Buttons.Y))
                            playerActive.SwapGun();
                        if (gamePadState.IsButtonDown(Buttons.LeftShoulder))
                            playerActive.ReloadGun();
                        if (playerActive.GunsOwned[playerActive.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Auto)
                        {
                            if (gamePadState.IsButtonDown(Buttons.RightTrigger))
                                playerActive.HitGunTrigger();
                        }
                        else if (playerActive.GunsOwned[playerActive.GunEquipped].FireMode == FAZEngine.Weapons.FireMode.Single)
                        {
                            if (gamePadState.IsButtonDown(Buttons.RightTrigger) && lastGamePadState.IsButtonUp(Buttons.RightTrigger))
                                playerActive.HitGunTrigger();
                        }
                    }
                }
                // when cowboy's in a vehicle
                else if (playerActive.Health > 0)
                {
                    if (playerActive.CarDriving.IsDriver(playerActive))
                    {
                        if (gamePadState.Triggers.Left != 0) playerActive.HitCarBrake(gamePadState.Triggers.Left);
                        else playerActive.HitCarGas(gamePadState.Triggers.Right);
                        playerActive.TurnCarWheels(gamePadState.ThumbSticks.Left.X);
                        if (gamePadState.IsButtonDown(Buttons.DPadLeft) && lastGamePadState.IsButtonUp(Buttons.DPadLeft))
                            playerActive.ShiftCarGear((FAZEngine.Vehicles.Gearbox)(((int)playerActive.CarDriving.Gear + 1) % 2));
                    }
                }

                // Right thumbstick to aim / look around
                // Use gpsC instead (for better accuracy)
                Vector2 pRStick = gamePadStateC.ThumbSticks.Right;
                float pRStickLength = MathHelper.Clamp(pRStick.Length(), 0, 1);
                Vector2 pCamLook = new Vector2(pRStick.X, -pRStick.Y);
                if (pCamLook != Vector2.Zero) pCamLook.Normalize();
                playerActive.Aim(pCamLook);
                if (playerNo == 0) p1CamFocus = playerActive.Position + pCamLook * p1MaxCamLook * pRStickLength;
                else p2CamFocus = playerActive.Position + pCamLook * p2MaxCamLook * pRStickLength;


                // Respawn player automatically
                if (!world.playerList.Contains(playerActive) && playerActive.Revives > 0 && world.SpeedCoeff == 1)
                {
                    playerActive.Lost1Revive();
                    playerActive.Position = new Vector2(random.Next((int)world.WorldSize.X), random.Next((int)world.WorldSize.Y));
                    playerActive.GainHealth(1000);
                    playerActive.GetMaxAmmo(true);
                    playerActive.SetInvincible(3000);
                    world.playerList.Add(playerActive);
                }
                if (gamePadState.IsButtonDown(Buttons.DPadLeft) && lastGamePadState.IsButtonUp(Buttons.DPadLeft))
                {
                    if (playerActive.Health > 0)
                    {
                        ScreenManager.AddScreen(new WeaponShopScreen(playerNo, playerActive, world), playerIndex);
                    }
                }
                if (gamePadState.IsButtonDown(Buttons.DPadRight) && lastGamePadState.IsButtonUp(Buttons.DPadRight))
                {
                    if (playerActive.Health > 0)
                        ScreenManager.AddScreen(new WeaponModScreen(playerNo, playerActive, playerActive.GunsOwned[playerActive.GunEquipped], world), playerIndex);
                }

                // Get in/out car controls
                if (playerActive.Health > 0 && ((gamePadState.IsButtonDown(Buttons.Y) && lastGamePadState.IsButtonUp(Buttons.Y))))
                {
                    if (playerActive.CarDriving == null) playerActive.GetInCar();
                    else playerActive.GetOutCar();
                }

                // transfer money controls
                if (OptionsMenuScreen.shareMoney)
                {
                    if (gamePadState.IsButtonDown(Buttons.RightShoulder))
                    {
                        transferHoldTimer += gameTime.ElapsedGameTime.Milliseconds;
                        if (transferHoldTimer >= transferHoldTime)
                        {
                            transferHoldTimer -= transferHoldTime;
                            if (playerNo == 0 && p1.DeduceMoney(10)) p2.AddMoney(10);
                            else if (playerNo == 1 && p2.DeduceMoney(10)) p1.AddMoney(10);
                        }
                    }
                    else if (lastGamePadState.IsButtonDown(Buttons.RightShoulder))
                    {
                        transferHoldTimer = 0;
                    }
                }

                // Cam zoom level controls
                //if (gamePadState.DPad.Down == ButtonState.Pressed) playerCam.Zoom = 0.5f;

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

            Viewport origViewport = ScreenManager.GraphicsDevice.Viewport;
            // SET VIEWPORT FOR PLAYER 1
            ScreenManager.GraphicsDevice.Viewport = p1Viewport;
            DrawForViewport(0, spriteBatch);
            // SET VIEWOIRT FOR PLAYER 2
            ScreenManager.GraphicsDevice.Viewport = p2Viewport;
            DrawForViewport(1, spriteBatch);
            // SET VIEWPORT BACK
            ScreenManager.GraphicsDevice.Viewport = origViewport;

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        public void DrawForViewport(int playerIndex, SpriteBatch spriteBatch)
        {
            ChasingCamera2D playerCam = (playerIndex == 0) ? p1Cam : p2Cam;
            Player playerActive = (playerIndex == 0) ? p1 : p2;
            Viewport viewport = (playerIndex == 0) ? p1Viewport : p2Viewport;

            // Draw under CAMERA
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, playerCam.get_transformation(viewport));

            // draw world
            world.Draw(spriteBatch);

            // draw cowboy's crosshair AND LAZER
            if (playerActive.CarDriving == null && playerActive.Health > 0)
            {
                Vector2 crosshairSize = new Vector2(crosshairTexture.Width, crosshairTexture.Height);
                spriteBatch.Draw(crosshairTexture, playerActive.Position + playerActive.Direction * 200,
                    null,
                    playerActive.GunsOwned[playerActive.GunEquipped].ClipAmmo > 0 && !playerActive.GunsOwned[playerActive.GunEquipped].Reloading ?
                    Color.YellowGreen : Color.Red,
                    playerActive.GunsOwned[playerActive.GunEquipped].Reloading ?
                    (float)Math.PI * playerActive.GunsOwned[playerActive.GunEquipped].ReloadPosition : 0,
                    crosshairSize / 2,
                    playerActive.GunsOwned[playerActive.GunEquipped].RecoilClimb * 15 + 1,
                    SpriteEffects.None, 1f);

                if (playerActive.GunsOwned[playerActive.GunEquipped].HasLazer)
                    spriteBatch.Draw(lazerTexture, playerActive.Position + playerActive.Direction * 48, null,
                        Color.White, Vector2Extensions.VectorToRadians(playerActive.Direction),
                        new Vector2(lazerTexture.Width / 2, lazerTexture.Height), 1,
                        SpriteEffects.None, 1f);
            }

            spriteBatch.End();

            // DRAW PARTICLE SYSTEM
            spriteBatch.Begin();
            foreach (ParticleEffect p in world.particleEffects)
            {
                Matrix camMatrix = playerCam.get_transformation(viewport);
                particleRenderer.RenderEffect(p, ref camMatrix);
            }
            spriteBatch.End();

            // draw status display
            spriteBatch.Begin();
            if (playerIndex == 0) p1StatDisplay.Draw(spriteBatch, p1Viewport);
            else p2StatDisplay.Draw(spriteBatch, p2Viewport);

            // draw a string "press B to revive" in the middle
            if (playerActive.Health <= 0)
            {
                string reviveStr;
                if (playerActive.Revives > 0)
                {
                    int numRevives = OptionsMenuScreen.shareRevives ? p1.Revives + p2.Revives : playerActive.Revives;
                    reviveStr = numRevives + " li" + (numRevives <= 1 ? "fe" : "ves") + " left";
                }
                else
                    reviveStr = "NO LIFE LEFT!\nWait " + (timeToRevive - reviveTimer) / 1000 + " second" + ((timeToRevive - reviveTimer) / 1000 > 1 ? "s" : "");
                Vector2 measure = gameFont.MeasureString(reviveStr);
                Vector2 posToDrawStr = new Vector2(viewport.Width, viewport.Height) / 2 - measure / 2;
                spriteBatch.Draw(blank, new Rectangle((int)posToDrawStr.X, (int)posToDrawStr.Y, (int)measure.X, (int)measure.Y), Color.FromNonPremultiplied(0, 0, 0, 140));
                spriteBatch.DrawString(gameFont, reviveStr, posToDrawStr, Color.White);
            }

            spriteBatch.End();
        }

        #endregion
    }
}
