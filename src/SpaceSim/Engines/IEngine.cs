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

        void Update(TimeStep timeStep);
        void Draw(Graphics graphics, RectangleD cameraBounds);
    }
}
