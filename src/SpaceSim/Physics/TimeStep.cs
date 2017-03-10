using System.Collections.Generic;

namespace SpaceSim.Physics
{
    class TimeStep
    {
        public double Multiplier { get; set; }

        public int UpdateLoops { get; set; }

        public double Dt { get; set; }

        public static int RealTimeIndex
        {
            get { return 4; }
        }

        public static List<TimeStep> Defaults()
        {
            return new List<TimeStep>
            {
                new TimeStep {Multiplier = 0.01, UpdateLoops = 1, Dt = 0.000166666667},
                new TimeStep {Multiplier = 0.1, UpdateLoops = 1, Dt = 0.001666667},
                new TimeStep {Multiplier = 0.25, UpdateLoops = 2, Dt = 0.00208333333333333333333333333333},
                new TimeStep {Multiplier = 0.5, UpdateLoops = 2, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 1, UpdateLoops = 4, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 2, UpdateLoops = 8, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 4, UpdateLoops = 16, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 8, UpdateLoops = 32, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 16, UpdateLoops = 64, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 32, UpdateLoops = 128, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 100, UpdateLoops = 256, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 500, UpdateLoops = 100, Dt = 0.0833333333},
                new TimeStep {Multiplier = 1000, UpdateLoops = 200, Dt = 0.0833333333},
                new TimeStep {Multiplier = 2000, UpdateLoops = 200, Dt = 0.1666666666},
                new TimeStep {Multiplier = 5000, UpdateLoops = 500, Dt = 0.1666666666},
                new TimeStep {Multiplier = 10000, UpdateLoops = 1000, Dt = 0.1666666666},
                new TimeStep {Multiplier = 50000, UpdateLoops = 1000, Dt = 0.8333333333333},
                new TimeStep {Multiplier = 100000, UpdateLoops = 2000, Dt = 0.8333333333333},
                new TimeStep {Multiplier = 1000000, UpdateLoops = 2000, Dt = 8.333333333333},
                new TimeStep {Multiplier = 5000000, UpdateLoops = 2000, Dt = 41.66666666666},
                new TimeStep {Multiplier = 20000000, UpdateLoops = 2000, Dt = 166.66666666664},
            };
        }
    }
}
