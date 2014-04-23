using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FAZEngine.Sprites
{
    /// <summary>
    /// A moving sprite in the world.
    /// </summary>
    public abstract class MovingSprite : BasicSprite
    {
        // the speed of the moving sprite.
        protected Vector2 speed;

        /// <summary>
        /// Gets or sets the speed of the moving sprite.
        /// </summary>
        public Vector2 Speed
        {
            get { return speed; }
            protected set { speed = value; }
        }

        /// <summary>
        /// Constructs a moving sprite.
        /// </summary>
        /// <param name="position">Position of the sprite.</param>
        /// <param name="speed">Initial speed of the sprite.</param>
        public MovingSprite(Vector2 position, Vector2 speed)
            : base(position)
        {
            this.speed = speed;
        }

        /// <summary>
        /// Updates the moving sprite.
        /// </summary>
        /// <param name="gameTime">Game timer</param>
        public virtual void Update(GameTime gameTime)
        {
            // s = s0 + v * dt
            Position += Speed * (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
        }
    }
}
