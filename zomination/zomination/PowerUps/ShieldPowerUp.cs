using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FAZEngine;
using FAZEngine.Environment;

namespace zomination.PowerUps
{
    class ShieldPowerUp : PowerUp
    {
        public ShieldPowerUp(Vector2 position, World world)
            : base(position, 50, world.powerUpTextures[4], world)
        {
        }

        public override void TriggerPowerUp(FAZEngine.Peds.Player player)
        {
            // display particle effect
            world.particleEffects[2].Trigger(this.position);
            // give player shield
            player.SetInvincible(10000);
            base.TriggerPowerUp(player);
        }
    }
}
