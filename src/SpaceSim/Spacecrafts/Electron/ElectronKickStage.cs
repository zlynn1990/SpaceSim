using System;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using SpaceSim.Common;
using System.Drawing;
using SpaceSim.Drawing;

using SpaceSim.Properties;
using System.IO;

namespace SpaceSim.Spacecrafts.Electron
{
    class ElectronKickStage : SpaceCraftBase
    {
        public override string CraftName { get { return "Electron Kick Stage"; } }
        public override string CommandFileName { get { return "ElectronKickStage.xml"; } }

        public override double DryMass {
            get
            {
                if (!_deployedFairings)
                {
                    return 25 + _leftFairing.DryMass + _rightFairing.DryMass;
                }

                return 25;
            }
        }

        public override double Width { get { return 1.2; } }
        public override double Height { get { return 0.5; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

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

        public override double StagingForce { get { return 10; } }

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

        public override Color IconColor
        {
            get
            {
                return Color.White;
            }
        }

        private bool _deployedBatteries = false;
        private bool _deployedFairings = false;
        private ElectronBattery _leftBattery;
        private ElectronBattery _rightBattery;
        private ElectronFairing _leftFairing;
        private ElectronFairing _rightFairing;
        DateTime timestamp = DateTime.Now;

        public ElectronKickStage(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass = 50, double propellantMass = 100)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, "Electron/ElectronS2.png")
        {
            StageOffset = new DVector2(0, 0);

            Engines = new IEngine[]
            {
                new Curie(this, new DVector2(0, Height * 0.3)), 
            };
        }

        public void AttachBatteries(ElectronBattery leftBattery, ElectronBattery rightBattery)
        {
            _leftBattery = leftBattery;
            _rightBattery = rightBattery;

            _leftBattery.SetParent(this);
            _rightBattery.SetParent(this);
        }

        public void AttachFairings(ElectronFairing leftFairing, ElectronFairing rightFairing)
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

            if (!_deployedBatteries)
            {
                _leftBattery.UpdateChildren(Position, Velocity);
                _rightBattery.UpdateChildren(Position, Velocity);

                _leftBattery.SetPitch(Pitch);
                _rightBattery.SetPitch(Pitch);
            }

            if (!_deployedFairings)
            {
                _leftFairing.UpdateChildren(Position, Velocity);
                _rightFairing.UpdateChildren(Position, Velocity);

                _leftFairing.SetPitch(Pitch);
                _rightFairing.SetPitch(Pitch);
            }
        }

        public override void DeployBattery()
        {
            _deployedBatteries = true;

            _leftBattery.Stage();
            _rightBattery.Stage();
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

            _leftFairing.RenderGdi(graphics, camera);
            _rightFairing.RenderGdi(graphics, camera);

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
                    //this.GetRelativeAltitude() / 100,
                    this.GetRelativeAltitude() / 1000,
                    this.Throttle * 10);
                File.AppendAllText(filename, contents);
            }
        }
    }
}
