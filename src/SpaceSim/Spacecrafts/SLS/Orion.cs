﻿using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Physics;
using VectorMath;
using System.IO;
using SpaceSim.Common;
using SpaceSim.Drawing;
using SpaceSim.Properties;
using SpaceSim.Particles;

using SpaceSim.Spacecrafts.FalconCommon;

namespace SpaceSim.Spacecrafts.DragonV2
{
    class Orion : SpaceCraftBase
    {
        public override string CraftName { get { return "Orion"; } }
        public override string CommandFileName { get { return "Orion.xml"; } }

        public override double Width { get { return 5.02; } }
        public override double Height { get { return 3.3; } }

        public override double DryMass { get { return 8500; } }

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

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.9);
                }

                baseCd *= Math.Cos(alpha);
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
            //get { return 21.504 + _parachuteRatio * 15000; }
            get { return 21.504 + _parachuteRatio * 25000; }
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
        DrogueChute _drogueChute;
        Parachute _parachute;
        LAS _las;
        private bool _lasDeployed;
        private DateTime timestamp = DateTime.Now;

        public Orion(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, double propellantMass = 175)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass, "SLS/Orion.png")
        {
            _drogueChute = new DrogueChute(this, new DVector2(7.0, -8.5));
            _parachute = new Parachute(this, new DVector2(-10.0, -36.0));

            Engines = new IEngine[]{};
        }

        public override void Release()
        {
            if(_las != null)
                _las.Release();

            base.Release();
        }

        public void AttachLAS(LAS las)
        {
            _las = las;
            _las.SetParent(this);
        }

        public override void DeployLAS()
        {
            if (_las != null)
            {
                _lasDeployed = true;
                _las.Stage();
            }
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

        public override void DeployDrogues()
        {
            _drogueChute.Deploy();
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
            _parachute.Deploy();
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

            _drogueChute.Update(dt);
            _parachute.Update(dt);

            if (_las != null && !_lasDeployed)
            {
                _las.UpdateChildren(Position, Velocity);
                _las.SetPitch(Pitch);
            }
        }

        protected override void RenderShip(Graphics graphics, Camera camera, RectangleF screenBounds)
        {
            double drawingRotation = Pitch + Math.PI * 0.5;

            var offset = new PointF(screenBounds.X + screenBounds.Width * 0.5f,
                                    screenBounds.Y + screenBounds.Height * 0.5f);

            camera.ApplyScreenRotation(graphics);
            camera.ApplyRotationMatrix(graphics, offset, drawingRotation);

            // Normalize the angle to [0,360]
            int rollAngle = (int)(Roll * MathHelper.RadiansToDegrees) % 360;

            int heatingRate = Math.Min((int)this.HeatingRate, 600000);
            if (heatingRate > 100000)
            {
                Random rnd = new Random();
                float noise = (float)rnd.NextDouble();
                float width = screenBounds.Width / (3 + noise);
                float height = screenBounds.Height / (18 + noise);
                RectangleF plasmaRect = screenBounds;
                plasmaRect.Inflate(new SizeF(width, height));

                if (Roll != 0)
                {
                    float foreshortening = (float)Math.Pow(Math.Cos(Roll), 0.4);
                    plasmaRect.Y += plasmaRect.Height * (1 - foreshortening);
                    plasmaRect.Height *= foreshortening;
                }

                int alpha = 255;
                int blue = Math.Min(heatingRate / 2000, 255);
                int green = 0;
                int red = Math.Max(blue - 64, 0);
                Color glow = Color.FromArgb(alpha, red, green, blue);

                float penWidth = width / 12;
                Pen glowPen = new Pen(glow, penWidth);
                glowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                glowPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                graphics.DrawArc(glowPen, plasmaRect, 220, 100);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.75), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 200, 140);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.5), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 180, 180);

                glowPen.Color = Color.FromArgb((int)(alpha * 0.25), glow);
                plasmaRect.Inflate(-penWidth, -penWidth);
                graphics.DrawArc(glowPen, plasmaRect, 160, 220);
            }

            graphics.DrawImage(this.Texture, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);

            // Index into the sprite
            //int ships = _spriteSheet.Cols * _spriteSheet.Rows;
            //int spriteIndex = (rollAngle * ships) / 360;
            //while (spriteIndex < 0)
            //    spriteIndex += ships;

            //_spriteSheet.Draw(spriteIndex, graphics, screenBounds);

            graphics.ResetTransform();

            if (_parachute.IsDeploying() || _parachute.IsDeployed())
            {
                _parachute.RenderGdi(graphics, camera);
            }
            else
            {
                if (_drogueChute.IsDeploying() || _drogueChute.IsDeployed())
                    _drogueChute.RenderGdi(graphics, camera);
            }

            if(_las != null)
                _las.RenderGdi(graphics, camera);

            if (Settings.Default.WriteCsv && (DateTime.Now - timestamp > TimeSpan.FromSeconds(1)))
            {
                string filename = MissionName + ".csv";

                if (!File.Exists(filename))
                {
                    File.AppendAllText(filename, "Velocity, Acceleration, Altitude, Throttle, Pressure, Heating\r\n");
                }

                timestamp = DateTime.Now;

                double targetVelocity = this.GetRelativeVelocity().Length();
                double density = this.GravitationalParent.GetAtmosphericDensity(this.GetRelativeAltitude());
                double dynamicPressure = 0.5 * density * targetVelocity * targetVelocity;

                string contents = string.Format("{0}, {1}, {2}, {3}, {4}, {5}\r\n",
                    targetVelocity,
                    this.GetRelativeAcceleration().Length() * 100,
                    this.GetRelativeAltitude() / 100,
                    this.Throttle * 10,
                    dynamicPressure / 10,
                    this.HeatingRate / 10);

                File.AppendAllText(filename, contents);
            }
        }

    }
}
