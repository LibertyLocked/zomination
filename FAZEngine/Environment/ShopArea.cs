using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Peds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Environment
{
    public class ShopArea
    {
        Vector2 position;
        Vector2 size;
        string name;
        Texture2D shopTexture;

        public string Name
        { get { return name; } }

        public ShopArea(Vector2 position, Vector2 size, string name, Texture2D shopTexture, World world)
        {
            this.name = name;
            this.position = position;
            this.size = size;
            this.shopTexture = shopTexture;
        }

        // this thing doesnt need to be updated.
        // but a method is required to decide whether player is in shop
        public bool IsInShop(Player playerToCheck)
        {
            return (playerToCheck.Position.X > position.X && playerToCheck.Position.X < position.X + size.X &&
                playerToCheck.Position.Y > position.Y && playerToCheck.Position.Y < position.Y + size.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(shopTexture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);
        }
    }
}
