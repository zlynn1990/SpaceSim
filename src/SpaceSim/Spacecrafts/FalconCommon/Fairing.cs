using System;
using System.Drawing;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    class Fairing : SpaceCraftBase
    {
        public override string CraftName { get { return "DragonV2"; } }
        public override string CommandFileName { get { return "dragon.xml"; } }

        public override double Width { get { return 3.7; } }
        public override double Height { get { return 12.9311; } }

        public override double DryMass { get { return 875; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override double FormDragCoefficient
        {
            get { throw new NotImplementedException(); }
        }

        public override double FrontalArea
        {
            get { return 1; }
        }

        public override double ExposedSurfaceArea
        {
            get { throw new NotImplementedException(); }
        }

        public override double LiftCoefficient
        {
            get { throw new NotImplementedException(); }
        }

        public override Color IconColor { get { return Color.White; } }

        public Fairing(string craftDirectory, DVector2 position, DVector2 velocity)
            : base(craftDirectory, position, velocity, 0, 0, "Falcon/Common/fairing.png", null)
        {
        }
    }
}
