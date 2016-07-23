using System;
using System.Drawing;
using SpaceSim.Engines;
using SpaceSim.Drawing;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    sealed class DragonV2 : SpaceCraftBase
    {
        public override string CraftName { get { return "DragonV2"; } }

        public override double Width { get { return 3.7; } }
        public override double Height { get { return 4.15; } }

        public override double DryMass { get { return _dryMass; } }

        public override bool ExposedToAirFlow { get { return true; } }

        public override double DragCoefficient
        {
            get
            {
                if (Children.Count > 0)
                {
                    return 0.1;
                }

                return 0.5 + _parachuteRatio * 0.2;
            }
        }

        // Base dome = 2 * pi * 1.85^2
        // Parachute size = 2 * pi * 20^2
        public override double CrossSectionalArea
        {
            get { return 21.504 + _parachuteRatio * 2500; }
        }

        public override string CommandFileName { get { return "dragon.xml"; } }

        public override Color IconColor { get { return Color.White; } }

        private double _dryMass;
        private bool _drogueDeployed;
        private bool _parachuteDeployed;
        private double _parachuteRatio;

        public DragonV2(string craftDirectory, DVector2 position, DVector2 velocity, double dryMass, double propellantMass)
            : base(craftDirectory, position, velocity, propellantMass, "Textures/dragonV2.png", new ReEntryFlame(1000, 1, new DVector2(2.5, 0)))
        {
            _dryMass = dryMass;

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

        /// <summary>
        /// Renders the space craft at it's correct scale and rotation according to the camera.
        /// The engines are rendered first and then the space craft body.
        /// </summary>
        public override void RenderGdi(Graphics graphics, RectangleD cameraBounds)
        {
            RectangleF screenBounds = RenderUtils.ComputeBoundingBox(Position, cameraBounds, Width, Height);

            // Saftey
            if (screenBounds.Width > RenderUtils.ScreenWidth * 500) return;

            RenderShip(graphics, cameraBounds, screenBounds);
            RenderAnimations(graphics, cameraBounds);
        }
    }
}
