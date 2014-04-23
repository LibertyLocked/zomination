using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FAZEngine;
using FAZEngine.Environment;

namespace zomination.PowerUps
{
    class MoneyPowerUp : PowerUp
    {
        int fixedAmount = 0;

        public MoneyPowerUp(Vector2 position, World world)
            : base(position, 50, world.powerUpTextures[2], world)
        {
        }

        public MoneyPowerUp(Vector2 position, World world, int fixedAmount)
            : this(position, world)
        {
            this.fixedAmount = fixedAmount;
        }

        public override void TriggerPowerUp(FAZEngine.Peds.Player player)
        {
            // display particle effect
            world.particleEffects[2].Trigger(this.position);
            // add money to player
            if (fixedAmount == 0)
                player.AddMoney(20 + (int)(new Random().NextDouble() * 100));   // from 20 to 120
            else
                player.AddMoney(fixedAmount);
            base.TriggerPowerUp(player);
        }
    }
}
