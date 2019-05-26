using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class LAS : SpaceCraftBase
    {
        public override string CraftName { get { return "LAS"; } }
        public override string CommandFileName { get { return "LAS.xml"; } }

        public override double Width { get { return 5.2; } }
        public override double Height { get { return 11.6 + 3.3 + 2.46; } }
        public override double DryMass { get { return 3696; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double StagingForce { get { return 1500; } }

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

        public LAS(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 2480)
            : base(craftDirectory, position, velocity, 0, propellantMass, "SLS/LAS.png", null)
        {
            StageOffset = new DVector2(0, -3.2);

            Engines = new IEngine[]
            {
                new OrionAbortMotor(0, this, new DVector2(-0.2, -3.5), -0.5),
                new OrionAbortMotor(1, this, new DVector2(-0.05, -3.5), -0.1),
                new OrionAbortMotor(2, this, new DVector2(0.05, -3.5), 0.1),
                new OrionAbortMotor(3, this, new DVector2(0.2, -3.5), 0.5)
            };
        }
    }
}