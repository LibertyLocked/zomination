using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;

namespace FAZEngine.Peds
{
    public abstract class Enemy : GeneralPed
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Enemy(Vector2 position, Vector2 speed, Vector2 direction, Texture2D sheetTexture, Point sheetSize, Point frameSize, int millisecondsPerFrame, int maxHealth, World world, float collisionRadius)
            : base(position, speed, direction, sheetTexture, sheetSize, frameSize, millisecondsPerFrame, maxHealth, world, collisionRadius)
        { }

        /// <summary>
        /// Gets the closest player in the world.
        /// </summary>
        /// <returns>Returns the player if found one, or null if no player exists.</returns>
        protected Player GetClosestPlayer()
        {
            if (world.playerList.Count == 0) return null;
            else
            {
                int index = 0;
                for (int i = 0; i < world.playerList.Count; i++)
                {
                    if ((world.playerList[i].Position - this.position).LengthSquared() <
                        (world.playerList[index].Position - this.position).LengthSquared())
                    {
                        index = i;
                    }
                }
                return world.playerList[index];
            }
        }

        public void ChangeMaxHealth(int maxHealth)
        {
            this.maxHealth = maxHealth;
        }
    }
}
