using System;
using System.Windows.Controls.Primitives;
using SpaceSim.Contracts.Commands;
using SpaceSim.Engines;
using SpaceSim.Proxies;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts;
using VectorMath;

namespace SpaceSim.Commands
{
    class AutoLandCommand : CommandBase
    {
        private bool _landed;

        private double _lastUpdate;

        private int[] _engineIds;

        private double _currentThrust;

        public AutoLandCommand(AutoLand autoLand)
            : base(autoLand.StartTime, autoLand.Duration)
        {
            _engineIds = autoLand.EngineIds;
        }

        public override void Initialize(SpaceCraftBase spaceCraft)
        {
            EventManager.AddMessage("Auto-Landing Engaged", spaceCraft);

            if (_engineIds != null)
            {
                // Startup the required landing engines
                foreach (int id in _engineIds)
                {
                    IEngine engine = spaceCraft.Engines[id];

                    engine.Startup();
                }
            }
        }

        public override void Finalize(SpaceCraftBase spaceCraft) { }

        public override void Update(double elapsedTime, SpaceCraftBase spaceCraft)
        {
            // Only run the landing algorithm while the spacecraft hasn't touched down
            if (!_landed)
            {
                double speed = spaceCraft.GetRelativeVelocity().Length();

                if (spaceCraft.OnGround || speed < 2 || spaceCraft.PropellantMass <= 0)
                {
                    _landed = true;

                    foreach (IEngine engine in spaceCraft.Engines)
                    {
                        engine.Shutdown();
                    }
                }

                double t = elapsedTime - _lastUpdate;

                double altitude = spaceCraft.GetRelativeAltitude();

                double updateRate = Math.Max(10 - altitude / 300, 1);

                // Update required this iteration
                if (t > 1.0 / updateRate)
                {
                    PredictTargetThrottle(spaceCraft);

                    _lastUpdate = elapsedTime;
                }
            }
        }

        private void PredictTargetThrottle(SpaceCraftBase spaceCraft)
        {
            double optimalThrust = _currentThrust;
            double optimalLandingSpeed = double.PositiveInfinity;

            for (int i = -1; i <= 1; i++)
            {
                double thrust = 65;

                if (_currentThrust > 0)
                {
                    thrust = _currentThrust;
                }

                thrust += i * 0.5;

                if (thrust < 60 || thrust > 100)
                {
                    continue;
                }

                IMassiveBody parent = spaceCraft.GravitationalParent;

                DVector2 initialPosition = spaceCraft.Position - parent.Position;

                var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
                var proxySatellite = new SpaceCraftProxy(initialPosition, spaceCraft.Velocity - parent.Velocity, spaceCraft);

                foreach (int id in _engineIds)
                {
                    proxySatellite.Engines[id].Startup();
                    proxySatellite.Engines[id].AdjustThrottle(thrust);
                }

                proxySatellite.SetGravitationalParent(proxyParent);

                // Simulate until the rocket runs out of fuel or touches down
                for (int step = 0; step < 1000; step++)
                {
                    proxySatellite.ResetAccelerations();
                    proxySatellite.ResolveGravitation(proxyParent);
                    proxySatellite.ResolveAtmopsherics(proxyParent);

                    if (proxySatellite.PropellantMass <= 0)
                    {
                        break;
                    }

                    if (proxySatellite.OnGround)
                    {
                        double landingSpeed = proxySatellite.RelativeVelocity.Length();

                        // If the rocket is in motion search for the best speed always
                        if (_currentThrust > 0)
                        {
                            if (landingSpeed < optimalLandingSpeed)
                            {
                                optimalLandingSpeed = landingSpeed;
                                optimalThrust = thrust;

                                break;
                            }
                        }
                        // Otherwise enforce the landing speed is very good to start
                        else if (landingSpeed < 5)
                        {
                            optimalLandingSpeed = landingSpeed;
                            optimalThrust = thrust;

                            break;
                        }
                    }

                    proxySatellite.Update(0.05);
                }
            }

            // Set the target engines to the optimal computed thrust
            if (_engineIds != null)
            {
                // Startup the required landing engines
                foreach (int id in _engineIds)
                {
                    IEngine engine = spaceCraft.Engines[id];

                    engine.AdjustThrottle(optimalThrust);
                }

                if (optimalThrust > 0)
                {
                    _currentThrust = optimalThrust;
                }
            }
        }
    }
}
