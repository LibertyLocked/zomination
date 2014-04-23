using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public class M4A1 : BasicGunsFromFile
    {
        public M4A1(IGunner owner, World world)
            : base(owner, world, "M4A1")
        { }
    }
}
