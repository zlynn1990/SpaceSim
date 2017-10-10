using System;
using System.Drawing;
using System.IO;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class Carousel : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "Carousel.xml"; } }

        public override double Width { get { return 7; } }
        public override double Height { get { return 10; } }

        public override double DryMass
        {
            get
            {
                return 10000;
            }
        }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.None; } }

        public override double FormDragCoefficient
        {
            get
            {
                return 1;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                return 1;
            }
        }

        public override double FrontalArea
        {
            get
            {
                return 1;
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                return 1;
            }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                return 1;
            }
        }

        public override Color IconColor { get { return Color.White; } }

        private string _craftName;

        public Carousel(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 0, "Satellites/carousel.png")
        {
            _craftName = new DirectoryInfo(craftDirectory).Name;

            Engines = new IEngine[0];
        }
    }
}
