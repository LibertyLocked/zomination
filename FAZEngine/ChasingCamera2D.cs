using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace FAZEngine
{
    public class ChasingCamera2D : Camera2D
    {
        protected Vector2 distance;
        protected Random rnd;

        // shaking variables
        protected bool isShaking;
        protected bool isMovingBack;
        protected double millisecondsInShaking;
        protected double millisecondsToShake;
        protected double timeSinceLastDirection;
        protected Vector2 shakingVector;
        protected float shakeCoefficient;
        protected PlayerIndex? playerToViberate;
        protected const double millisecondsPerDirection = 50;

        // chasing variables
        public float maxChaseSpeed = 1000;

        // constants
        protected const float maxShakePercentage = 0.2f;
        protected const float posError = 0.01f;
        protected const float chasingCoefficient = 0.4f;

        /// <summary>
        /// Check out if the camera is shaking
        /// </summary>
        public bool IsShaking
        {
            get { return isShaking; }
        }

        /// <summary>
        /// Constructs a shaking camera
        /// </summary>
        public ChasingCamera2D()
            : base()
        {
            rnd = new Random();
        }

        /// <summary>
        /// Updates the camera to chase to a position.
        /// </summary>
        /// <param name="gameTime">Game timer</param>
        /// <param name="chasePosition">The position to move</param>
        /// <param name="viewport">Viewport that is associated with the camera</param>
        public void Update(GameTime gameTime, Vector2 chasePosition, Viewport viewport)
        {
            distance = chasePosition - _pos;
            if (distance.Length() > maxChaseSpeed * posError && distance != Vector2.Zero)
            {
                UpdateChase(gameTime, viewport);
            }
            if (isShaking)
            {
                UpdateShake(gameTime, viewport);
            }
        }

        private void UpdateChase(GameTime gameTime, Viewport viewport)
        {
            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 speed = Vector2.Normalize(distance);
            float screenShorter = Math.Min(viewport.Height / 2, viewport.Width / 2);
            float chaseOffset = (float)Math.Pow(distance.Length() / screenShorter, 2);
            speed *= maxChaseSpeed * chaseOffset * Zoom;
            //speed *= maxChaseSpeed * distance.Length() / Math.Min(viewport.Height / 2, viewport.Width / 2) / chasingCoefficient * Zoom;
            Move(speed * deltaT);
        }

        private void UpdateShake(GameTime gameTime, Viewport viewport)
        {
            millisecondsInShaking += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (millisecondsInShaking < millisecondsToShake)
            {
                timeSinceLastDirection += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timeSinceLastDirection > millisecondsPerDirection)
                {
                    if (!isMovingBack)
                    {
                        rnd = new Random();
                        float dampingCofficient = 1 - (float)(millisecondsInShaking / millisecondsToShake);
                        float shakeVecX = (float)(rnd.NextDouble());
                        float shakeVecY = (float)(rnd.NextDouble());
                        //Trace.Write(shakeVecX);
                        //Trace.Write(shakeVecY);
                        shakingVector = new Vector2(shakeVecX, shakeVecY);
                        shakingVector.Normalize();
                        shakingVector *= maxShakePercentage * shakeCoefficient * dampingCofficient * Math.Min(viewport.Width, viewport.Height) * Zoom;
                    }
                    else
                    {
                        shakingVector *= -1;
                    }
                    isMovingBack = !isMovingBack;
                    timeSinceLastDirection -= millisecondsPerDirection;
                }
                this.Pos += shakingVector;
            }
            else
            {
                // ends shaking
                isShaking = false;
                if (playerToViberate != null)
                    GamePad.SetVibration((PlayerIndex)playerToViberate, 0, 0);
            }
        }

        /// <summary>
        /// Start a camera shake.
        /// </summary>
        /// <param name="millisecondsOfShaking">Camera shaking time</param>
        /// <param name="shakeCoefficient">Coefficient of shaking.</param>
        /// <param name="playerToViberate">Controller to viberate. Leave null then controller won't viberate.</param>
        /// <param name="viberation">Viberation coefficient</param>
        public void Shake(double millisecondsOfShaking, float shakeCoefficient, PlayerIndex? playerToViberate, float LVib, float RVib)
        {
            isShaking = true;
            this.shakeCoefficient = shakeCoefficient;
            millisecondsInShaking = 0;
            millisecondsToShake = millisecondsOfShaking;
            timeSinceLastDirection = millisecondsPerDirection;
            this.playerToViberate = playerToViberate;
            if (playerToViberate != null)
                GamePad.SetVibration((PlayerIndex)playerToViberate, LVib, RVib);
        }

        public void Shake(double millisecondsOfShaking, float shakeCoefficient)
        {
            this.Shake(millisecondsOfShaking, shakeCoefficient, null, 0, 0);
        }
    }
}
