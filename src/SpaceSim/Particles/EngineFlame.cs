using System;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Particles
{
    class EngineFlame : ParticleSystem
    {
        private Random _random;

        private double _particleRate;
        private double _minSpread;
        private double _maxSpread;
        private double _maxAge;
        private double _angle;

        public EngineFlame(int seed, Color color, int maxParticles, double particleRate,
                           double minSpread, double maxSpread, double maxAge, double angle = 0)
            :base(maxParticles, color)
        {
            _random = new Random(seed);

            _particleRate = particleRate;
            _minSpread = minSpread;
            _maxSpread = maxSpread;
            _maxAge = maxAge;
            _angle = angle;
        }

        public void Update(TimeStep timeStep, DVector2 enginePosition, DVector2 shipVelocity,
                           double rotation, double throttle, double ispMultiplier)
        {
            double retrograde = rotation + Math.PI + _angle;

            int particles = (int)((throttle * _particleRate)  / timeStep.UpdateLoops);

            // Interpolate between spreads based on ISP
            double spreadMultiplier = (1.0 - ispMultiplier) * _minSpread + ispMultiplier * _maxSpread;

            double throttleMultiplier = Math.Max(throttle, 0.3) * 0.01;

            // Add new particles if nessecary
            for (int i = 0; i < particles; i++)
            {
                if (_availableParticles.Count > 0)
                {
                    double velocityFactor = _random.Next(200, 300) * throttleMultiplier;
                    double spread = _random.NextDouble() - 0.5;

                    DVector2 velocity = DVector2.FromAngle(retrograde + spread * spreadMultiplier);

                    int id = _availableParticles.Dequeue();

                    Particle particle = _particles[id];

                    particle.IsActive = true;
                    particle.Age = 0;
                    particle.MaxAge = _random.NextDouble() * 0.05 + _maxAge;

                    particle.Position = enginePosition.Clone();
                    particle.Velocity = shipVelocity.Clone() + velocity * velocityFactor;
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
