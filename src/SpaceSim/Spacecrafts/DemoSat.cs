using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class DemoSat : SpaceCraftBase
    {
        public override string CraftName {get { return "Satellite"; }}

        public override double Width { get { return 5.10655; } }
        public override double Height { get { return 12.9311; } }

        public override double DryMass { get { return _dryMass + _fairingMass; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();
                double cosAlpha = Math.Cos(alpha);
                return baseCd * cosAlpha;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                double sinAlpha = Math.Sin(alpha * 2.0);
                double alphaCd = baseCd * sinAlpha;
                return alphaCd;
            }
        }

        public override double CrossSectionalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + CrossSectionalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public override string CommandFileName { get { return "demosat.xml"; } }

        private double _dryMass;
        private double _fairingMass;

        public DemoSat(string craftDirectory, DVector2 position, DVector2 velocity, double dryMass, double propellantMass)
            : base(craftDirectory, position, velocity, propellantMass, "Textures/fairing.png")
        {
            _dryMass = dryMass;
            _fairingMass = 1750;

            Engines = new IEngine[0];
        }

        public override void DeployFairing()
        {
            _fairingMass = 0;
        }
    }
}
