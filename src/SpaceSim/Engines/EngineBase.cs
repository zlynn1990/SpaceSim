using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Engines
{
    abstract class EngineBase : IEngine
    {
        public bool IsActive { get; protected set; }

        public double Throttle { get; protected set; }

        private ISpaceCraft _parent;

        private double _offsetLength;
        private double _offsetRotation;

        private EngineFlame _engineFlame;

        protected EngineBase(ISpaceCraft parent, DVector2 offset, EngineFlame flame)
        {
            _parent = parent;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Math.PI / 2.0;

            _engineFlame = flame;
        }

        public virtual void Startup()
        {
            IsActive = true;
        }

        public virtual void Shutdown()
        {
            IsActive = false;
        }

        public void AdjustThrottle(double targetThrottle)
        {
            Throttle = targetThrottle;
        }

        public abstract double Thrust(double ispMultiplier);

        public abstract double MassFlowRate();

        public void Update(TimeStep timeStep)
        {
            double rotation = _parent.Rotation -_offsetRotation;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            double throttle = (IsActive && _parent.PropellantMass > 0) ? Throttle : 0;

            _engineFlame.Update(timeStep, _parent.Position - offset, _parent.Velocity, _parent.Rotation, throttle);
        }

        public void Draw(Graphics graphics, RectangleD cameraBounds)
        {
            _engineFlame.Draw(graphics, cameraBounds);
        }
    }
}
