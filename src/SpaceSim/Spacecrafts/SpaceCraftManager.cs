using System;
using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Controllers;
using SpaceSim.Drawing;
using SpaceSim.Physics;
using SpaceSim.SolarSystem;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    class SpaceCraftManager
    {
        public ISpaceCraft First { get { return _spaceCrafts[0]; } }

        private List<ISpaceCraft> _spaceCrafts;
        private List<IGravitationalBody> _gravitationalBodies;

        public SpaceCraftManager(List<IGravitationalBody> gravitationalBodies)
        {
            _spaceCrafts = new List<ISpaceCraft>();
            _gravitationalBodies = gravitationalBodies;
        }

        public void Add(List<ISpaceCraft> crafts)
        {
            _spaceCrafts.AddRange(crafts);
            _gravitationalBodies.AddRange(crafts);
        }

        public void Initialize(EventManager eventManager, double clockDelay)
        {
            foreach (ISpaceCraft craft in _spaceCrafts)
            {
                craft.Initialize(this, eventManager, clockDelay);
            }
        }

        public void ToggleDisplayVectors()
        {
            foreach (ISpaceCraft craft in _spaceCrafts)
            {
                craft.ToggleDisplayVectors();
            }
        }

        public double GetNextBurnTime()
        {
            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                var controller = spaceCraft.Controller as CommandController;

                // Spacecraft uses command controller
                if (controller != null)
                {
                    double nextBurn = controller.NextBurnTime();

                    if (nextBurn > 0)
                    {
                        return nextBurn;
                    }
                }
            }

            return 0;
        }

        public void ResolveGravitionalParents(List<IMassiveBody> massiveBodies)
        {
            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                double bestMassDistanceRatio = double.MaxValue;

                foreach (IMassiveBody bodyB in massiveBodies)
                {
                    double massRatio = Math.Pow(spaceCraft.Mass / bodyB.Mass, 0.4);

                    DVector2 difference = spaceCraft.Position - bodyB.Position;

                    double massDistanceRatio = massRatio * difference.Length();

                    // New parent
                    if (massDistanceRatio < bestMassDistanceRatio)
                    {
                        spaceCraft.SetGravitationalParent(bodyB);

                        bestMassDistanceRatio = massDistanceRatio;
                    }
                }
            }
        }

        public void ResolveForces(List<IMassiveBody> massiveBodies)
        {
            foreach (SpaceCraftBase spaceCraft in _spaceCrafts)
            {
                spaceCraft.ResetAccelerations();

                foreach (IMassiveBody massiveBody in massiveBodies)
                {
                    spaceCraft.ResolveGravitation(massiveBody);

                    if (spaceCraft.GravitationalParent == massiveBody && !spaceCraft.Terminated)
                    {
                        spaceCraft.ResolveAtmopsherics(massiveBody);
                    }
                }
            }
        }

        public void Update(TimeStep timeStep, double targetDt)
        {
            // Don't update the animations every frame
            if (timeStep.UpdateLoops < 16)
            {
                foreach (ISpaceCraft spaceCraft in _spaceCrafts)
                {
                    if (!spaceCraft.Terminated)
                    {
                        spaceCraft.UpdateAnimations(timeStep);
                    }
                }
            }

            foreach (ISpaceCraft spaceCraft in _spaceCrafts)
            {
                if (!spaceCraft.Terminated)
                {
                    spaceCraft.UpdateController(targetDt);
                }
            }
        }

        public void Render(Graphics graphics, Camera camera)
        {
            // Draw spacecraft
            foreach (SpaceCraftBase spaceCraft in _spaceCrafts)
            {
                spaceCraft.RenderGdi(graphics, camera);
            }
        }
    }
}
