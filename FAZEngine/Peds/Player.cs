using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Environment;
using FAZEngine.Vehicles;
using FAZEngine.Weapons;

namespace FAZEngine.Peds
{
    public abstract class Player : GeneralPed, IDriver, IGunner
    {
        // Health regen fields
        protected int RegenIdleTime = 9000;
        protected int PerRegenTime = 70;
        protected int PerRegenHealth = 1;
        int timeSinceLastDamage = 0;
        int timeRegening = 0;
        protected float regenPercent = 1f;

        // RPG STATS
        int money = 0;
        int killCount = 0;
        int revives = 3;

        public int Money 
        { get { return money; } }

        public ShopArea InShop
        { get; set; }

        public int KillCount
        { get { return killCount; } }

        public int Revives
        { get { return revives; } }

        public List<BasicGun> GunsOwned
        {
            get;
            protected set;
        }

        public int GunEquipped
        {
            get;
            protected set;
        }

        public List<GeneralPed> Targets
        {
            get;
            protected set;
        }

        public float GunRecoilControl
        {
            get;
            protected set;
        }

        public BasicCar CarDriving
        {
            get;
            set;
        }

        public new Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Player(Vector2 position, Vector2 speed, Vector2 direction, Texture2D sheetTexture, Point sheetSize, Point frameSize, int millisecondsPerFrame, int maxHealth, World world, float collisionRadius)
            : base(position, speed, direction, sheetTexture, sheetSize, frameSize, millisecondsPerFrame, maxHealth, world, collisionRadius)
        {
            GunsOwned = new List<BasicGun>();
            GunEquipped = 0;
            Targets = new List<GeneralPed>();
        }

        public override void Update(GameTime gameTime)
        {
            // prevent move out
            if (!world.IsWithinMap(this))
            {
                // clamp postion
                this.position = Vector2.Clamp(this.position, Vector2.Zero, world.WorldSize);
            }

            // code for health regen
            timeSinceLastDamage += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastDamage > RegenIdleTime)
            {
                timeRegening += gameTime.ElapsedGameTime.Milliseconds;
                if (timeRegening > PerRegenTime)
                {
                    timeRegening -= PerRegenTime;
                    if (health + PerRegenHealth <= maxHealth * regenPercent)
                    {
                        this.GainHealth(PerRegenHealth);
                    }
                }
            }

            foreach (BasicGun gun in GunsOwned)
            {
                gun.Update(gameTime);
            }

            RefreshTargetList();

            base.Update(gameTime);
            if (CarDriving != null) this.position = CarDriving.Position;
        }

        public override void Draw(SpriteBatch spriteBatch, float layerDepth)
        {
            if (CarDriving == null) 
                base.Draw(spriteBatch, 1f);
        }

        public override void TakeDamage(int damage, GeneralPed attacker)
        {
            if (!IsInvincible)
            {
                timeSinceLastDamage = 0;
                timeRegening = 0;
            }
            base.TakeDamage(damage, attacker);
        }

        public void AddMoney(int amount)
        {
            if (amount < 0) return;
            money += amount;
        }

        public bool DeduceMoney(int amount)
        {
            if (money - amount < 0) return false;
            else
            {
                money -= amount;
                return true;
            }
        }

        public void AddKillCount()
        {
            killCount++;
        }

        public bool Lost1Revive()
        {
            if (revives < 1) return false;
            else
            {
                revives--;
                return true;
            }
        }

        public void Gain1Revive()
        {
            revives++;
        }

        #region All the IDriver methods
        public bool GetInCar()
        {
            if (world.basicCarList.Count == 0) return false;
            else
            {
                int carIndex = 0;
                for (int i = 0; i < world.basicCarList.Count; i++)
                {
                    if ((world.basicCarList[i].Position - this.position).LengthSquared() < 
                        (world.basicCarList[carIndex].Position - this.position).LengthSquared())
                    {
                        carIndex = i;
                    }
                }
                bool found = world.basicCarList[carIndex].LetInDriver(this);
                return found;
            }
        }

        public void GetOutCar()
        {
            if (CarDriving.IsDriver(this))
                CarDriving.LetOutDriver();
            else
                CarDriving.LetOutPassenger();
        }

        public void HitCarGas(float pressure)
        {
            CarDriving.GasPad(pressure);
        }

        public void HitCarBrake(float pressure)
        {
            CarDriving.BrakePad(pressure);
        }

        public void TurnCarWheels(float amount)
        {
             CarDriving.TurnWheels(amount);
        }

        public void ShiftCarGear(Gearbox gear)
        {
            CarDriving.ShiftGear(gear);
        }
        #endregion

        #region All the IGunOwner methods & other gun related stuff
        public virtual bool HitGunTrigger()
        {
            if (GunsOwned.Count > 0)
                return GunsOwned[GunEquipped].Trigger(direction, 40);
            else
                return false;
        }

        public virtual bool ReloadGun()
        {
            if (GunsOwned.Count > 0)
                return GunsOwned[GunEquipped].Reload();
            else
                return false;
        }

        protected virtual void RefreshTargetList()
        {
            Targets.Clear();
            foreach (Enemy e in world.enemyList)
            {
                Targets.Add(e);
            }
        }

        public virtual void GetGun(BasicGun gun)
        {
            GunsOwned.Add(gun);
        }

        public virtual void LoseGun(BasicGun gun)
        {
            GunsOwned.Remove(gun);
        }

        public virtual void SwapGun(int index)
        {
            if (index < 0 || index >= GunsOwned.Count)
            {
                MathHelper.Clamp(0, index, GunsOwned.Count);
                //throw new Exception("gun index out of range");
            }
            else GunEquipped = index;
        }

        public void SwapGun()
        {
            int index = (GunEquipped + 1) % GunsOwned.Count;
            SwapGun(index);
        }

        public virtual void GetMaxAmmo(bool withReload)
        {
            foreach (BasicGun gun in GunsOwned)
            {
                gun.AddAmmo(10000);
                if (withReload) gun.InstaReload();
                gun.AddAmmo(10000);
            }
        }

        #endregion
    }
}
