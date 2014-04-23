using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FAZEngine.Vehicles
{
    public interface IDriver
    {
        /// <summary>
        /// Gets or sets the car driver is in.
        /// </summary>
        BasicCar CarDriving
        { get; set; }

        Vector2 Position
        { get; set; }

        bool GetInCar();
        void GetOutCar();
        void HitCarGas(float pressure);
        void HitCarBrake(float pressure);
        void TurnCarWheels(float pressure);
        void ShiftCarGear(Gearbox gear);
    }
}
