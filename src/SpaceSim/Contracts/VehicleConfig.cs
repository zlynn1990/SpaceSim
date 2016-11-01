using System;

namespace SpaceSim.Contracts
{
    [Serializable]
    public class VehicleConfig
    {
        public string VehicleType { get; set; }

        public double PayloadMass { get; set; }
    }
}
