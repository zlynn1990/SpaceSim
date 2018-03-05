using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;

namespace SpaceSim.Engines
{
    interface IEngine
    {
        bool IsActive { get; }

        double Throttle { get; }
        double Cant { get; }

        void Startup();
        void Shutdown();

        void AdjustThrottle(double targetThrottle);
        void AdjustCant(double targetAngle);

        double Thrust(double ispMultiplier);
        double MassFlowRate(double ispMultiplier);

        IEngine Clone();

        void Update(TimeStep timeStep, double ispMultiplier);
        void Draw(Graphics graphics, Camera camera);
    }
}