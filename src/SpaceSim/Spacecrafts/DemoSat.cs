using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class DemoSat : SpaceCraftBase
    {
        public override double Width { get { return 5.10655; } }
        public override double Height { get { return 12.9311; } }

        public override double DryMass { get { return _dryMass; } }

        public override bool ExposedToAirFlow { get { return true; } }

        public override double DragCoefficient
        {
            get
            {
                if (MachNumber < 0.65 || MachNumber > 2.8)
                {
                    return 0.05;
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

                return 0.05 + normalizedMach * 0.3;
            }
        }

        // Base dome = 2 * pi * 1.85^2
        public override double CrossSectionalArea
        {
            get { return 21.504; }
        }

        public override Color IconColor { get { return Color.White; } }

        private double _dryMass;

        public DemoSat(DVector2 position, DVector2 velocity, double dryMass, double propellantMass)
            : base(position, velocity, propellantMass, "Textures/fairing.png")
        {
            _dryMass = dryMass;

            Engines = new IEngine[0];
        }

        public override string CommandFileName { get { return "demosat.xml"; } }

        public override string ToString()
        {
            return "DemoSat";
        }
    }
}
