using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine.Vehicles
{
    public abstract class BasicCar
    {
        // General fields
        protected Vector2 position;
        protected float speed;
        protected float acceleration;
        protected float carRotation;
        protected float wheelsRotation;
        protected Gearbox gear;
        protected Vector2 origin;
        protected IDriver driver;
        protected IDriver passenger;
        protected World world;
        protected int health;

        // Unique car property fields
        protected float maxAcc;
        protected float maxBrake;
        protected float maxSpeed;
        protected float maxTurn;
        protected float fC;
        protected Texture2D carTexture;
        protected Texture2D wheelsTexture;
        protected Vector2 leftWheelOffset;
        protected int maxHealth;

        // Customizable fields
        protected float wheelDrawRotationMultiplier = 20;

        #region Public Properties

        /// <summary>
        /// Gets the direction that the car is facing.
        /// </summary>
        public Vector2 CarDirection
        {
            get { return Vector2Extensions.RadiansToVector(carRotation); }
        }

        /// <summary>
        /// Gets the rotation of the car. (relative to car rotation)
        /// </summary>
        public float CarRotation
        {
            get { return carRotation; }
        }

        public Vector2 WheelsDirection
        {
            get { return Vector2Extensions.RadiansToVector(wheelsRotation); }
        }

        public float WheelsRotation
        {
            get { return wheelsRotation; }
        }

        public Texture2D Texture
        {
            get { return carTexture; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public Gearbox Gear
        {
            get { return gear; }
        }

        public int Health
        {
            get { return health; }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
        }

        public virtual bool RemoveMePlz
        {
            get { return (health <= 0); }
        }

        public Vector2[] CornerPositions
        {
            get
            {
                Vector2 leftTop = position + Vector2Extensions.RotateVector(-origin, carRotation);
                Vector2 rightTop = position + Vector2Extensions.RotateVector(new Vector2(Texture.Width - origin.X, -origin.Y), carRotation);
                Vector2 leftBottom = position + Vector2Extensions.RotateVector(new Vector2(-origin.X, Texture.Height - origin.Y), carRotation);
                Vector2 rightButtom = position + Vector2Extensions.RotateVector(new Vector2(Texture.Width - origin.X, Texture.Height - origin.Y), carRotation);
                return (new Vector2[] { leftTop, rightTop, leftBottom, rightButtom });
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BasicCar(Vector2 position, Vector2 origin, float carRotation, float maxAcc, float maxBrake, 
            float fC, float maxTurn, Texture2D carTexture, Texture2D wheelsTexture, Vector2 leftWheelPos, int maxHealth, World world)
        {
            acceleration = 0;
            speed = 0;
            gear = Gearbox.D;

            this.position = position;
            this.origin = origin;
            this.carRotation = carRotation;
            this.maxAcc = maxAcc;
            this.maxBrake = maxBrake;
            this.fC = fC;
            this.maxTurn = maxTurn;
            this.carTexture = carTexture;
            this.wheelsTexture = wheelsTexture;
            this.maxHealth = maxHealth;

            this.maxSpeed = maxAcc / fC;
            this.health = maxHealth;

            // find left wheel offset
            leftWheelOffset = leftWheelPos - origin;
        }
        
        /// <summary>
        /// Updates the car.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // fake frictions!
            if (driver == null)
            {
                acceleration = 0;
                TurnWheels(0);
            }
            acceleration -= fC * speed;

            // there is no way the car shall be driving backward when its on D gear.
            if (gear == Gearbox.D && speed < 0) speed = 0;
            else if (gear == Gearbox.R && speed > 0) speed = 0;
            // dt
            float dt = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
            // v = v0 + a * dt
            speed += acceleration * dt;
            // x = x0 + v * dt
            position += speed * dt * Vector2Extensions.RadiansToVector(carRotation + wheelsRotation);

            // Fix rotation between -2pi and 2pi
            carRotation %= 2 * (float)Math.PI;
            wheelsRotation %= 2 * (float)Math.PI;

            // Car rotation update
            carRotation += wheelsRotation * speed / maxSpeed;

            // zero speed fixes
            if (Gear == Gearbox.D && acceleration < 0 && speed < 0) speed = 0;
            if (Gear == Gearbox.R && acceleration > 0 && speed > 0) speed = 0;
        }

        /// <summary>
        /// Turn car wheels.
        /// </summary>
        /// <param name="pressure">1 fully right, -1 fully left</param>
        public void TurnWheels(float pressure)
        {
            if (Gear == Gearbox.D)
                wheelsRotation = pressure * maxTurn;
            else if (Gear == Gearbox.R)
                wheelsRotation = pressure * maxTurn;
        }

        /// <summary>
        /// Accelerate the car.
        /// </summary>
        /// <param name="pressure">1 fully accelerate, 0 no acceleration</param>
        public void GasPad(float pressure)
        {
            if (Gear == Gearbox.D)
            {
                acceleration = maxAcc * pressure;
            }
            else if (Gear == Gearbox.R)
            {
                acceleration = -maxAcc * pressure * 0.6f;
            }
        }

        /// <summary>
        /// Brake the car.
        /// </summary>
        /// <param name="pressure">1 fully brake, 0 no brake.</param>
        public void BrakePad(float pressure)
        {
            if (speed > 0)
            {
                acceleration = -maxBrake * pressure;
            }
            else if (speed < 0)
            {
                acceleration = maxBrake * pressure;
            }
            if ((gear == Gearbox.D && speed <= 0) || (gear == Gearbox.R && speed >= 0))
                acceleration = 0;
        }

        /// <summary>
        /// Shift gearbox!
        /// </summary>
        /// <param name="gear">Gear to switch to</param>
        public void ShiftGear(Gearbox gear)
        {
            if (speed != 0)
                BrakePad(1);
            else
                this.gear = gear;
        }

        /// <summary>
        /// Draws the car.
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Draw wheels
            float lWOffsetAngle = Vector2Extensions.VectorToRadians(leftWheelOffset);
            Vector2 lWROffset = Vector2Extensions.RadiansToVector(lWOffsetAngle + this.CarRotation) * leftWheelOffset.Length();
            float rWOffsetAngle = -lWOffsetAngle;
            Vector2 rWROffset = Vector2Extensions.RadiansToVector(rWOffsetAngle + this.CarRotation) * leftWheelOffset.Length();

            spriteBatch.Draw(wheelsTexture, Position + lWROffset, null, Color.White, carRotation + wheelsRotation * wheelDrawRotationMultiplier,
                new Vector2(wheelsTexture.Width, wheelsTexture.Height) / 2, 1, SpriteEffects.None, 0.9f);
            spriteBatch.Draw(wheelsTexture, Position + rWROffset, null, Color.White, carRotation + wheelsRotation * wheelDrawRotationMultiplier,
                new Vector2(wheelsTexture.Width, wheelsTexture.Height) / 2, 1, SpriteEffects.None, 0.9f);

            // Draw car
            spriteBatch.Draw(carTexture, Position, null, Color.White, CarRotation, origin, 1, SpriteEffects.None, 1f);
        }

        public bool LetInDriver(IDriver driver)
        {
            if (this.driver != null) return LetInPassenger(driver);
            if (driver.CarDriving == null && GetDistanceWithDriver(driver) < 130)
            {
                driver.CarDriving = this;
                this.driver = driver;
                return true;
            }
            else return false;
        }

        public bool LetInPassenger(IDriver passenger)
        {
            if (passenger.CarDriving == null && GetDistanceWithDriver(passenger) < 130)
            {
                passenger.CarDriving = this;
                this.passenger = passenger;
                return true;
            }
            else return false;
        }

        public bool LetOutPassenger()
        {
            if (passenger != null && passenger.CarDriving == this)
            {
                passenger.CarDriving = null;
                passenger.Position = position + Vector2Extensions.RadiansToVector(carRotation + (float)Math.PI / 2) * 130;
                this.passenger = null;
                return true;
            }
            else return false;
        }

        public bool LetOutDriver()
        {
            if (driver != null && driver.CarDriving == this)
            {
                driver.CarDriving = null;
                driver.Position = position - Vector2Extensions.RadiansToVector(carRotation + (float)Math.PI / 2) * 130;
                this.driver = null;
                return true;
            }
            else return false;
        }

        public float GetDistanceWithDriver(IDriver driver)
        {
            return ((this.position - driver.Position).Length());
        }

        public float GetSpeed()
        {
            return speed;
        }

        public float GetAcceleration()
        {
            return acceleration;
        }

        public void TakeDamage(int damage)
        {
            this.health -= damage;
            if (health <= 0)
            {
                this.LetOutDriver();
                this.LetOutPassenger();
            }
        }

        public bool IsDriver(IDriver tester)
        {
            if (driver != null && tester == driver) return true;
            else return false;
        }
    }
}
