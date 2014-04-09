namespace PerfCounter
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <code>
    /// var counterResult = PerfCounters.Results["CounterName"];
    /// PerfCounters.Reset("CounterName");
    /// PerfCounters.ResetAll();
    /// </code>
    /// </summary>
    public static class PerfCounters
    {
        private static readonly Dictionary<string, PerfCounterResult> Measurements = new Dictionary<string, PerfCounterResult>();
        private static readonly object Locker = new object();

        public static ReadOnlyDictionary<string, PerfCounterResult> Results
        {
            get
            {
                lock (Locker)
                {
                    var result = Measurements.ToDictionary(x => x.Key, x => x.Value.Clone());
                    return new ReadOnlyDictionary<string, PerfCounterResult>(result);
                }
            }
        }

        public static void ResetAll()
        {
            lock (Locker)
            {
                var keys = new List<string>(Measurements.Keys);

                foreach (var key in keys)
                {
                    Measurements[key] = PerfCounterResult.Empty;
                }
            }
        }

        public static void Reset(string counterName)
        {
            lock (Locker)
            {
                if (Measurements.ContainsKey(counterName))
                {
                    Measurements[counterName] = PerfCounterResult.Empty;
                }
            }
        }

        internal static void Add(string counterName, long elapsedTicks)
        {
            lock (Locker)
            {
                if (Measurements.ContainsKey(counterName))
                {
                    Measurements[counterName].Add(elapsedTicks);
                }
                else
                {
                    Measurements.Add(counterName, new PerfCounterResult(elapsedTicks, 1));
                }
            }
        }
    }
}
