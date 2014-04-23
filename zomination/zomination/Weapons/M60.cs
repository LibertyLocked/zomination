using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public class M60 : BasicGunsFromFile
    {
        public M60(IGunner owner, World world)
            : base(owner, world, "M60")
        { }
    }
}
