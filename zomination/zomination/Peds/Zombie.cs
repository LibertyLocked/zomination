using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine;
using FAZEngine.Peds;
using FAZEngine.Weapons;
using zomination.PowerUps;

namespace zomination.Peds
{
    public class Zombie : Enemy
    {
        static Random rnd = new Random();
        protected float maxSpeed;

        protected int millisecondsPerAttack = 400;    // milliseconds
        protected int attackRadius = 100;
        protected int attackDamage = 85;
        protected int timeSinceLastAttack = 0;
        List<GeneralPed> targetList = new List<GeneralPed>();

        public Zombie(Vector2 position, float maxSpeed, Texture2D sheetTexture, World world)
            : base(position, Vector2.Zero, Vector2.Zero, sheetTexture, new Point(6, 1), new Point(80, 104), 200, 300, world, 37)
        {
            this.maxSpeed = maxSpeed;
            this.timeSinceLastAttack = this.millisecondsPerAttack;
        }

        public override void Update(GameTime gameTime)
        {
            Player playerToChase = GetClosestPlayer();
            if (playerToChase != null)
            {
                // refresh target list
                targetList.Clear();
                targetList.Add(playerToChase);

                Vector2 speedToChase = playerToChase.Position - this.position;
                speedToChase.Normalize();
                speedToChase *= maxSpeed;
                Aim(speedToChase);
                // if too close, STOP!
                if ((playerToChase.Position - this.position).Length() < attackRadius / 2) this.speed = Vector2.Zero;

                // attacking
                timeSinceLastAttack += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastAttack > millisecondsPerAttack)
                    timeSinceLastAttack = millisecondsPerAttack;

                if (timeSinceLastAttack >= millisecondsPerAttack)
                {
                    if ((position - playerToChase.Position).Length() < attackRadius)
                    {
                        timeSinceLastAttack = 0;
                        Attack(playerToChase);
                    }
                    speed = speedToChase;
                }
                else
                {
                    speed = Vector2.Zero;
                }
            }
            else
            {
                // when no player to chase
                //this.speed = Vector2.Zero ;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Attack target player using fake bullets.
        /// </summary>
        /// <param name="playerToAttack"></param>
        private void Attack(Player playerToAttack)
        {
            Vector2 direction = playerToAttack.Position - this.position;
            world.bulletList.Add(new Bullet(this.position, direction / direction.Length() * 30, attackDamage, attackRadius, null, targetList, this, world));
        }

        protected override void UponDeath(int damage, GeneralPed attacker)
        {
            Player player = attacker as Player;
            if (player != null)
            {
                player.AddMoney(1);
                player.AddKillCount();
                if (rnd.Next(100) < 11) // zombies have 11% rate to drop power ups 
                {
                    int shuffle = rnd.Next(100);
                    if (shuffle < 55) world.powerUpList.Add(new MoneyPowerUp(this.position, world, rnd.Next(14, 30)));
                    else if (shuffle < 75) world.powerUpList.Add(new AmmoPowerUp(this.position, world));
                    else if (shuffle < 90) world.powerUpList.Add(new HealthPowerUp(this.position, world));
                    else if (shuffle < 96) world.powerUpList.Add(new ShieldPowerUp(this.position, world));
                    else world.powerUpList.Add(new RevivePowerUp(this.position, world));
                }
            }
            base.UponDeath(damage, attacker);
        }
    }
}
