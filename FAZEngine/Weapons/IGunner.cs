using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine.Peds;
using Microsoft.Xna.Framework;

namespace FAZEngine.Weapons
{
    public interface IGunner
    {
        Vector2 Position
        { get; }

        Vector2 Speed
        { get; }

        Vector2 Direction
        { get; }

        List<GeneralPed> Targets { get; }
        List<BasicGun> GunsOwned { get; }
        int GunEquipped { get; }
        float GunRecoilControl { get; }

        bool HitGunTrigger();
        bool ReloadGun();
        void SwapGun();
    }
}
