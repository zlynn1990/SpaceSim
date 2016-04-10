using System;
using System.Drawing;
using SpaceSim.Engines;
using VectorMath;

namespace SpaceSim.Spacecrafts.DragonV2
{
    sealed class DragonV2 : SpaceCraftBase
    {
        public override double Width { get { return 3.7; } }
        public override double Height { get { return 4.15; } }

        // 4200 dry + 2015 payload
        //public override double DryMass { get { return 6215; } }
        public override double DryMass { get { return 2500; } }

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

        public DragonV2(DVector2 position, DVector2 velocity)
            : base(position, velocity, 1388, "Textures/dragonV2.png")
        {
            Engines = new IEngine[8];

            for (int i = 0; i < 8; i++)
            {
                Engines[i] = new SuperDraco(i, this, DVector2.Zero);
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

        public override string CommandFileName { get { return "dragon.xml"; } }

        public override string ToString()
        {
            return "DragonV2";
        }
    }
}
