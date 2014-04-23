using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Sprites
{
    /// <summary>
    /// A moving sprite that is animated.
    /// </summary>
    public abstract class AnimatedSprite : MovingSprite
    {
        protected Texture2D sheetTexture;
        protected Point sheetSize;      // SheetSize is actually number of pics in row and col
        protected Point frameSize;      // FrameSize is the size of texture to draw
        protected Point currentFrame;
        protected int millisecondsPerFrame;
        protected int timeSinceLastFrame;
        protected Vector2 direction, prevDirection;
        protected bool getDirectionFromSpeed = false;

        private bool playing;

        /// <summary>
        /// Gets the origin of the texture (aka center coord of texture)
        /// </summary>
        public Vector2 TextureOrigin
        {
            get { return new Vector2(frameSize.X / 2, frameSize.Y / 2); }
        }

        /// <summary>
        /// Gets the direction of the animated sprite
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
        }

        /// <summary>
        /// Gets the state whether the sprite should update its direction from speed.
        /// </summary>
        protected bool GetDirectionFromSpeed
        {
            get { return !getDirectionFromSpeed; }
        }

        /// <summary>
        /// Gets the state of the animation playing.
        /// </summary>
        protected bool IsPlaying
        {
            get { return playing; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">Initial position of the sprite</param>
        /// <param name="speed">Initial speed of the sprite</param>
        /// <param name="direction">Directional vector.</param>
        /// <param name="sheetTexture">Sheet texture</param>
        /// <param name="sheetSize">Row num and col num of pictures</param>
        /// <param name="frameSize">Frame size</param>
        /// <param name="millisecondsPerFrame">Animation playing speed</param>
        public AnimatedSprite(Vector2 position, Vector2 speed, Vector2 direction, Texture2D sheetTexture, Point sheetSize, Point frameSize, int millisecondsPerFrame)
            : base(position, speed)
        {
            this.direction = direction;
            this.sheetTexture = sheetTexture;
            this.sheetSize = sheetSize;
            this.frameSize = frameSize;
            this.millisecondsPerFrame = millisecondsPerFrame;
            this.currentFrame = new Point(0, 0);
        }

        /// <summary>
        /// Changes the row of current frame in order to play other animations.
        /// </summary>
        /// <param name="rowIndex">Row index to change in sheet</param>
        protected void ChangeCurrentRow(int rowIndex)
        {
            if (rowIndex >= frameSize.Y || rowIndex < 0) throw new Exception("row index out of range");
            this.currentFrame.Y = rowIndex;
        }

        /// <summary>
        /// Updates the animation
        /// </summary>
        /// <param name="gameTime"></param>
        public new virtual void Update(GameTime gameTime)
        {
            prevDirection = direction;

            if (speed != Vector2.Zero && getDirectionFromSpeed)
            {
                direction = speed;
                direction.Normalize();
            }

            if (playing)
            {
                if (timeSinceLastFrame >= millisecondsPerFrame)
                {
                    timeSinceLastFrame -= millisecondsPerFrame;
                    currentFrame.X++;
                    if (currentFrame.X >= sheetSize.X) currentFrame.X = 0;
                }
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Start playing animation
        /// </summary>
        public virtual void StartAnimation()
        {
            if (playing) throw new Exception("animation alrady started");
            timeSinceLastFrame = 0;
            playing = true;
            currentFrame.X = 0;
        }

        /// <summary>
        /// Stop playing animation
        /// </summary>
        public virtual void StopAnimation()
        {
            if (!playing) throw new Exception("animation already stopped");
            timeSinceLastFrame = 0;
            playing = false;
            currentFrame.X = 0;
        }

        /// <summary>
        /// Draws the animated sprite
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch, float layerDepth)
        {
            spriteBatch.Draw(sheetTexture, position,
                new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
                Color.White, GetRotation(), TextureOrigin, 1f, SpriteEffects.None, layerDepth);
        }

        /// <summary>
        /// Finds the rotation (in radians)
        /// </summary>
        protected virtual float GetRotation()
        {
            if (direction == null || direction == Vector2.Zero)
                return Vector2Extensions.VectorToRadians(speed);
            else
            {
                return Vector2Extensions.VectorToRadians(direction);
            }
        }

        /// <summary>
        /// Changes the direction of the sprite, until its movement status is changed
        /// </summary>
        protected virtual void ChangeDirection(Vector2 direction)
        {
            this.direction = direction;
        }
    }
}
