using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

using System;
using System.Drawing;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.DeltaIV
{
    sealed class SideBooster : CommonBoosterCore
    {
        public override string CraftName { get { return "DIVH Booster " + Id; } }

        public override string CommandFileName
        {
            get { return Id == 1 ? "DHLeftBooster.xml" : "DHRightBooster.xml"; }
        }

        public int Id { get; private set; }

        public override double DryMass { get { return 26760; } }

        public override double Width { get { return 5.1; } }
        public override double Height { get { return 40.0; } }

        public new AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsCrossSection; } }

        public SideBooster(string craftDirectory, int id, DVector2 position, DVector2 velocity, double propellantMass = 199640)
            : base(craftDirectory, position, velocity, propellantMass, "DeltaIV/booster" + id + ".png")
        {
            Id = id;

            StageOffset = Id == 1 ? new DVector2(-5.1, 0.5) : new DVector2(5.1, 0.5);

            Engines = new IEngine[1];
            var offset = new DVector2(0, Height * 0.48);
            Engines[0] = new RS68A(0, this, offset);
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            // set the booster offsets according to the roll
            float rollFactor = (float)Math.Cos(Roll);
            StageOffset = Id == 1 ? new DVector2(-5.1 * rollFactor, 0.5) : new DVector2(5.1 * rollFactor, 0.5);

            base.RenderShip(graphics, camera, screenBounds);
        }
    }
}
