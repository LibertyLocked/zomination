using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Vehicles;

namespace zomination.Vehicles
{
    class Bobcat : BasicCar
    {
        public Bobcat(Vector2 pos, float rotation, Texture2D carTexture, Texture2D wheelsTexture, World world)
            : base(pos, new Vector2(95, 204.5f), rotation, 80, 100, 0.2f, 0.025f, carTexture, wheelsTexture, new Vector2(16,76), 6000, world)
        { }
    }
}
