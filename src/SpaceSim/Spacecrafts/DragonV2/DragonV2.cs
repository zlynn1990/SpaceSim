using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    class DragonV2 : SpaceCraftBase
    {
        public override string CraftName { get { return "DragonV2"; } }
        public override string CommandFileName { get { return "dragon.xml"; } }

        public override double Width { get { return 3.7; } }
        public override double Height { get { return 4.15; } }

        public override double DryMass { get { return 4200; } }

        public override AeroDynamicProperties GetAeroDynamicProperties
        {
            get { return AeroDynamicProperties.ExposedToAirFlow; }
        }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.45);
                bool isRetrograde = false;

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    isRetrograde = true;
                    baseCd = GetBaseCd(0.9);
                }

                baseCd *= Math.Cos(alpha);

                double dragPreservation = 1.0;

                if (isRetrograde)
                {
                    // if retrograde
                    if (MachNumber > 1.5 && MachNumber < 20.0)
                    {
                        double throttleFactor = Throttle / 50;
                        double cantFactor = Math.Sin(Engines[0].Cant * 2);
                        dragPreservation += throttleFactor * cantFactor;
                        baseCd *= dragPreservation;
                    }
                }

                return Math.Abs(baseCd);
            }
        }

        public override double LiftCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.3);

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.6);
                }

                double alphaCd = baseCd * Math.Sin(alpha * 2);
                return -alphaCd;
            }
        }

        // Base dome = 2 * pi * 1.85^2
        // Parachute size = 2 * pi * 20^2
        public override double FrontalArea
        {
            get { return 21.504 + _parachuteRatio * 2500; }
        }

        public override double LiftingSurfaceArea
        {
            get
            {
                double area = Math.PI * Math.Pow(Width / 2, 2);
                double alpha = GetAlpha();

                return Math.Abs(area * Math.Cos(alpha * 2));
            }
        }

        public override double ExposedSurfaceArea
        {
            get
            {
                // A = πr(r + root(h ^ 2 + r ^ 2))
                double r = Width / 2;
                double h2 = Math.Pow(Height, 2);
                double r2 = Math.Pow(r, 2);
                return Math.PI * r * (r + Math.Pow(h2 + r2, 0.5));
            }
        }

        public override Color IconColor { get { return Color.White; } }

        private bool _drogueDeployed;
        private bool _parachuteDeployed;
        private double _parachuteRatio;

        public DragonV2(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, double propellantMass)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, "Dragon/V2/capsule.png", new ReEntryFlame(1000, 1, new DVector2(2.5, 0)))
        {
            Engines = new IEngine[]
            {
                new SuperDraco(0, this, new DVector2(-1.35, 0.1), -0.15),
                new SuperDraco(1, this, new DVector2(-1.35, 0.1), -0.15),
                new SuperDraco(2, this, new DVector2(-1.4, 0.1), -0.25),
                new SuperDraco(3, this, new DVector2(-1.4, 0.1), -0.25),
                new SuperDraco(4, this, new DVector2(1.35, 0.1), 0.15),
                new SuperDraco(5, this, new DVector2(1.35, 0.1), 0.15),
                new SuperDraco(6, this, new DVector2(1.4, 0.1), 0.25),
                new SuperDraco(7, this, new DVector2(1.4, 0.1), 0.25),
            };
        }

        public void Abort()
        {
            if (Children.Count > 0)
            {
                ISpaceCraft[] children = Children.ToArray();

                foreach (ISpaceCraft child in children)
                {
                    child.SetParent(null);

                    Children.Remove(child);
                }
            }

            SetThrottle(100);
        }

        public override void DeployParachutes()
        {
            if (!_drogueDeployed)
            {
                _drogueDeployed = true;
            }
            else if (!_parachuteDeployed)
            {
                _drogueDeployed = false;
                _parachuteDeployed = true;
            }
        }

        public override void Update(double dt)
        {
            if (_drogueDeployed)
            {
                _parachuteRatio = Math.Min(_parachuteRatio + dt * 0.03, 0.15);
            }
            else if (_parachuteDeployed)
            {
                _parachuteRatio = Math.Min(_parachuteRatio + dt * 0.03, 1);
            }

            base.Update(dt);
        }

        protected override void RenderShip(Graphics graphics, RectangleD cameraBounds, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                screenBounds.Y + screenBounds.Height * 0.5f);

            graphics.TranslateTransform(offset.X, offset.Y);

            float pitchAngle = (float)(drawingRotation * 180 / Math.PI);

            graphics.RotateTransform(pitchAngle);
            graphics.TranslateTransform(-offset.X, -offset.Y);

            int heatingRate = Math.Min((int)this.HeatingRate, 2000000);
            if (heatingRate > 100000)
            {
                Random rnd = new Random();
                float noise = (float)rnd.NextDouble();
                float width = screenBounds.Width / (3 + noise);
                float height = screenBounds.Height / (18 + noise);
                RectangleF plasmaRect = screenBounds;
                plasmaRect.Inflate(screenBounds.Width / 2, screenBounds.Height / 4);
                plasmaRect.Inflate(new SizeF(width, height));

                int alpha = Math.Min(heatingRate / 7800, 255);
                int red = alpha;
                int green = Math.Max(red - 128, 0) * 2;
                int blue = 0;
                Color glow = Color.FromArgb(alpha, red, green, blue);

                float penWidth = width / 12;
                Pen glowPen = new Pen(glow, penWidth);
                glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                graphics.DrawArc(glowPen, plasmaRect, 40, 100);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.75), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 20, 140);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.5), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 0, 180);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.25), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, -20, 220);
            }

            graphics.DrawImage(Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            graphics.ResetTransform();
        }
    }
}
