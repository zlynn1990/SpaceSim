using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.SLS
{
    class ICPSFairing : SpaceCraftBase
    {
        public override string CraftName { get { return _isLeft ? "Fairing Left" : "Fairing Right"; } }
        public override string CommandFileName { get { return _isLeft ? "fairingLeft.xml" : "fairingRight.xml"; } }

        public override double Width { get { return 2.5; } }
        public override double Height { get { return 5.0; } }
        public override double DryMass { get { return 500; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double StagingForce { get { return 3000; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);

                double alpha = GetAlpha();

                return baseCd * Math.Cos(alpha);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);

                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2.0);
            }
        }

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + FrontalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        private bool _isLeft;

        public ICPSFairing(string craftDirectory, DVector2 position, DVector2 velocity, bool isLeft)
            : base(craftDirectory, position, velocity, 0, 0, isLeft ? "SLS/fairingIcpsLeft.png" : "SLS/fairingIcpsRight.png", null)
        {
            _isLeft = isLeft;

            if (_isLeft)
            {
                StageOffset = new DVector2(-1.24, -7.4);
            }
            else
            {
                StageOffset = new DVector2(1.24, -7.4);
            }

            Engines = new IEngine[0];
        }
    }
}
