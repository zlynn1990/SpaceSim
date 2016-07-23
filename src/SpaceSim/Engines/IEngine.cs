using System.Drawing;
using SpaceSim.Physics;
using VectorMath;

namespace SpaceSim.Engines
{
    interface IEngine
    {
        bool IsActive { get; }

        double Throttle { get; }

        void Startup();
        void Shutdown();

        void AdjustThrottle(double targetThrottle);
        double Thrust(double ispMultiplier);
        double MassFlowRate();

        IEngine Clone();

        void Update(TimeStep timeStep, double ispMultiplier);
        void Draw(Graphics graphics, RectangleD cameraBounds);
    }
}
