using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

using System;
using System.Drawing;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class FHBoosterB5 : F9S1Base
    {
        public override string CraftName { get { return Id == 1 ? "FH Booster PY" : "FH Booster MY"; } }

        public override string CommandFileName
        {
            get { return Id == 1 ? "FHLeftBooster.xml" : "FHRightBooster.xml"; }
        }

        public int Id { get; private set; }

        public override double DryMass { get { return 30000; } }

        public override double Width { get { return 4.11; } }
        public override double Height { get { return 46; } }

        public new AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsCrossSection; } }

        public FHBoosterB5(string craftDirectory, int id, DVector2 position, DVector2 velocity, double propellantMass = 425000)
            : base(craftDirectory, position, velocity, 5, propellantMass, "Falcon/Heavy/booster" + id + ".png", -18.35)
        {
            Id = id;

            StageOffset = Id == 1 ? new DVector2(-4, 0.8) : new DVector2(4, 0.8);

            Engines = new IEngine[9];

            for (int i = 0; i < 9; i++)
            {
                double engineOffsetX = (i - 4.0) / 4.0;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.45);

                Engines[i] = new Merlin1DB5(i, this, offset);
            }
        }
        
        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            // set the booster offsets according to the roll
            float rollFactor = (float)Math.Cos(Roll);
            StageOffset = Id == 1 ? new DVector2(-4 * rollFactor, 0.8) : new DVector2(4 * rollFactor, 0.8);

            base.RenderShip(graphics, camera, screenBounds);
        }
    }
}
