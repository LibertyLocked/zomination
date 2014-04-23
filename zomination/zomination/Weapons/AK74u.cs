using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public class AK74u : BasicGunsFromFile
    {
        public AK74u(IGunner owner, World world)
            : base(owner, world, "AK74u")
        { }
    }
}
