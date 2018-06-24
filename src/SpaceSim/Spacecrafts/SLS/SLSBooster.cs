using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

using System;
using System.Drawing;
using SpaceSim.Drawing;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class SLSBooster : SpaceCraftBase
    {
        public override string CraftName { get { return "SLS Booster " + Id; } }

        public override string CommandFileName
        {
            get { return Id == 1 ? "SLSLeftBooster.xml" : "SLSRightBooster.xml"; }
        }

        public int Id { get; private set; }

        public override double DryMass { get { return 98000; } }

        public override double Width { get { return 3.71; } }
        public override double Height { get { return 54.2; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsCrossSection; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
                return Math.Abs(dragCoefficient);
            }
        }

        public override double FrontalArea
        {
            get
            {
                double area = Math.PI * Math.Pow(Width / 2, 2);
                double alpha = GetAlpha();
                return Math.Abs(area * Math.Cos(alpha));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                return Width * Height;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();
                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override Color IconColor
        {
            get
            {
                return Color.White;
            }
        }

        public SLSBooster(string craftDirectory, int id, DVector2 position, DVector2 velocity, double propellantMass = 628000)
            : base(craftDirectory, position, velocity, 0, propellantMass, "SLS/booster" + id + ".png")
        {
            Id = id;

            StageOffset = Id == 1 ? new DVector2(-6, 4) : new DVector2(6, 4);

            Engines = new IEngine[1];
            var offset = new DVector2(0, Height * 0.45);
            Engines[0] = new SRB(0, this, offset);
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            // set the booster offsets according to the roll
            float rollFactor = (float)Math.Cos(Roll);
            StageOffset = Id == 1 ? new DVector2(-6 * rollFactor, 4) : new DVector2(6 * rollFactor, 4);

            base.RenderShip(graphics, camera, screenBounds);
        }
    }
}
