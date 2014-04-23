using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Weapons;

namespace FAZEngine.Weapons
{
    public abstract class Shotgun : BasicGun
    {
        protected int subShells;
        protected float shellSpread;
        public int singleReloadTime;
        protected bool clipReload;

        public Shotgun(IGunner owner, int damage, float range, int fireRate, float bulletSpeed,
            FireMode fireMode, float recoilPerShot, float maxRecoil, int clipSize, int reserveSize, int reloadTime, 
            int subShells, float shellSpread, bool clipReload, World world, string weaponName)
            : base(owner, damage, range, fireRate, bulletSpeed, fireMode, recoilPerShot, maxRecoil, clipSize, reserveSize, reloadTime, world, weaponName, GunType.Shotgun)
        {
            this.subShells = subShells;
            this.shellSpread = shellSpread;
            this.clipReload = clipReload;

            singleReloadTime = reloadTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (reloadCue != null && reloadCue.IsPlaying && !reloading && !clipReload) 
                reloadCue.Stop(AudioStopOptions.AsAuthored);
            base.Update(gameTime);
        }

        public override bool Trigger(Microsoft.Xna.Framework.Vector2 direction, float directionOffset)
        {
            if (timeSinceLastShot > MillisecondsToFire && !reloading && clipAmmo > 0)
            {
                clipAmmo--;

                for (int i = 0; i < subShells; i++)
                {
                    Texture2D pathTexture = world.bulletPathTextures[random.Next(world.bulletPathTextures.Count())];
                    float offset = (float)random.NextDouble() * currentRecoil * (random.Next(2) == 1 ? -1 : 1);
                    float spread = (float)random.NextDouble() * shellSpread * (random.Next(2) == 1 ? -1 : 1);
                    float bulletRotation = Vector2Extensions.VectorToRadians(direction) + offset + spread;
                    Vector2 bulletDirection = Vector2Extensions.RadiansToVector(bulletRotation);
                    world.bulletList.Add(new Bullet(owner.Position + owner.Direction * directionOffset,
                        bulletDirection * bulletSpeed, damage, range, pathTexture, owner.Targets, (GeneralPed)owner, world));
                }
                currentRecoil += recoilPerShot;
                timeSinceLastShot = 0;
                // play sound
                world.weaponSounds.PlayCue(WeaponName + "_fire");
                return true;
            }
            else
                return false;
        }

        public override bool Reload()
        {
            if (reserveAmmo > 0 && clipAmmo < clipSize && !reloading && !clipReload)
            {
                int totalAmmo = clipAmmo + reserveAmmo;
                if (totalAmmo < clipSize)
                    reloadTime = singleReloadTime * (totalAmmo - clipAmmo) + singleReloadTime * 2;
                else
                    reloadTime = singleReloadTime * (clipSize - clipAmmo) + singleReloadTime * 2;
            }
            return base.Reload();
        }
    }
}
