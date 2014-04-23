using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Environment
{
    public class TestingBlock : MapElem
    {
        int r, g, b;
        int ir, ig, ib;
        Random rnd = new Random();
        int timeSinceLastColorChange;
        int timeToColorChange = 50;
        public bool colorChange;

        public TestingBlock(Vector2 position, Vector2 size, Texture2D texture, bool colorChange)
            : base(position, size, texture)
        {
            this.colorChange = colorChange;

            r = rnd.Next(256);
            g = rnd.Next(256);
            b = rnd.Next(256);

            ir = 1 + rnd.Next(6);
            ig = 1 + rnd.Next(6);
            ib = 1 + rnd.Next(6);
        }

        public override void Update(GameTime gameTime)
        {
            if (colorChange)
            {
                if ((r > 255 && ir > 0) || (r < 0 && ir < 0)) ir *= -1;
                if ((g > 255 && ig > 0) || (g < 0 && ig < 0)) ig *= -1;
                if ((b > 255 && ib > 0) || (b < 0 && ib < 0)) ib *= -1;

                timeSinceLastColorChange += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastColorChange >= timeToColorChange)
                {
                    timeSinceLastColorChange = 0;
                    r += ir;
                    g += ig;
                    b += ib;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float layer)
        {
            spriteBatch.Draw(Texture,
                new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), colorChange ? Color.FromNonPremultiplied(r, g, b, 255) : Color.Green);
        }
    }
}
