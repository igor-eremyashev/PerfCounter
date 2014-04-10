namespace PerfCounter
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// <code>
    /// using (new PerfCounter("CounterName"))
    /// {
    ///     // Some operations here...
    /// }
    /// var elapsedTicks = PerfCounters.Results["CounterName"].ElapsedTicks;
    /// </code>
    /// </summary>
    public struct PerfCounter : IDisposable
    {
        private readonly string _counterName;
        private readonly Stopwatch _stopwatch;

        /// <summary>
        /// Creates and starts new counter instance.
        /// </summary>
        public PerfCounter(string counterName)
        {
            _counterName = counterName;
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Stops counter and writes result.
        /// </summary>
        public void Dispose()
        {
            if (_stopwatch == null)
            {
                throw new PerfCounterNameNotSpecifiedException();
            }

            _stopwatch.Stop();
            PerfCounters.Add(_counterName, _stopwatch.Elapsed.Ticks);
        }
    }
}