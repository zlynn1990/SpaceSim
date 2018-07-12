using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;
using SpaceSim.Drawing;
using SpaceSim.Properties;
using System.IO;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class SLSS1 : SpaceCraftBase
    {
        public override string CraftName { get { return "SLS Core"; } }
        public override string CommandFileName { get { return "SLSS1.xml"; } }

        public override double DryMass { get { return 85270; } }

        public override double Width { get { return 8.4; } }
        public override double Height { get { return 64.6; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get
            {
                return AeroDynamicProperties.ExtendsFineness;
            }
        }

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

        DateTime timestamp = DateTime.Now;

        public SLSS1(string craftDirectory, DVector2 position, DVector2 velocity, double propellantMass = 894182)
            : base(craftDirectory, position, velocity, 4, propellantMass, "SLS/S1.png")
        {
            StageOffset = new DVector2(0, 30);

            Engines = new IEngine[4];

            for (int i = 0; i < 4; i++)
            {
                double engineOffsetX = (i - 1.5) / 1.5;

                var offset = new DVector2(engineOffsetX * Width * 0.3, Height * 0.48);

                Engines[i] = new RS25(i, this, offset);
            }
        }

        //public override void RenderGdi(Graphics graphics, Camera camera)
        //{
        //    base.RenderGdi(graphics, camera);

        //    if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
        //    {
        //        string filename = MissionName + ".csv";

        //        if (!File.Exists(filename))
        //        {
        //            File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
        //        }

        //        timestamp = DateTime.Now;

        //        string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
        //            this.GetRelativeVelocity().Length() / 10,
        //            this.GetRelativeAcceleration().Length() * 100,
        //            this.GetRelativeAltitude() / 100,
        //            this.Throttle * 10);
        //        File.AppendAllText(filename, contents);
        //    }
        //}
    }
}
