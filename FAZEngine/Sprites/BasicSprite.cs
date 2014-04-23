using System;
using Microsoft.Xna.Framework;

namespace FAZEngine.Sprites
{
    /// <summary>
    /// A basic sprite in the world.
    /// </summary>
    public abstract class BasicSprite
    {
        // The position of the sprite in the world coord.
        protected Vector2 position;

        /// <summary>
        /// Gets or sets the position of this sprite.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            protected set { position = value; }
        }

        /// <summary>
        /// Constructs a basic sprite.
        /// </summary>
        /// <param name="position">Position of the sprite.</param>
        public BasicSprite(Vector2 position)
        {
            this.position = position;
        }
    }
}
