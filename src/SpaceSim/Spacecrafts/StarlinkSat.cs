using System;
using System.Drawing;
using System.IO;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts
{
    class StarlinkSat : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "StarlinkSat.xml"; } }

        public override double Width { get { return 1; } }
        public override double Height { get { return 1; } }

        public override double DryMass
        {
            get
            {
                return 125;
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

        DateTime timestamp = DateTime.Now;

        public StarlinkSat(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, double propellantMass = 125)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, "Satellites/default.png")
        {
            //_craftName = new DirectoryInfo(craftDirectory).Name;
            _craftName = "StarlinkSat";

            Engines = new IEngine[]
            {
                new StarDrive(0, this, new DVector2(0, Height * 0.33))
            };
        }

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            base.RenderGdi(graphics, camera);

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle\r\n");
                }

                timestamp = DateTime.Now;

                string contents = string.Format("{0}, {1}, {2}, {3}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 1000,
                    //this.GetRelativeAltitude() / 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 100);
                File.AppendAllText(filename, contents);
            }
        }
    }
}

