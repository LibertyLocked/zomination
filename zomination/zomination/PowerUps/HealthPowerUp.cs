using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FAZEngine;
using FAZEngine.Environment;

namespace zomination.PowerUps
{
    class HealthPowerUp : PowerUp
    {
        public HealthPowerUp(Vector2 position, World world)
            : base(position, 50, world.powerUpTextures[0], world)
        {
        }

        public override void TriggerPowerUp(FAZEngine.Peds.Player player)
        {
            // display particle effect
            world.particleEffects[2].Trigger(this.position);
            // add health to player
            player.GainHealth(1000);
            base.TriggerPowerUp(player);
        }
    }
}
