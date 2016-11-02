using System;

namespace SpaceSim.Contracts
{
    [Serializable]
    public class Payload
    {
        public string CraftType { get; set; }

        public double DryMass { get; set; }

        public double PropellantMass { get; set; }
    }
}
