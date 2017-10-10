using System;
using System.Drawing;
using System.IO;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;

using SpaceSim.Properties;

namespace SpaceSim.Spacecrafts.NewGlenn
{
    class NGSatellite : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "Satellite.xml"; } }

        public override double Width { get { return 4.5; } }
        public override double Height { get { return 13.0; } }

        public override double DryMass
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.DryMass + _rightFairing.DryMass;
                }

                return 0;
            }
        }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        DateTime timestamp = DateTime.Now;

        public override double FormDragCoefficient
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.FormDragCoefficient + _rightFairing.FormDragCoefficient;
                }

                return 1;
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.LiftCoefficient + _rightFairing.LiftCoefficient;
                }

                return 1;
            }
        }

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea { get { return 2 * Math.PI * (Width / 2) * Height + FrontalArea; } }
        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        private NGFairing _leftFairing;
        private NGFairing _rightFairing;
        private bool _deployedFairings;
        private string _craftName;

        public NGSatellite(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 0, "Satellites/default.png")
        {
            _craftName = new DirectoryInfo(craftDirectory).Name;

            Engines = new IEngine[0];
        }

        public void AttachFairings(NGFairing leftFairing, NGFairing rightFairing)
        {
            _leftFairing = leftFairing;
            _rightFairing = rightFairing;

            _leftFairing.SetParent(this);
            _rightFairing.SetParent(this);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            if (!_deployedFairings)
            {
                _leftFairing.UpdateChildren(Position, Velocity);
                _rightFairing.UpdateChildren(Position, Velocity);

                _leftFairing.SetPitch(Pitch);
                _rightFairing.SetPitch(Pitch);
            }
        }

        public override void DeployFairing()
        {
            _deployedFairings = true;

            _leftFairing.Stage();
            _rightFairing.Stage();
        }

        public override void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            base.RenderGdi(graphics, cameraBounds);

            _leftFairing.RenderGdi(graphics, cameraBounds);
            _rightFairing.RenderGdi(graphics, cameraBounds);
        }

        protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
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
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10);
                File.AppendAllText(filename, contents);
            }

            base.RenderShip(graphics, cameraBounds, screenBounds);
        }
    }
}
