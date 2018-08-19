using System;

namespace SpaceSim.Common.Contracts.Commands
{
   [Serializable]
   public class RelativePitch : Command
   {
      public double TargetOrientation { get; set; }
   }
}
