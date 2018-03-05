using System.Collections.Generic;
using System.Drawing;
using SpaceSim.Drawing;
using VectorMath;

namespace SpaceSim.Particles
{
    abstract class ParticleSystem
    {
        protected Particle[] _particles;
        protected Queue<int> _availableParticles;

        private Color _color;

        protected ParticleSystem(int particleCount, Color color)
        {
            _particles = new Particle[particleCount];

            _availableParticles = new Queue<int>(particleCount);

            for (int i = 0; i < particleCount; i++)
            {
                _particles[i] = new Particle();

                _availableParticles.Enqueue(i);
            }

            _color = color;
        }

        public void Draw(Graphics graphics, RectangleD cameraBounds)
        {
            var particleBounds = new List<RectangleF>();

            float particleScale;

            // Scale particle size with viewport width
            if (cameraBounds.Width > 1000)
            {
                particleScale = 1.5f;
            }
            else
            {
                particleScale = (float)(1.22e-6 * cameraBounds.Width * cameraBounds.Width - 4.8e-3 * cameraBounds.Width + 5.5);
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

            RenderUtils.DrawRectangles(graphics, particleBounds, _color);
        }
    }
}
