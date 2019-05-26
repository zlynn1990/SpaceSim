using System;
using SpaceSim.Common;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class Fin : SpaceCraftPart, IFin
    {
        public override double Width { get; protected set; }
        public override double Height { get; protected set; }
        protected override double DrawingOffset { get; }

        public double Dihedral { get; protected set; }

        private double _offsetLength;
        private double _offsetRotation;
        private double _offsetRatio;

        private double _width;
        private double _height;

        public Fin(ISpaceCraft parent, DVector2 offset, DVector2 size, double dihedral = 0.0, string texturePath = "Textures/Spacecrafts/ITS/Fin.png", double offsetRatio = 1.5)
            : base(parent, texturePath)
        {
            _width = size.X;
            _height = size.Y;

            Width = _width;
            Height = _height;

            Dihedral = dihedral;

            DrawingOffset = 0.0;

            _offsetLength = offset.Length();
            _offsetRotation = offset.Angle() - Constants.PiOverTwo;
            _offsetRatio = offsetRatio;
        }

        public void SetDihedral(double targetAngle)
        {
            Dihedral = targetAngle;
        }

        public override void Update(double dt)
        {
            double rotation = _parent.Pitch - _offsetRotation;
            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            // handle dihedral
            offset.X += Math.Sin(Dihedral) * Math.Cos(rotation - Constants.PiOverTwo) * _width / _offsetRatio;
            offset.Y += Math.Sin(Dihedral) * Math.Sin(rotation - Constants.PiOverTwo) * _width / _offsetRatio;

            Position = _parent.Position - offset;

            Width = -Math.Sin(Dihedral) * _width;
        }
    }
}
