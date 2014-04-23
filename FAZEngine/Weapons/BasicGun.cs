using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FAZEngine.Peds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FAZEngine.Weapons
{
    public abstract class BasicGun : IDisposable
    {
        // times
        protected int timeSinceLastShot;
        protected int timeInReload;

        // Basic fields
        protected Texture2D gunTexture;
        protected IGunner owner;
        protected float range, bulletSpeed;
        protected int damage;
        protected int clipAmmo, clipSize;
        protected int reserveAmmo, reserveSize;
        protected FireMode fireMode;
        protected bool reloading;
        protected int reloadTime, fireRate;
        protected string weaponName;
        protected GunType gunType;
        // world and related stuff
        protected World world;
        // recoil vars
        protected float currentRecoil, recoilPerShot, maxRecoil;
        // reload cue
        protected Cue reloadCue;

        // MODS
        protected bool hasLazer;
        protected bool hasExtendedClip;
        protected bool hasFullAuto;
        protected bool hasFmj;
        protected bool hasFastReload;
        protected bool hasMoreAmmo;

        protected Random random;

        public bool HasLazer { get { return hasLazer; } }
        public bool HasExtendedClip { get { return hasExtendedClip; } }
        public bool HasFullAuto { get { return hasFullAuto || (fireMode == Weapons.FireMode.Auto); } }
        public bool HasFmj { get { return hasFmj; } }
        public bool HasFastReload { get { return hasFastReload; } }
        public bool HasMoreAmmo { get { return hasMoreAmmo; } }

        public string WeaponName
        {
            get { return weaponName; }
        }

        public int FireRate
        {
            get { return fireRate; }
            set { fireRate = value; }
        }

        public float Range
        {
            get { return range; }
        }

        public float RecoilClimb
        {
            get { return currentRecoil; }
        }

        public FireMode FireMode
        {
            get { return fireMode; }
        }

        public int ClipAmmo
        {
            get { return clipAmmo; }
        }

        public int ReserveAmmo
        {
            get { return reserveAmmo; }
        }

        public int ReserveSize
        {
            get { return reserveSize; }
        }

        public bool Reloading
        {
            get { return reloading; }
        }

        public float ReloadPosition
        {
            get
            {
                if (reloading)
                    return MathHelper.Clamp((float)timeInReload / reloadTime, 0, 1);
                else
                    return 1;
            }
        }

        public bool CurrentlyEquipped
        {
            get { return owner.GunsOwned[owner.GunEquipped] == this; }
        }

        public GunType GunType
        {
            get { return gunType; }
        }

        /// <summary>
        /// Gets the minimum cooldown time between shots.
        /// </summary>
        protected float MillisecondsToFire
        {
            get { return 60000 / fireRate; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicGun(IGunner owner, int damage, float range, int fireRate, float bulletSpeed, FireMode fireMode, 
            float recoilPerShot, float maxRecoil, int clipSize, int reserveSize, int reloadTime, World world, string weaponName, GunType gunType)
        {
            this.owner = owner;
            this.damage = damage;
            this.range = range;
            this.fireRate = fireRate;
            this.bulletSpeed = bulletSpeed;
            this.fireMode = fireMode;
            this.recoilPerShot = recoilPerShot;
            this.maxRecoil = maxRecoil;
            this.clipSize = clipSize;
            this.reserveSize = reserveSize;
            this.reloadTime = reloadTime;
            this.world = world;
            this.weaponName = weaponName;
            this.gunType = gunType;
            clipAmmo = clipSize;
            reserveAmmo = reserveSize;

            currentRecoil = 0;
            random = new Random();
        }

        public virtual void Update(GameTime gameTime)
        {
            timeSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            // recover recoil
            currentRecoil = MathHelper.Clamp(currentRecoil - owner.GunRecoilControl * world.SpeedCoeff, 0, maxRecoil);
            // auto reload when clip empty
            if (clipAmmo == 0 && !reloading && CurrentlyEquipped)
                Reload();
            // When reloading
            if (reloading)
            {
                if (!CurrentlyEquipped) // abort reload
                {
                    reloadCue.Stop(AudioStopOptions.AsAuthored);
                    reloading = false;
                    timeInReload = 0;
                }
                timeInReload += gameTime.ElapsedGameTime.Milliseconds;
                if (timeInReload > reloadTime)
                    InstaReload();
            }
        }

        public virtual bool Trigger(Vector2 direction, float directionOffset)
        {
            if (timeSinceLastShot > MillisecondsToFire && !reloading && clipAmmo > 0)
            {
                clipAmmo--;
                Texture2D pathTexture = world.bulletPathTextures[random.Next(world.bulletPathTextures.Count())];
                float offset = (float)random.NextDouble() * currentRecoil * (random.Next(2) == 1 ? -1 : 1);
                float bulletRotation = Vector2Extensions.VectorToRadians(direction) + offset;
                Vector2 bulletDirection = Vector2Extensions.RadiansToVector(bulletRotation);
                world.bulletList.Add(new Bullet(owner.Position + owner.Direction * directionOffset,
                    bulletDirection * bulletSpeed, damage, range, pathTexture, owner.Targets, (GeneralPed)owner, world));
                currentRecoil += recoilPerShot;
                timeSinceLastShot = 0;
                // play sound
                world.weaponSounds.PlayCue(WeaponName + "_fire");
                return true;
            }
            else
                return false;
        }

        public virtual bool Reload()
        {
            if (reserveAmmo > 0 && clipAmmo < clipSize && !reloading)
            {
                reloading = true;
                // play sound
                reloadCue = world.weaponSounds.GetCue(WeaponName + "_reload");
                reloadCue.Play();
                return true;
            }
            else
                return false;
        }

        public void InstaReload()
        {
            int totalAmmo = reserveAmmo + clipAmmo;
            if (totalAmmo < clipSize)
            {
                reserveAmmo = 0;
                clipAmmo = totalAmmo;
            }
            else
            {
                reserveAmmo = totalAmmo - clipSize;
                clipAmmo = clipSize;
            }
            timeInReload = 0;
            reloading = false;
        }

        public void AddAmmo(int amount)
        {
            reserveAmmo = Math.Min(reserveAmmo + amount, reserveSize);
        }

        public void Dispose()
        {
            if (reloadCue != null && reloadCue.IsPlaying) reloadCue.Stop(AudioStopOptions.Immediate);
        }

#region GUN MODS
        public void ModLazer()
        {
            if (hasLazer) throw new Exception("already have lazer");
            hasLazer = true;
            this.recoilPerShot = this.recoilPerShot * 0.75f;
            this.range = this.range * 2;
        }

        public void ModFullAuto()
        {
            if (this.fireMode == Weapons.FireMode.Auto || hasFullAuto) throw new Exception("already have full auto");
            else
            {
                hasFullAuto = true;
                this.fireMode = Weapons.FireMode.Auto;
                if (gunType == Weapons.GunType.Shotgun)
                {
                    fireRate = (int)(fireRate * 2f);
                    recoilPerShot = 0;
                }
            }
        }

        public void ModExtendedClip()
        {
            if (hasExtendedClip) throw new Exception("already have extended clip");
            hasExtendedClip = true;
            clipSize = (int)Math.Floor(clipSize * 1.4f);
        }

        public void ModFmj()
        {
            if (hasFmj) throw new Exception("already have fmj");
            hasFmj = true;
            damage *= 2;
        }

        public void ModMoreAmmo()
        {
            if (hasMoreAmmo) throw new Exception("already have more ammo");
            hasMoreAmmo = true;
            reserveSize = (int)(reserveSize * 1.5f);
        }

        public void ModFastReload()
        {
            if (hasFastReload) throw new Exception("already have fast reload");
            hasFastReload = true;
            if (this.gunType == Weapons.GunType.Shotgun)
            {
                Shotgun thisGun = this as Shotgun;
                thisGun.singleReloadTime = (int)(thisGun.singleReloadTime * 0.5f);
            }
            else reloadTime = (int)(reloadTime * 0.5f);
        }

        public float GetModValue()
        {
            float val = 1f;
            if (this.gunType == Weapons.GunType.Pistol)
            {
                val = 0f;
            }
            if (hasLazer) val += 0.2f;
            if (hasExtendedClip) val += 0.5f;
            if (hasFastReload) val += 1f;
            if (hasFmj) val += 4f;
            if (hasFullAuto) val += 0.2f;
            if (hasMoreAmmo) val += 2f;

            return val;
        }
#endregion
    }
}
