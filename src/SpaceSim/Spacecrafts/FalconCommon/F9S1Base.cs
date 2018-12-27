using System;
using System.Drawing;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.FalconCommon
{
    abstract class F9S1Base : SpaceCraftBase
    {
        public override Color IconColor { get { return Color.White; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExtendsFineness; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.4);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    if (_landingLegs[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(1.6);
                    }
                    else if (_gridFins[0].Pitch > 0)
                    {
                        baseCd = GetBaseCd(0.9);
                    }
                    else
                    {
                        baseCd = GetBaseCd(0.6);
                    }

                    isRetrograde = true;
                }

                double dragCoefficient = Math.Abs(baseCd * Math.Cos(alpha));
                double dragPreservation = 1.0;

                if (isRetrograde)
                {
                    // if retrograde
                    if (Throttle > 0 && MachNumber > 1.5 && MachNumber < 20.0)
                    {
                        double throttleFactor = Throttle / 50;
                        double cantFactor = Math.Sin(Engines[0].Cant * 2);
                        dragPreservation += throttleFactor * cantFactor;
                        dragCoefficient *= dragPreservation;
                    }
                }

                return Math.Abs(dragCoefficient);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double baseCd = GetBaseCd(0.6);
                double alpha = GetAlpha();

                return baseCd * Math.Sin(alpha * 2);
            }
        }

        public override double LiftingSurfaceArea { get { return Width * Height; } }

        public override double FrontalArea
        {
            get
            {
                double area = Math.PI * Math.Pow(Width / 2, 2);
                double alpha = GetAlpha();

                return Math.Abs(area * Math.Cos(alpha));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = 2πrh + πr2
                return 2 * Math.PI * (Width / 2) * Height + FrontalArea;
            }
        }

        private GridFin[] _gridFins;
        private LandingLeg[] _landingLegs;

        private Smoke _engineSmoke;

	    private SootRenderer _bodyRenderer;

        private double _sootRatio;

        protected F9S1Base(string craftDirectory, DVector2 position, DVector2 velocity, int block, double propellantMass, string texturePath, double finOffset = -17.6)
            : base(craftDirectory, position, velocity, 0, propellantMass, texturePath)
        {
            _gridFins = new[]
            {
                new GridFin(this, new DVector2(1.3, finOffset), block, true),
                new GridFin(this, new DVector2(-1.3, finOffset), block, false)
            };

            _landingLegs = new[]
            {
                new LandingLeg(this, new DVector2(1.0, 21), block, true),
                new LandingLeg(this, new DVector2(-1.0, 21), block, false)
            };

	        _bodyRenderer = new SootRenderer(Texture, Path.Combine("Textures/Spacecrafts", texturePath));

			_engineSmoke = new Smoke(1000, Color.FromArgb(90, 130, 130, 130));
        }

        public override void DeployGridFins()
        {
            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Deploy();
            }
        }

        public override void DeployLandingLegs()
        {
            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Deploy();
            }
        }

        public override void UpdateAnimations(TimeStep timeStep)
        {
            DVector2 retrogradeVelocity = GetRelativeVelocity();
            retrogradeVelocity.Negate();

            DVector2 engineBase = Position - DVector2.FromAngle(Pitch) * Height * 0.5;

            double altitude = GetRelativeAltitude();

            double atmosphericDensity = GravitationalParent.GetAtmosphericDensity(altitude);

            _engineSmoke.Update(timeStep, engineBase, Velocity, retrogradeVelocity, atmosphericDensity, _sootRatio);

            base.UpdateAnimations(timeStep);
        }

        public override void Update(double dt)
        {
            base.Update(dt);

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.Update(dt);
                gridFin.UpdateSootRatio(_sootRatio);
            }

            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.Update(dt);
                landingLeg.UpdateSootRatio(_sootRatio);
            }

            DVector2 velocity = GetRelativeVelocity();
            double altitude = GetRelativeAltitude();

            DVector2 normalizedVelocity = velocity.Clone();
            normalizedVelocity.Normalize();

            DVector2 rotation = new DVector2(Math.Cos(Pitch), Math.Sin(Pitch));

            // If we are going retro-grade and firing rockets adds soot
            if (altitude < 70000 && normalizedVelocity.Dot(rotation) < 0 && velocity.Length() > 400)
            {
                foreach (IEngine engine in Engines)
                {
                    if (engine.IsActive && engine.Throttle > 0)
                    {
                        _sootRatio = Math.Min(_sootRatio + 0.02 * dt, 1.0);
                    }
                }
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
			// Doesn't need to be disposed because it's re-used internally
			Bitmap sootBody = _bodyRenderer.GenerateTexture(_sootRatio);

            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            graphics.DrawImage(sootBody, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();

            foreach (GridFin gridFin in _gridFins)
            {
                gridFin.RenderGdi(graphics, camera);
            }

            foreach (LandingLeg landingLeg in _landingLegs)
            {
                landingLeg.RenderGdi(graphics, camera);
            }
        }

        protected override void RenderAbove(Graphics graphics, Camera camera)
        {
            base.RenderAbove(graphics, camera);

            _engineSmoke.Draw(graphics, camera);
        }
    }
}
