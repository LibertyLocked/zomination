using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    class R870MCS : ShotgunsFromFile
    {
        public R870MCS(IGunner owner, World world)
            : base(owner, world, "R870MCS")
        {
            // because it's pump action
            this.recoilPerShot = 3600 * owner.GunRecoilControl / fireRate;
        }
    }
}
