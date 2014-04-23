using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FAZEngine.Peds;
using FAZEngine.Vehicles;
using ProjectMercury.Emitters;

namespace FAZEngine.Weapons
{
    public class Bullet
    {
        Vector2 startingPos, position, velocity;
        int damage;
        float range;
        List<GeneralPed> potentialTargets;
        Texture2D pathTexture;
        World world;

        GeneralPed attacker;
        float pathDrawLength;
        bool hit;
        Vector2 hitPoint;

        public bool Hit
        {
            get { return hit; }
        }

        private float Rotation
        {
            get { return Vector2Extensions.VectorToRadians(velocity); }
        }

        public bool RemoveMePlz
        {
            get { return hit && pathDrawLength == 0 ; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Bullet(Vector2 position, Vector2 velocity, int damage, float range, Texture2D pathTexture, List<GeneralPed> potentialTargets, GeneralPed attacker, World world)
        {
            this.position = position;
            this.velocity = velocity;
            this.damage = damage;
            this.range = range;
            this.pathTexture = pathTexture;
            this.potentialTargets = potentialTargets;
            this.attacker = attacker;
            this.world = world;

            hit = false;
            startingPos = position;
        }

        public void Update(GameTime gameTime)
        {
            Vector2 origVelocity = velocity;
            velocity *= world.SpeedCoeff;
            if (hit)
            {
                if (pathTexture == null) pathDrawLength = 0;
                else
                pathDrawLength = MathHelper.Clamp(pathTexture.Height - (hitPoint - position - velocity).Length(), 0, pathTexture.Height);
                // when shall we remove the bullet?
                if (pathDrawLength == 0)
                {
                    return;
                }
            }
            else
            {
                if (pathTexture != null)
                    pathDrawLength = MathHelper.Clamp((startingPos - position - velocity).Length(), 0, pathTexture.Height);

                // if going out of range
                if ((position + velocity - startingPos).Length() > range)
                {
                    hitPoint = startingPos + velocity / velocity.Length() * range;
                    hit = true;
                }
                // if still in range, check collision with targets
                else
                {
                    // collision with vehicles
                    if (!hit && world.basicCarList != null)
                    {
                        Vector2 point;
                        foreach (BasicCar c in world.basicCarList)
                        {
                            if (CollisionMath.RectangleLineCollide(c.CornerPositions, position, position + velocity, out point))
                            {
                                c.TakeDamage(damage);
                                hitPoint = point;
                                hit = true;
                                break;
                                //return;
                            }
                        }
                    }

                    // collision with target peds
                    if (!hit && potentialTargets != null)
                    {
                        CollisionMath.CircleLineCollisionResult r = new CollisionMath.CircleLineCollisionResult();
                        foreach (GeneralPed p in potentialTargets)
                        {
                            if (CollisionMath.CircleLineCollide(p.Position, p.CollisionRadius, position, position + velocity, ref r))
                            {
                                // do not damage peds when they are in cars!
                                IDriver driver = p as IDriver;
                                if (driver != null && driver.CarDriving != null) continue;

                                // ped hit!
                                if (p.Health > 0 && p.Health - damage <= 0 && !p.IsInvincible)
                                {
                                    // trigger death blast if ped isnt protected
                                    TriggerDeathBlast(p, origVelocity);
                                }
                                else
                                {
                                    // trigger blood blast
                                    TriggerBloodBlast(p, origVelocity);
                                }
                                p.TakeDamage(damage, attacker);
                                hitPoint = r.Point;
                                hit = true;
                                break;
                                //return;
                            }
                        }
                    }
                }
            }
            
            position += velocity;
            velocity = origVelocity;
        }

        public void Draw(SpriteBatch spriteBatch, float layer)
        {
            if (pathTexture == null) return;
            if (hit)
            {
                spriteBatch.Draw(pathTexture, hitPoint, 
                    new Rectangle(0, pathTexture.Height - (int)pathDrawLength, pathTexture.Width, (int)pathDrawLength), Color.White, Rotation,
                    new Vector2(pathTexture.Width / 2, 0), 1, SpriteEffects.None, layer);
            }
            else
            {
                spriteBatch.Draw(pathTexture, position, new Rectangle(0, 0, pathTexture.Width, (int)pathDrawLength), Color.White, Rotation,
                    new Vector2(pathTexture.Width / 2, 0), 1, SpriteEffects.None, layer);
            }
        }

        private void TriggerDeathBlast(GeneralPed ped, Vector2 origVelocity)
        {
            foreach (Emitter e in world.particleEffects[1])
            {
                e.ReleaseImpulse = origVelocity * 5;
            }
            world.particleEffects[1].Trigger(ped.Position);
        }

        private void TriggerBloodBlast(GeneralPed ped, Vector2 origVelocity)
        {
            foreach (Emitter e in world.particleEffects[0])
            {
                e.ReleaseImpulse = origVelocity * 10;
            }
            world.particleEffects[0].Trigger(ped.Position);
        }
    }
}
