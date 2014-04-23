using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Sprites;

namespace FAZEngine.Peds
{
    public abstract class GeneralPed : AnimatedSprite
    {
        protected int health;
        protected int maxHealth;
        protected World world;
        private float collisionRadius;

        private int invincibleTime = 0;
        private int invincibleTimer = 0;

        public float CollisionRadius
        {
            get { return collisionRadius; }
        }

        public int Health
        {
            get { return health; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public virtual bool RemoveMePlz
        {
            get { return (health <= 0); }
        }

        public bool IsInvincible
        {
            get { return invincibleTime > 0; }
        }

        public GeneralPed(Vector2 position, Vector2 speed, Vector2 direction, Texture2D sheetTexture, Point sheetSize, Point frameSize, int millisecondsPerFrame, int maxHealth, World world, float collisionRadius)
            : base(position, speed, direction, sheetTexture, sheetSize, frameSize, millisecondsPerFrame)
        {
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.world = world;
            this.collisionRadius = collisionRadius;
        }

        /// <summary>
        /// Updates the general sprite
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Counts invincible timer
            invincibleTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (invincibleTimer >= invincibleTime)
            {
                invincibleTime = 0;
                invincibleTimer = 0;
            }

            if (speed == Vector2.Zero)
            {
                if (IsPlaying) StopAnimation();
            }
            else
            {
                if (!IsPlaying) StartAnimation();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Move the sprite with a given speed.
        /// </summary>
        /// <param name="speed"></param>
        public void Move(Vector2 speed)
        {
            this.speed = speed;
        }

        /// <summary>
        /// Direction independent to speed when aiming.
        /// </summary>
        /// <param name="direction"></param>
        public void Aim(Vector2 direction)
        {
            getDirectionFromSpeed = (direction == Vector2.Zero);
            if (!getDirectionFromSpeed)
            {
                this.direction = direction;
                this.direction.Normalize();
            }
        }

        public virtual void TakeDamage(int damage, GeneralPed attacker)
        {
            if (invincibleTime != 0) return;
            if (health > 0 && health - damage <= 0) UponDeath(damage, attacker);
            this.health = (int)MathHelper.Clamp(health - damage, 0f, maxHealth);
        }

        public virtual void GainHealth(int gained)
        {
            this.health = (int)MathHelper.Clamp(health + gained, 0f, maxHealth);
        }

        /// <summary>
        /// Set the ped to be invincible for some time
        /// </summary>
        /// <param name="milliseconds"></param>
        public void SetInvincible(int milliseconds)
        {
            invincibleTimer = 0;
            invincibleTime = milliseconds;
        }

        /// <summary>
        /// Triggered upon player death
        /// </summary>
        protected virtual void UponDeath(int damage, GeneralPed attacker) { }
    }
}
