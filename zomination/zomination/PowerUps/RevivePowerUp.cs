using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FAZEngine;
using FAZEngine.Environment;

namespace zomination.PowerUps
{
    class RevivePowerUp : PowerUp
    {
        public RevivePowerUp(Vector2 position, World world)
            : base(position, 50, world.powerUpTextures[3], world)
        {
        }

        public override void TriggerPowerUp(FAZEngine.Peds.Player player)
        {
            // display particle effect
            world.particleEffects[2].Trigger(this.position);
            // add 1 revive to player
            player.Gain1Revive();
            base.TriggerPowerUp(player);
        }
    }
}
