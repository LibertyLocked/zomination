using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Environment
{
    public abstract class MapElem
    {
        Texture2D texture;
        Vector2 position;
        Vector2 size;

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MapElem(Vector2 position, Vector2 size, Texture2D texture)
        {
            this.position = position;
            this.size = size;
            this.texture = texture;
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch, float layer) { }
    }
}
