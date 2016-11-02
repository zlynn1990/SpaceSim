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
        public override string CommandFileName { get { return "demosat.xml"; } }

        public override double Width { get { return 5.10655; } }
        public override double Height { get { return 12.9311; } }

        public override double DryMass { get { return _fairingMass; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

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

        public override double CrossSectionalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + CrossSectionalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        private double _fairingMass;

        public DemoSat(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 0, "Textures/fairing.png")
        {
            _fairingMass = 1750;

            Engines = new IEngine[0];
        }

        public override void DeployFairing()
        {
            _fairingMass = 0;
        }
    }
}
