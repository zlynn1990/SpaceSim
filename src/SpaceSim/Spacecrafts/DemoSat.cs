using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class DemoSat : SpaceCraftBase
    {
        public override double Width { get { return 5.10655; } }
        public override double Height { get { return 12.9311; } }

        public override double DryMass { get { return _dryMass + _fairingMass; } }

        public override bool ExposedToAirFlow { get { return true; } }

        public override double DragCoefficient
        {
            get
            {
                if (MachNumber < 0.65 || MachNumber > 2.8)
                {
                    return 0.25;
                }

                double normalizedMach;

                if (MachNumber < 1.5)
                {
                    normalizedMach = (MachNumber - 0.65) * 1.17;
                }
                else
                {
                    normalizedMach = (2.8 - MachNumber) * 0.769;
                }

                return 0.25 + normalizedMach * 0.35;
            }
        }

        // Fairing
        public override double CrossSectionalArea { get { return Math.PI * 2.6 * 2.6; } }

        public override Color IconColor { get { return Color.White; } }

        private double _dryMass;
        private double _fairingMass;

        public DemoSat(DVector2 position, DVector2 velocity, double dryMass, double propellantMass)
            : base(position, velocity, propellantMass, "Textures/fairing.png")
        {
            _dryMass = dryMass;
            _fairingMass = 1750;

            Engines = new IEngine[0];
        }

        public override string CommandFileName { get { return "demosat.xml"; } }

        public override void DeployFairing()
        {
            _fairingMass = 0;
        }

        public override string ToString()
        {
            return "DemoSat";
        }
    }
}
