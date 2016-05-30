using System;
using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV1
{
    sealed class Dragon : SpaceCraftBase
    {
        public override double Width { get { return 3.7; } }
        public override double Height { get { return 4.194; } }

        // Dragon + pressuredized cargo (1723) + unpressurized (1413)
        public override double DryMass { get { return 7336; } }

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

        public override Color IconColor { get { return Color.White; } }

        private bool _drogueDeployed;
        private bool _parachuteDeployed;
        private double _parachuteRatio;

        public Dragon(DVector2 position, DVector2 velocity)
            : base(position, velocity, 1388, "Textures/dragon.png", new ReEntryFlame(1000, 1, new DVector2(2.5, 0)))
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

        public override string CommandFileName { get { return "dragon.xml"; } }

        public override string ToString()
        {
            return "Dragon";
        }
    }
}
