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

        public static int MaxRealTimeIndex
        {
            get { return 10; }
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
                new TimeStep {Multiplier = 100, UpdateLoops = 400, Dt = 0.00416666666666666666666666666667},
                new TimeStep {Multiplier = 500, UpdateLoops = 400, Dt = 0.02083333333333333333333333333335},
                new TimeStep {Multiplier = 1000, UpdateLoops = 400, Dt = 0.0416666666666666666666666666667},
                new TimeStep {Multiplier = 5000, UpdateLoops = 400, Dt = 0.2083333333333333333333333333335},
                new TimeStep {Multiplier = 20000, UpdateLoops = 400, Dt = 0.833333333333333333333333333334},
                new TimeStep {Multiplier = 100000, UpdateLoops = 400, Dt = 4.16666666666666666666666666667},
                new TimeStep {Multiplier = 500000, UpdateLoops = 400, Dt = 20.83333333333333333333333333335},
                new TimeStep {Multiplier = 2000000, UpdateLoops = 400, Dt = 83.3333333333333333333333333334},
                new TimeStep {Multiplier = 10000000, UpdateLoops = 400, Dt = 416.666666666666666666666666667},
                new TimeStep {Multiplier = 50000000, UpdateLoops = 400, Dt = 2083.333333333333333333333333335},
                new TimeStep {Multiplier = 200000000, UpdateLoops = 400, Dt = 8333.33333333333333333333333334},
            };
        }
    }
}
