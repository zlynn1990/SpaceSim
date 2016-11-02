using System;
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
        // Update rate of the controller in Hz
        private const int UpdateRate = 10;

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
                // Start at 55% thrust
                _currentThrust = 55;

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
                double t = elapsedTime - _lastUpdate;

                // Update required this iteration
                if (t > 1.0 / UpdateRate)
                {
                    PredictTargetThrottle(spaceCraft);

                    if (spaceCraft.OnGround)
                    {
                        _landed = true;

                        foreach (IEngine engine in spaceCraft.Engines)
                        {
                            engine.Shutdown();
                        }
                    }

                    _lastUpdate = elapsedTime;
                }
            }
        }

        private void PredictTargetThrottle(SpaceCraftBase spaceCraft)
        {
            double optimalThrust = 0;
            DVector2 lowestVelocity = new DVector2(10e6, 10e6);

            for (int i = -1; i <= 1; i++)
            {
                double thrust = _currentThrust + i;

                if (thrust < 0 || thrust > 100)
                {
                    continue;
                }

                IMassiveBody parent = spaceCraft.GravitationalParent;

                DVector2 initialPosition = spaceCraft.Position - parent.Position;

                var shipOffset = new DVector2(Math.Cos(spaceCraft.Pitch)*(spaceCraft.TotalWidth - spaceCraft.Width),
                    Math.Sin(spaceCraft.Pitch)*(spaceCraft.TotalHeight - spaceCraft.Height))*0.5;

                initialPosition -= shipOffset;

                var proxyParent = new MassiveBodyProxy(DVector2.Zero, DVector2.Zero, parent);
                var proxySatellite = new SpaceCraftProxy(initialPosition, spaceCraft.Velocity - parent.Velocity, spaceCraft);

                foreach (int id in _engineIds)
                {
                    proxySatellite.Engines[id].Startup();
                    proxySatellite.Engines[id].AdjustThrottle(thrust);
                }

                proxySatellite.SetGravitationalParent(proxyParent);

                // Simulate until the rocket runs out of fuel or touches down
                for (int step = 0; step < 2000; step++)
                {
                    proxySatellite.ResetAccelerations();
                    proxySatellite.ResolveGravitation(proxyParent);
                    proxySatellite.ResolveAtmopsherics(proxyParent);

                    proxySatellite.Update(0.02);

                    if (proxySatellite.Altitude <= spaceCraft.Height * 0.65)
                    {
                        if (proxySatellite.RelativeVelocity.Length() < lowestVelocity.Length())
                        {
                            lowestVelocity = proxySatellite.RelativeVelocity;
                            optimalThrust = thrust;

                            break;
                        }
                    }
                }
            }

            // Set the target engines to 100% thrust
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
