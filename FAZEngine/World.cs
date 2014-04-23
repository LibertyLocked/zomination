using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using FAZEngine.Sprites;
using FAZEngine.Peds;
using FAZEngine.Environment;
using FAZEngine.Vehicles;
using FAZEngine.Weapons;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace FAZEngine
{
    public class World
    {
        // Textures
        Texture2D mapBackground;

        // Lists of sprites
        public List<TestingBlock> testingBlocksList;
        public List<ShopArea> shopAreas;
        public List<BasicCar> basicCarList;
        public List<Player> playerList;
        public List<Enemy> enemyList;
        public List<Bullet> bulletList;
        public List<PowerUp> powerUpList;

        // Array of shared bullet path textures
        public Texture2D[] bulletPathTextures
        {
            get;
            private set;
        }

        public Texture2D[] powerUpTextures
        {
            get;
            private set;
        }

        public ParticleEffect[] particleEffects
        {
            get;
            private set;
        }

        public Texture2D[] shopTextures
        {
            get;
            private set;
        }

        // Shared weapon sound bank
        public SoundBank weaponSounds
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of the world.
        /// </summary>
        public Vector2 WorldSize
        {
            get;
            protected set;
        }

        public float SpeedCoeff
        {
            get { return speedCoeff; }
        }

        private float speedCoeff = 1f;
        private int slowMoTimer = 0;
        private int slowMoTime;

        /// <summary>
        /// Construct a world.
        /// </summary>
        /// <param name="game">Game associated with this world</param>
        /// <param name="worldSize">Size of the world.</param>
        public World(Game game, Vector2 worldSize, Texture2D mapBackground, 
            Texture2D[] bulletPathTextures, Texture2D[] powerUpTextures, Texture2D[] shopTextures,
            SoundBank weaponSounds, ParticleEffect[] particleEffects)
        {
            this.WorldSize = worldSize;
            this.bulletPathTextures = bulletPathTextures;
            this.powerUpTextures = powerUpTextures;
            this.weaponSounds = weaponSounds;
            this.particleEffects = particleEffects;
            this.shopTextures = shopTextures;
            this.mapBackground = mapBackground;

            testingBlocksList = new List<TestingBlock>();
            shopAreas = new List<ShopArea>();
            basicCarList = new List<BasicCar>();
            playerList = new List<Player>();
            enemyList = new List<Enemy>();
            bulletList = new List<Bullet>();
            powerUpList = new List<PowerUp>();
        }

        /// <summary>
        /// Updates the world.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            GameTime modGameTime;
            if (speedCoeff != 1)
            {
                double slowElapsedTime = gameTime.ElapsedGameTime.Milliseconds * speedCoeff;
                modGameTime = new GameTime(gameTime.TotalGameTime, new System.TimeSpan(0, 0, 0, 0, (int)slowElapsedTime));

                // count timer to end slow mo
                slowMoTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (slowMoTimer >= slowMoTime)
                {
                    speedCoeff = MathHelper.Min(speedCoeff + 0.01f, 1);
                }
            }
            else
                modGameTime = gameTime;


            // Update map elements
            foreach (TestingBlock t in testingBlocksList)
                if (t.colorChange) t.Update(modGameTime);

            // Update all peds and cars and bullets and powerups
            foreach (BasicCar c in basicCarList)
                c.Update(modGameTime);

            foreach (Player p in playerList)
                p.Update(modGameTime);

            foreach (Enemy e in enemyList)
                e.Update(modGameTime);

            foreach (Bullet b in bulletList)
                b.Update(modGameTime);

            foreach (PowerUp p in powerUpList)
                p.Update(modGameTime);

            // AND... remove things from list when they are not needed anymore
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].RemoveMePlz)
                {
                    bulletList.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].RemoveMePlz || IsMuchOutside(enemyList[i]))
                {
                    enemyList.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < basicCarList.Count; i++)
            {
                if (basicCarList[i].RemoveMePlz)
                {
                    basicCarList.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].RemoveMePlz)
                {
                    playerList[i].GunsOwned[playerList[i].GunEquipped].InstaReload();
                    playerList[i].GunsOwned[playerList[i].GunEquipped].Update(gameTime);
                    playerList.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < powerUpList.Count; i++)
            {
                if (powerUpList[i].RemoveMePlz)
                {
                    powerUpList.RemoveAt(i);
                    i--;
                }
            }

            // Update Particle Effects
            foreach (ParticleEffect p in particleEffects)
            {
                p.Update((float)modGameTime.ElapsedGameTime.TotalSeconds);
            }

            // check if a player is in shop area and set their .InShop property
            foreach (Player p in playerList)
            {
                p.InShop = null;
                foreach (ShopArea sh in shopAreas)
                {
                    if (sh.IsInShop(p)) p.InShop = sh;
                }
            }
        }

        /// <summary>
        /// Draws the world. (its elements)
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // DRAW TILE BACKGROUND
            spriteBatch.Draw(mapBackground, new Rectangle(0, 0, (int)WorldSize.X, (int)WorldSize.Y), Color.White);

            // Map Elements
            foreach (TestingBlock t in testingBlocksList)
                t.Draw(spriteBatch, 0f);

            foreach (ShopArea sh in shopAreas)
                sh.Draw(spriteBatch);

            // World entities
            foreach (PowerUp p in powerUpList)
                p.Draw(spriteBatch);

            foreach (Bullet b in bulletList)
                b.Draw(spriteBatch, 1f);

            foreach (BasicCar c in basicCarList)
                c.Draw(spriteBatch);

            foreach (Player p in playerList)
                p.Draw(spriteBatch, 1f);

            foreach (Enemy e in enemyList)
                e.Draw(spriteBatch, 1f);
        }

        public void SlowDown(int time, float coeff)
        {
            slowMoTime = time;
            speedCoeff = coeff;
            slowMoTimer = 0;
        }

        /// <summary>
        /// Loads preset maps.
        /// </summary>
        /// <param name="mapIndex"></param>
        /// <param name="blank"></param>
        public virtual void LoadPresetMap(int mapIndex, Texture2D blank)
        {
            switch (mapIndex)
            {
                case 0:
                    this.WorldSize = new Vector2(4000, 2500);

                    // just map blocks
                    //testingBlocksList.Add(new TestingBlock(new Vector2(100, 100), new Vector2(100, 100), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(200, 200), new Vector2(200, 200), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(500, 500), new Vector2(500, 500), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(1300, 1300), new Vector2(300, 300), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(1500, 550), new Vector2(400, 200), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(1700, 1700), new Vector2(100, 100), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(2300, 0), new Vector2(100, 2000), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(2500, 1700), new Vector2(200, 200), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(2800, 1300), new Vector2(500, 500), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(3600, 500), new Vector2(300, 300), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(3000, 700), new Vector2(300, 300), blank, false));
                    //testingBlocksList.Add(new TestingBlock(new Vector2(0, 1900), new Vector2(2000, 100), blank, false));
                    // draw a nice border
                    testingBlocksList.Add(new TestingBlock(new Vector2(-100, -100), new Vector2(WorldSize.X + 200, 100), blank, true)); // top
                    testingBlocksList.Add(new TestingBlock(new Vector2(-100, 0), new Vector2(100, WorldSize.Y + 100), blank, true)); // left
                    testingBlocksList.Add(new TestingBlock(new Vector2(0, WorldSize.Y), new Vector2(WorldSize.X + 100, 100), blank, true));
                    testingBlocksList.Add(new TestingBlock(new Vector2(WorldSize.X, 0), new Vector2(100, WorldSize.Y + 100), blank, true));
                    // shop areas
                    //shopAreas.Add(new ShopArea(new Vector2(1500, 1900), new Vector2(shopTextures[0].Width, shopTextures[0].Height), "WeaponShop", shopTextures[0], this));
                    //shopAreas.Add(new ShopArea(new Vector2(3200, 2000), new Vector2(shopTextures[1].Width, shopTextures[1].Height), "ModShop", shopTextures[1], this));
                    //shopAreas.Add(new ShopArea(new Vector2(3000, 400), new Vector2(shopTextures[0].Width, shopTextures[0].Height), "WeaponShop", shopTextures[0], this));
                    //shopAreas.Add(new ShopArea(new Vector2(200, 600), new Vector2(shopTextures[1].Width, shopTextures[1].Height), "ModShop", shopTextures[1], this));
                    break;
            }
            
        }

        public bool IsWithinMap(GeneralPed ped)
        {
            return !(ped.Position.X < 0 || ped.Position.X > WorldSize.X || ped.Position.Y < 0 || ped.Position.Y > WorldSize.Y);
        }

        public bool IsMuchOutside(GeneralPed ped)
        {
            // calculate the distance to middle of the map
            Vector2 distance = (ped.Position - WorldSize / 2);
            if (distance.Length() > 20000) return true;
            else return false;
        }
    }
}
