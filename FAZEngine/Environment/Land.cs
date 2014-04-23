using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Environment
{
    public class Land : MapElem
    {
        public Land(Vector2 position, Vector2 size, Texture2D texture)
            : base(position, size, texture)
        { }
    }
}
