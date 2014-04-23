using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public abstract class ShotgunsFromFile : Shotgun
    {
        public ShotgunsFromFile(IGunner owner, World world, string weaponName)
            : base(owner, 0, 0, 0, 0, FireMode.Single, 0, 0, 0, 0, 0, 0, 0, true, world, weaponName)
        {
#if WINDOWS
            IniFile data = new IniFile(@"main\specs\gunspecs.txt");
            this.damage = int.Parse(data.IniReadValue(weaponName, "damage"));
            this.range = float.Parse(data.IniReadValue(weaponName, "range"));
            this.fireRate = int.Parse(data.IniReadValue(weaponName, "fireRate"));
            this.bulletSpeed = float.Parse(data.IniReadValue(weaponName, "bulletSpeed"));
            this.fireMode = (FAZEngine.Weapons.FireMode)int.Parse(data.IniReadValue(weaponName, "fireMode"));
            this.recoilPerShot = float.Parse(data.IniReadValue(weaponName, "recoilPerShot"));
            this.maxRecoil = float.Parse(data.IniReadValue(weaponName, "maxRecoil"));
            this.clipSize = int.Parse(data.IniReadValue(weaponName, "clipSize"));
            this.reserveSize = int.Parse(data.IniReadValue(weaponName, "reserveSize"));
            this.reloadTime = int.Parse(data.IniReadValue(weaponName, "reloadTime"));
            this.subShells = int.Parse(data.IniReadValue(weaponName, "subShells"));
            this.shellSpread = float.Parse(data.IniReadValue(weaponName, "shellSpread"));
            this.clipReload = bool.Parse(data.IniReadValue(weaponName, "clipReload"));
#else
            if (weaponName == "R870MCS")
            {
                this.damage = 83;
                this.range = 800;
                this.fireRate = 80;
                this.bulletSpeed = 50;
                this.fireMode = FAZEngine.Weapons.FireMode.Single;
                this.recoilPerShot = 0;
                this.maxRecoil = 10;
                this.clipSize = 8;
                this.reserveSize = 64;
                this.reloadTime = 400;
                this.subShells = 8;
                this.shellSpread = 0.26f;
                this.clipReload = false;
            }
#endif
            clipAmmo = clipSize;
            reserveAmmo = reserveSize;
            singleReloadTime = reloadTime;                
        }
    }
}
