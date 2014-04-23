using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FAZEngine;
using FAZEngine.Environment;

namespace zomination.PowerUps
{
    class AmmoPowerUp : PowerUp
    {
        public AmmoPowerUp(Vector2 position, World world)
            : base(position, 50, world.powerUpTextures[1], world)
        {
        }

        public override void TriggerPowerUp(FAZEngine.Peds.Player player)
        {
            // trigger particle effect
            world.particleEffects[2].Trigger(this.position);
            // add ammo to player
            player.GetMaxAmmo(false);
            base.TriggerPowerUp(player);
        }
    }
}
