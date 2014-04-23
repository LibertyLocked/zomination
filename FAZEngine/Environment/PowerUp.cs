using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;

namespace FAZEngine.Environment
{
    public abstract class PowerUp
    {
        protected Vector2 position;
        protected float collisionRadius;
        protected World world;
        Texture2D powerUpTexture;
        bool triggered = false;

        int liveTimer = 0;
        protected int timeToDisappear = 30000;

        bool flash = false;
        int flashTimer = 0;
        protected int timeToFlash = 500;

        public bool RemoveMePlz
        {
            get { return triggered; }
        }

        public PowerUp(Vector2 position, float collisionRadius, Texture2D powerUpTexture, World world)
        {
            this.position = position;
            this.collisionRadius = collisionRadius;
            this.powerUpTexture = powerUpTexture;
            this.world = world;
        }

        public virtual void Update(GameTime gameTime)
        {
            // clamp the powerup inside the world
            this.position = Vector2.Clamp(position, Vector2.Zero, world.WorldSize);

            // check life timer
            liveTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (liveTimer > timeToDisappear)
            {
                this.triggered = true;
            }

            if (timeToDisappear - liveTimer <= timeToDisappear / 4)
            {
                flashTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (flashTimer > timeToFlash)
                {
                    flashTimer -= timeToFlash;
                    flash = !flash;
                }
            }

            // test collison with any player
            if (!triggered)
            {
                foreach (Player p in world.playerList)
                {
                    if ((p.Position - this.position).Length() < collisionRadius)
                    {           
                        // dead players don't trigger
                        if (p.Health > 0) TriggerPowerUp(p);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2 measure = new Vector2(powerUpTexture.Width, powerUpTexture.Height);
            if (!flash)
                spriteBatch.Draw(powerUpTexture, this.position - measure / 2, Color.White);
        }

        public virtual void TriggerPowerUp(Player player)
        {
            triggered = true;
        }
    }
}
