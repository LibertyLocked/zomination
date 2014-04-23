using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Weapons;
using zomination.Weapons;
using zomination.PowerUps;

namespace zomination.Peds
{
    class EvilCowboy : EnemyPlus
    {
        static Random random = new Random();
        float accuracyOffset = 0.15f;
        float maxSpeed = 150f;
        float viewDistance = 910f;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EvilCowboy(Vector2 position, Texture2D sheetTexture, World world)
            : base(position, Vector2.Zero, Vector2.Zero, sheetTexture, new Point(6, 5), new Point(80, 104), 180, 100, world, 37)
        {
            GunRecoilControl = 0.004f;
            // bad cowboy could either carry an AK74u or R870MCS
            BasicGun cowboyGun;
            if (random.Next(0, 2) == 0)
            {
                cowboyGun = new AK74u(this, world);
                this.ChangeCurrentRow(2);
            }
            else
            {
                cowboyGun = new R870MCS(this, world);
                this.ChangeCurrentRow(1);
            }
            cowboyGun.FireRate = (int)(cowboyGun.FireRate * 0.7);
            this.GetGun(cowboyGun);
        }

        public override void Update(GameTime gameTime)
        {
            // get the closest player
            Player closestPlayer = GetClosestPlayer();
            if (closestPlayer != null)
            {
                // where to aim?
                float baseAim = Vector2Extensions.VectorToRadians(closestPlayer.Position - this.position);
                baseAim += (float)random.NextDouble() * accuracyOffset * (random.Next(2) == 1 ? -1 : 1);
                Aim(Vector2Extensions.RadiansToVector(baseAim));
                float distance = (closestPlayer.Position - this.position).Length();
                // move to player if out of gun range, or outside map
                if (!world.IsWithinMap(this) || distance > GunsOwned[GunEquipped].Range || distance > viewDistance)
                {
                    Vector2 speedToChase = closestPlayer.Position - this.position;
                    speedToChase.Normalize();
                    speedToChase *= maxSpeed;
                    speed = speedToChase;
                }
                else
                {
                    if (world.IsWithinMap(this)) HitGunTrigger();
                    speed = Vector2.Zero;
                }
                Aim(closestPlayer.Position - this.position);
            }
            base.Update(gameTime);
        }

        protected override void RefreshTargetList()
        {
            base.RefreshTargetList();
            foreach (Enemy e in world.enemyList)
            {
                Targets.Add(e);
            }
        }

        public override bool HitGunTrigger()
        {
            if (GunsOwned.Count > 0)
                return GunsOwned[GunEquipped].Trigger(direction, 48);
            else
                return false;
        }

        public override void TakeDamage(int damage, GeneralPed attacker)
        {
            base.TakeDamage(damage, attacker);
        }

        protected override void UponDeath(int damage, GeneralPed attacker)
        {
            Player player = attacker as Player;
            if (player != null)
            {
                player.AddMoney(20);
                player.AddKillCount();
                if (random.Next(100) < 80) // evil cowboys have 80% rate to drop power ups 
                {
                    int shuffle = random.Next(100);
                    if (shuffle < 25) world.powerUpList.Add(new MoneyPowerUp(this.position, world, 120));
                    else if (shuffle < 90) world.powerUpList.Add(new AmmoPowerUp(this.position, world));
                    else world.powerUpList.Add(new RevivePowerUp(this.position, world));
                }
                this.GunsOwned[GunEquipped].Dispose();  // Dispose current gun to prevent infinite reload loop
            }
            base.UponDeath(damage, attacker);
        }
    }
}
