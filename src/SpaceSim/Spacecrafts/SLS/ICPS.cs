using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using SpaceSim.Drawing;
using System.IO;
using SpaceSim.Properties;
using SpaceSim.Spacecrafts.SLS;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    sealed class ICPS : SpaceCraftBase
    {
        public override string CraftName { get { return "ICPS"; } }
        public override string CommandFileName { get { return "ICPS.xml"; } }

        public override double DryMass
        {
            get
            {
                if (!_deployedFairings)
                {
                    return 3490 + _leftFairing.DryMass + _rightFairing.DryMass;
                }

                return 3490;
            }
        }

        public override double Width { get { return 5.1; } }
        public override double Height { get { return 13.7; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.4);
                double alpha = GetAlpha();

                return Math.Abs(baseCd * Math.Cos(alpha));
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

        public override double FrontalArea { get { return Math.PI * Math.Pow(Width / 2, 2); } }
        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override Color IconColor { get { return Color.White; } }

        public ICPS(string craftDirectory, DVector2 position, DVector2 velocity, double zOffset = 9)
            : base(craftDirectory, position, velocity, 0, 27220, "SLS/ICPS.png")
        {
            StageOffset = new DVector2(0, zOffset);

            Engines = new IEngine[]
            {
                new RL10(0, this, new DVector2(0, Height * 0.33))
            };
        }

        private ICPSFairing _leftFairing;
        private ICPSFairing _rightFairing;
        private bool _deployedFairings;

        public override void Release()
        {
            _rightFairing.Release();
            _leftFairing.Release();

            base.Release();
        }

        public void AttachFairings(ICPSFairing leftFairing, ICPSFairing rightFairing)
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

        DateTime timestamp = DateTime.Now;
        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            base.RenderGdi(graphics, camera);

            _leftFairing.RenderGdi(graphics, camera);
            _rightFairing.RenderGdi(graphics, camera);

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromMilliseconds(1125)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle, Pressure, Heating\r\n");
                }

                timestamp = DateTime.Now;

                double targetVelocity = this.GetRelativeVelocity().Length();
                double density = this.GravitationalParent.GetAtmosphericDensity(this.GetRelativeAltitude());
                double dynamicPressure = 0.5 * density * targetVelocity * targetVelocity;

                string contents = string.Format("{0}, {1}, {2}, {3}, {4}, {5}\r\n",
                    targetVelocity,
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10,
                    dynamicPressure / 10,
                    this.HeatingRate / 10);

                File.AppendAllText(filename, contents);
            }
        }
    }
}

