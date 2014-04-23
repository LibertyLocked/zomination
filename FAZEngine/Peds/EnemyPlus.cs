using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Vehicles;
using FAZEngine.Weapons;

namespace FAZEngine.Peds
{
    /// <summary>
    /// EnemyPlus is an enemy with guns.
    /// </summary>
    public abstract class EnemyPlus : Enemy, IDriver, IGunner
    {
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
        public EnemyPlus(Vector2 position, Vector2 speed, Vector2 direction, Texture2D sheetTexture, Point sheetSize, Point frameSize, int millisecondsPerFrame, int maxHealth, World world, float collisionRadius)
            : base(position, speed, direction, sheetTexture, sheetSize, frameSize, millisecondsPerFrame, maxHealth, world, collisionRadius)
        {
            GunsOwned = new List<BasicGun>();
            GunEquipped = 0;
            Targets = new List<GeneralPed>();
        }

        public override void Update(GameTime gameTime)
        {
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
            CarDriving.LetOutDriver();
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
                return GunsOwned[GunEquipped].Trigger(direction, 20);
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
            foreach (Player p in world.playerList)
            {
                Targets.Add(p);
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
            if (index < 0 || index >= GunsOwned.Count) throw new Exception("gun index out of range");
            else GunEquipped = index;
        }

        public void SwapGun()
        {
            int index = (GunEquipped + 1) % GunsOwned.Count;
            SwapGun(index);
        }

        #endregion
    }
}
