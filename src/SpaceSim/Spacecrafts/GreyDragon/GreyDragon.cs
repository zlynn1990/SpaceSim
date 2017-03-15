using VectorMath;

namespace SpaceSim.Spacecrafts.GreyDragon
{
    class GreyDragon : DragonV2.DragonV2
    {
        public override string CraftName { get { return "Grey Dragon"; } }

        public GreyDragon(string craftDirectory, DVector2 position, DVector2 velocity, double payloadMass, double propellantMass)
            : base(craftDirectory, position, velocity, payloadMass, propellantMass)
        {
        }
    }
}
