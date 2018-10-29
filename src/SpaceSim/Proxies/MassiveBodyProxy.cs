using System;
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

        public string ApoapsisName { get; }
        public string PeriapsisName { get; }

        public double SurfaceRadius { get { return _proxy.SurfaceRadius; } }
        public double AtmosphereHeight { get {return _proxy.AtmosphereHeight; } }

        public double RotationRate { get { return _proxy.RotationRate; } }
        public double RotationPeriod { get { return _proxy.RotationPeriod; } }

        public override Color IconColor { get { return Color.White; } }

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

        public double GetIspMultiplier(double altitude)
        {
            return _proxy.GetIspMultiplier(altitude);
        }

        public double GetAtmosphericDensity(double altitude)
        {
            return _proxy.GetAtmosphericDensity(altitude);
        }

        public double GetAtmosphericViscosity(double altitude)
        {
            return _proxy.GetAtmosphericViscosity(altitude);
        }

        public double GetSurfaceAngle(DateTime localTime, IMassiveBody sun)
        {
            return _proxy.GetSurfaceAngle(localTime, sun);
        }

        public double GetSpeedOfSound(double altitude)
        {
            return _proxy.GetSpeedOfSound(altitude);
        }

        public override void Update(double dt)
        {
            // Integrate for position
            Velocity += (AccelerationG * dt);
            Position += (Velocity * dt);
        }

        public override void FixedUpdate(TimeStep timeStep)
        {
            throw new System.NotImplementedException();
        }

        public override double Visibility(RectangleD cameraBounds)
        {
            throw new System.NotImplementedException();
        }

        public override RectangleD ComputeBoundingBox()
        {
            throw new System.NotImplementedException();
        }
    }
}
