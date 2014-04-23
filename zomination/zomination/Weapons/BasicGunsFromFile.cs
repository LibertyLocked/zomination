using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine;
using FAZEngine.Weapons;

namespace zomination.Weapons
{
    public abstract class BasicGunsFromFile : BasicGun
    {
        public BasicGunsFromFile(IGunner owner, World world, string weaponName)
            : base(owner, 0, 0, 0, 0, FireMode.Single, 0, 0, 0, 0, 0, world, weaponName, GunType.Pistol)
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
            this.gunType = (FAZEngine.Weapons.GunType)int.Parse(data.IniReadValue(weaponName, "gunType"));
#else
            if (weaponName == "M1911")
            {
                this.damage = 40;
                this.range = 850;
                this.fireRate = 500;
                this.bulletSpeed = 50;
                this.fireMode = FAZEngine.Weapons.FireMode.Single;
                this.recoilPerShot = 0.038f;
                this.maxRecoil = 0.25f;
                this.clipSize = 8;
                this.reserveSize = 120;
                this.reloadTime = 1600;
                this.gunType = FAZEngine.Weapons.GunType.Pistol;
            }
            else if (weaponName == "AK74u")
            {
                this.damage = 45;
                this.range = 1100;
                this.fireRate = 750;
                this.bulletSpeed = 50;
                this.fireMode = FAZEngine.Weapons.FireMode.Auto;
                this.recoilPerShot = 0.040f;
                this.maxRecoil = 0.12f;
                this.clipSize = 30;
                this.reserveSize = 240;
                this.reloadTime = 2500;
                this.gunType = FAZEngine.Weapons.GunType.SMG;
            }
            else if (weaponName == "M4A1")
            {
                this.damage = 55;
                this.range = 1300;
                this.fireRate = 700;
                this.bulletSpeed = 50;
                this.fireMode = FAZEngine.Weapons.FireMode.Auto;
                this.recoilPerShot = 0.035f;
                this.maxRecoil = 0.16f;
                this.clipSize = 30;
                this.reserveSize = 240;
                this.reloadTime = 3000;
                this.gunType = FAZEngine.Weapons.GunType.AssultRifle;
            }
            else if (weaponName == "M60")
            {
                this.damage = 63;
                this.range = 1300;
                this.fireRate = 535;
                this.bulletSpeed = 50;
                this.fireMode = FAZEngine.Weapons.FireMode.Auto;
                this.recoilPerShot = 0.055f;
                this.maxRecoil = 0.16f;
                this.clipSize = 100;
                this.reserveSize = 300;
                this.reloadTime = 9100;
                this.gunType = FAZEngine.Weapons.GunType.LMG;
            }
#endif

            clipAmmo = clipSize;
            reserveAmmo = reserveSize;
        }
    }
}
