using System;
using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Drawing
{
    class EngineFlame
    {
        private Particle[] _particles;

        private Queue<int> _availableParticles;

        private Random _random;

        private double _particleRate;
        private double _minSpread;
        private double _maxSpread;
        private double _maxAge;
        private double _angle;

        public EngineFlame(int seed, int maxParticles, double particleRate,
                           double minSpread, double maxSpread, double maxAge, double angle = 0)
        {
            _random = new Random(seed);

            _particles = new Particle[maxParticles];

            _availableParticles = new Queue<int>(maxParticles);

            for (int i =0; i < maxParticles; i++)
            {
                _particles[i] = new Particle();

                _availableParticles.Enqueue(i);
            }

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

            // Add new particles if nessecary
            for (int i = 0; i < particles; i++)
            {
                if (_availableParticles.Count > 0)
                {
                    double velocityFactor = _random.Next(150, 250);
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

        public void Draw(Graphics graphics, RectangleD cameraBounds)
        {
            var particleBounds = new List<RectangleF>();

            float particleScale;

            // Scale particle size with viewport width
            if (cameraBounds.Width > 1000)
            {
                particleScale = 1;
            }
            else
            {
                particleScale = (float)(1.22e-6 * cameraBounds.Width * cameraBounds.Width - 4.8e-3 * cameraBounds.Width + 4.4);
            }

            float halfParticleScale = particleScale * 0.5f;

            foreach (Particle particle in _particles)
            {
                if (particle.IsActive)
                {
                    if (cameraBounds.Contains(particle.Position))
                    {
                        PointF localPoint = RenderUtils.WorldToScreen(particle.Position, cameraBounds);

                        particleBounds.Add(new RectangleF(localPoint.X - halfParticleScale,
                                                          localPoint.Y - halfParticleScale,
                                                          particleScale, particleScale));
                    }
                }
            }

            RenderUtils.DrawRectangles(graphics, particleBounds, Color.FromArgb(200, 255, 255, 0));
        }
    }
}
