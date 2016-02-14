using System;

namespace SpaceSim.Contracts
{
    [Serializable]
    public class Payload
    {
        public double DryMass { get; set; }

        public double PropellantMass { get; set; }
    }
}
