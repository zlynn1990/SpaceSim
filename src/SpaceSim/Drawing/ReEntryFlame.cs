using System;
using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Drawing
{
    class ReEntryFlame
    {
        private Particle[] _particles;
        private Queue<int> _availableParticles;

        private Random _random;
        private double _particleRate;

        private double _offsetAngle;
        private double _offsetLength;

        public ReEntryFlame(int maxParticles, double particleRate, DVector2 offset)
        {
            _random = new Random();

            _particles = new Particle[maxParticles];

            _availableParticles = new Queue<int>(maxParticles);

            for (int i = 0; i < maxParticles; i++)
            {
                _particles[i] = new Particle();

                _availableParticles.Enqueue(i);
            }

            _particleRate = particleRate;

            _offsetAngle = offset.Angle();
            _offsetLength = offset.Length();
        }

        public void Update(TimeStep timeStep, DVector2 shipPosition, DVector2 shipVelocity, double pitch, double heatingRate)
        {
            double rotation = pitch - _offsetAngle;

            DVector2 offset = new DVector2(Math.Cos(rotation), Math.Sin(rotation)) * _offsetLength;

            DVector2 shockPosition = shipPosition - offset;

            double normalizedHeating = Math.Max((heatingRate - 500000) * 0.0005, 0);

            int particles = (int)(normalizedHeating * _particleRate) / timeStep.UpdateLoops;

            // Add new particles if nessecary
            for (int i = 0; i < particles; i++)
            {
                if (_availableParticles.Count > 0)
                {
                    double velocityFactor = _random.Next(50, 200);
                    double spread = _random.NextDouble() * 2.5 - 1.25;

                    DVector2 velocity = DVector2.FromAngle(rotation + spread);

                    int id = _availableParticles.Dequeue();

                    Particle particle = _particles[id];

                    particle.IsActive = true;
                    particle.Age = 0;
                    particle.MaxAge = _random.NextDouble()*0.01 + 0.02;

                    particle.Position = shockPosition.Clone();
                    particle.Velocity = shipVelocity.Clone() + velocity*velocityFactor;
                }
            }

            // Update the particles
            for (int i = 0; i < _particles.Length; i++)
            {
                Particle particle = _particles[i];

                if (particle.IsActive)
                {
                    particle.Position += particle.Velocity*timeStep.Dt;
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

            foreach (Particle particle in _particles)
            {
                if (particle.IsActive)
                {
                    if (cameraBounds.Contains(particle.Position))
                    {
                        PointF localPoint = RenderUtils.WorldToScreen(particle.Position, cameraBounds);

                        particleBounds.Add(new RectangleF(localPoint.X - 1.5f, localPoint.Y - 1.5f, 3, 3));
                    }
                }
            }

            RenderUtils.DrawRectangles(graphics, particleBounds, Color.FromArgb(50, 255, 255, 0));
        }
    }
}