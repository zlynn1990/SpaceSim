using VectorMath;

namespace SpaceSim.Spacecrafts.RedDragon
{
    class RedDragon : DragonV2.DragonV2
    {
        public override string CraftName { get { return "RedDragon"; } }

        public RedDragon(string craftDirectory, DVector2 position, DVector2 velocity, double dryMass, double propellantMass)
            : base(craftDirectory, position, velocity, dryMass, propellantMass)
        {
        }
    }
}
