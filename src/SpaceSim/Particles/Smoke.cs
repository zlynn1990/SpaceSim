using System;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Particles
{
    class Smoke : ParticleSystem
    {
        private Random _random;

        public Smoke(int particleCount, Color color)
            : base(particleCount, color)
        {
            _random = new Random();
        }

        public void Update(TimeStep timeStep, DVector2 enginePosition, DVector2 shipVelocity, DVector2 retrogradeVelocity, double density, double sootRatio)
        {
            int particles = (int)((sootRatio * shipVelocity.Length() * 0.001) / timeStep.UpdateLoops);

            if (density < 0.2 || density > 0.7)
            {
                particles = 0;
            }

            // Add new particles if nessecary
            for (int i = 0; i < particles; i++)
            {
                if (_availableParticles.Count > 0)
                {
                    var randomUnitVector = new DVector2(_random.NextDouble(), _random.NextDouble());

                    DVector2 velocity = shipVelocity.Clone();

                    int id = _availableParticles.Dequeue();

                    Particle particle = _particles[id];

                    particle.IsActive = true;
                    particle.Age = 0;
                    particle.MaxAge = _random.NextDouble() + 1;

                    particle.Position = enginePosition.Clone() + randomUnitVector * 2;
                    particle.Velocity = velocity + retrogradeVelocity * 0.2 + randomUnitVector * 2;
                }
            }

            // Update the particles
            for (int i = 0; i < _particles.Length; i++)
            {
                Particle particle = _particles[i];

                if (particle.IsActive)
                {
                    particle.Position += particle.Velocity * timeStep.Dt;
                    particle.Age += timeStep.Dt;

                    if (particle.Age > particle.MaxAge)
                    {
                        particle.IsActive = false;
                        _availableParticles.Enqueue(i);
                    }
                }
            }
        }
    }
}
