using System;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Vehicles;
using FAZEngine.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using zomination.PowerUps;

namespace zomination.Peds
{
    public class Cowboy : Player
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Cowboy(Vector2 position, Vector2 direction, Texture2D sheetTexture, World world)
            : base(position, Vector2.Zero, direction, sheetTexture, new Point(6, 5), new Point(80, 104), 180, 1000, world, 37)
        {
            GunRecoilControl = 0.005f;

            RegenIdleTime = 6000;
            PerRegenTime = 50;
            PerRegenHealth = 1;
        }

        public override void Update(GameTime gameTime)
        {
            if (world.SpeedCoeff != 1) this.GunsOwned[GunEquipped].InstaReload();
            base.Update(gameTime);
        }

        public override void SwapGun(int index)
        {
            base.SwapGun(index);
            switch (GunsOwned[GunEquipped].GunType)
            {
                case GunType.Pistol:
                    ChangeCurrentRow(0);
                    break;
                case GunType.Shotgun:
                    ChangeCurrentRow(1);
                    break;
                case GunType.SMG:
                    ChangeCurrentRow(2);
                    break;
                case GunType.AssultRifle:
                    ChangeCurrentRow(3);
                    break;
                case GunType.LMG:
                    ChangeCurrentRow(4);
                    break;
            }
        }

        public override bool HitGunTrigger()
        {
            if (GunsOwned.Count > 0)
                return GunsOwned[GunEquipped].Trigger(direction, 48);
            else
                return false;
        }

        protected override void RefreshTargetList()
        {
            base.RefreshTargetList();
            if (OptionsMenuScreen.friendlyFire)
            {
                foreach (Player p in world.playerList)
                {
                    if (p != this) Targets.Add(p);
                }
            }
        }

        protected override void UponDeath(int damage, GeneralPed attacker)
        {
            world.SlowDown(3000, 0.2f);
            world.powerUpList.Add(new MoneyPowerUp(this.position, world, (int)Math.Floor(Money * 0.1f)));
            this.DeduceMoney((int)Math.Floor(Money * 0.1f));
            base.UponDeath(damage, attacker);
        }
    }
}
