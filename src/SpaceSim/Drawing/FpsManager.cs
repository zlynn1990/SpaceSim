using System.Diagnostics;
using System.Linq;

namespace SpaceSim.Drawing
{
    /// <summary>
    /// Keeps the game loop running at a specified FPS by spin waits.
    /// </summary>
    class FpsManager
    {
        public double TargetDt { get; private set; }

        public int CurrentFps
        {
            get
            {
                double avgFrameTicks = _frameSamples.Average();

                double avgFrameTime = avgFrameTicks / Stopwatch.Frequency;

                return (int)(1.0f / avgFrameTime);
            }
        }

        private Stopwatch _updateTimer;

        private int _sampleIndex;
        private long[] _frameSamples;

        private long _targetFrameTicks;

        public FpsManager(int targetFps)
        {
            TargetDt = (1.0 / targetFps);

            _updateTimer = new Stopwatch();

            _frameSamples = new long[30];

            _targetFrameTicks = (long)(TargetDt * Stopwatch.Frequency);
        }

        public void StartFrame()
        {
            _updateTimer.Reset();
            _updateTimer.Start();
        }

        public void FinishFrame()
        {
            _updateTimer.Stop();

            long elapsedTicks = _updateTimer.ElapsedTicks;

            // Busy wait until enough time has elasped for this frame
            if (elapsedTicks < _targetFrameTicks)
            {
                long sleepTicks = _targetFrameTicks - elapsedTicks;

                SpinWait(sleepTicks);

                _frameSamples[_sampleIndex++] = _targetFrameTicks;
            }
            else
            {
                _frameSamples[_sampleIndex++] = elapsedTicks;
            }

            // Wrap the buffer
            if (_sampleIndex == 30)
            {
                _sampleIndex = 0;
            }
        }

        private void SpinWait(long ticks)
        {
            var stopWatch = Stopwatch.StartNew();

            // Busy wait for exact amount of ticks
            while (stopWatch.ElapsedTicks < ticks)
            {
            }
        }
    }
}
