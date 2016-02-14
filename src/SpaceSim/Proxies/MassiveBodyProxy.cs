using System.Drawing;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Proxies
{
    /// <summary>
    /// Massive body proxy used for doing orbital approximations and traces.
    /// </summary>
    class MassiveBodyProxy : GravitationalBodyBase, IMassiveBody
    {
        public override double Mass { get { return _proxy.Mass; } }

        public double SurfaceRadius { get { return _proxy.SurfaceRadius; } }

        public double AtmosphereHeight { get {return _proxy.AtmosphereHeight; } }

        public double RotationRate { get { return _proxy.RotationRate; } }
        public double RotationPeriod { get { return _proxy.RotationPeriod; } }

        public Color IconAtmopshereColor { get { return _proxy.IconAtmopshereColor; } }

        private IMassiveBody _proxy;

        public MassiveBodyProxy(DVector2 position, DVector2 velocity, IMassiveBody massiveBody)
            : base(position, velocity, 0)
        {
            _proxy = massiveBody;
        }

        public double GetRelativeHeight(DVector2 position)
        {
            DVector2 difference = Position - position;

            double totalDistance = difference.Length();

            return totalDistance - _proxy.SurfaceRadius;
        }

        public double GetSurfaceGravity()
        {
            return _proxy.GetSurfaceGravity();
        }

        public double GetIspMultiplier(double height)
        {
            return _proxy.GetIspMultiplier(height);
        }

        public double GetAtmosphericDensity(double height)
        {
            return _proxy.GetAtmosphericDensity(height);
        }

        public override void Update(double dt)
        {
            // Integrate for position
            Velocity += (AccelerationG * dt);
            Position += (Velocity * dt);
        }
    }
}
