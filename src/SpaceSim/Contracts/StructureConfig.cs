using System;

namespace SpaceSim.Contracts
{
    [Serializable]
    public class StructureConfig
    {
        public string Type { get; set; }

        public double DownrangeDistance { get; set; }

        public double HeightOffset { get; set; }
    }
}
