using System;
using System.Drawing;
using System.IO;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using SpaceSim.Properties;
using SpaceSim.Spacecrafts.FalconCommon;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconHeavy
{
    class Roadster : SpaceCraftBase
    {
        public override string CraftName { get { return _craftName; } }
        public override string CommandFileName { get { return "Roadster.xml"; } }

        public override double Width { get { return 4; } }
        public override double Height { get { return 4; } }

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

        public override double FrontalArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.FrontalArea + _rightFairing.FrontalArea;
                }

                return 1;
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.ExposedSurfaceArea + _rightFairing.ExposedSurfaceArea;
                }

                return 1;
            }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                if (!_deployedFairings)
                {
                    return _leftFairing.LiftingSurfaceArea + _rightFairing.LiftingSurfaceArea;
                }

                return 1;
            }
        }

        public override Color IconColor { get { return Color.White; } }

        private string _craftName;

        private Fairing _leftFairing;
        private Fairing _rightFairing;
        private bool _deployedFairings;
        DateTime timestamp = DateTime.Now;

        public Roadster(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 0, "Falcon/Heavy/roadster.png")
        {
            _craftName = new DirectoryInfo(craftDirectory).Name;

            Engines = new IEngine[0];
        }

        public void AttachFairings(Fairing leftFairing, Fairing rightFairing)
        {
            _leftFairing = leftFairing;
            _rightFairing = rightFairing;

            _leftFairing.SetParent(this);
            _rightFairing.SetParent(this);
        }

        public override void Release()
        {
            _rightFairing.Release();
            _leftFairing.Release();

            base.Release();
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

        public override void RenderGdi(Graphics graphics, Camera camera)
        {
            base.RenderGdi(graphics, camera);

            if (!_leftFairing.Terminated && !_rightFairing.Terminated)
            {
                _leftFairing.RenderGdi(graphics, camera);
                _rightFairing.RenderGdi(graphics, camera);
            }

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle, Downrange\r\n");
                }

                timestamp = DateTime.Now;

                //{ { -99014944588.2743,-109869666553.046} }
                //{ { -99015072098.6158,-109869554508.308}}

                string contents = string.Format("{0}, {1}, {2}, {3}, {4}\r\n",
                    this.GetRelativeVelocity().Length(),
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10,
                    this.GetDownrangeDistance(new DVector2(-99015072098.6158, -109869554508.308)) / 100);
                File.AppendAllText(filename, contents);
            }
        }
    }
}

