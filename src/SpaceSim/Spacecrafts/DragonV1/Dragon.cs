using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Particles;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV1
{
    sealed class Dragon : SpaceCraftBase
    {
        public override string CraftName { get { return "Dragon"; } }
        public override string CommandFileName { get { return "dragon.xml"; } }

        public override double Width { get { return 3.7; } }
        public override double Height { get { return 4.194; } }

        public override double DryMass { get { return 4200; } }

        public override AeroDynamicProperties GetAeroDynamicProperties { get { return AeroDynamicProperties.ExposedToAirFlow; } }

        public override double FormDragCoefficient
        {
            get
            {
                double alpha = GetAlpha();
                double baseCd = GetBaseCd(0.45);

                if (alpha > Constants.PiOverTwo || alpha < -Constants.PiOverTwo)
                {
                    baseCd = GetBaseCd(0.8);
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

                return alphaCd;
            }
        }

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

        public Dragon(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass)
            : base(craftDirectory, position, velocity, payloadMass, 1290, "Textures/dragon.png", new ReEntryFlame(1000, 1, new DVector2(2.5, 0)))
        {
            Engines = new IEngine[0];
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

        public void DeployParachutes()
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
                _parachuteRatio = Math.Min(_parachuteRatio + dt * 0.03, 0.3);
            }
            else if (_parachuteDeployed)
            {
                _parachuteRatio = Math.Min(_parachuteRatio + dt * 0.03, 1);
            }

            base.Update(dt);
        }
    }
}
