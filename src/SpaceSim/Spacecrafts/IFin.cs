using System.Drawing;
using SpaceSim.Drawing;
using SpaceSim.Physics;

namespace SpaceSim.Spacecrafts
{
    interface IFin
    {
        double Dihedral { get; }
        void SetDihedral(double targetAngle);
    }
}
