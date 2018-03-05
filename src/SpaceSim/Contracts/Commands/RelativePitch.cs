using System;

namespace SpaceSim.Contracts.Commands
{
   [Serializable]
   public class RelativePitch : Command
   {
      public double TargetOrientation { get; set; }
   }
}
