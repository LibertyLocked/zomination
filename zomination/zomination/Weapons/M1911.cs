using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public class M1911 : BasicGunsFromFile
    {
        public M1911(IGunner owner, World world)
            : base(owner, world, "M1911")
        { }
    }
}
