namespace PerfCounter
{
    using System;

    /// <summary>
    /// <code>
    /// var result = PerfCounters.Results["CounterName"];
    /// 
    /// var hitCount = result.HitCount;
    /// 
    /// var elapsedTicks = result.ElapsedTicks;
    /// var elapsedMilliseconds = result.ElapsedMilliseconds;
    /// 
    /// var averageTicks = result.AverageTicks;
    /// var averageMilliseconds = result.AverageMilliseconds;
    /// 
    /// var shortestPeriodTicks = result.ShortestPeriodTicks;
    /// var shortestPeriodMilliseconds = result.ShortestPeriodMilliseconds;
    /// 
    /// var longestPeriodTicks = result.LongestPeriodTicks;
    /// var longestPeriodMilliseconds = result.LongestPeriodMilliseconds;
    /// </code>
    /// </summary>
    public class PerfCounterResult
    {
        internal PerfCounterResult(long elapsedTicks, long hitCount)
        {
            ElapsedTicks = elapsedTicks;
            ShortestPeriodTicks = elapsedTicks;
            LongestPeriodTicks = elapsedTicks;
            HitCount = hitCount;
        }

        internal PerfCounterResult()
            : this(0, 0)
        {
        }

        public long HitCount { get; private set; }

        public long ElapsedTicks { get; private set; }

        public long ElapsedMilliseconds
        {
            get { return ConvertTicksToMilliseconds(ElapsedTicks); }
        }

        public long ShortestPeriodTicks { get; private set; }

        public long ShortestPeriodMilliseconds
        {
            get { return ConvertTicksToMilliseconds(ShortestPeriodTicks); }
        }

        public long LongestPeriodTicks { get; private set; }

        public long LongestPeriodMilliseconds
        {
            get { return ConvertTicksToMilliseconds(LongestPeriodTicks); }
        }

        public long AverageTicks
        {
            get
            {
                if (HitCount == 0) return 0;
                return ElapsedTicks/HitCount;
            }
        }

        public long AverageMilliseconds
        {
            get { return ConvertTicksToMilliseconds(AverageTicks); }
        }

        internal static PerfCounterResult Empty
        {
            get { return new PerfCounterResult(); }
        }

        private static long ConvertTicksToMilliseconds(long ticks)
        {
            return ticks/TimeSpan.TicksPerMillisecond;
        }

        internal void Add(long elapsedTicks)
        {
            ElapsedTicks += elapsedTicks;
            HitCount++;
            UpdateShortestPeriod(elapsedTicks);
            UpdateLongestPeriod(elapsedTicks);
        }

        private void UpdateShortestPeriod(long elapsedTicks)
        {
            if (elapsedTicks < ShortestPeriodTicks)
            {
                ShortestPeriodTicks = elapsedTicks;
            }
        }

        private void UpdateLongestPeriod(long elapsedTicks)
        {
            if (elapsedTicks > LongestPeriodTicks)
            {
                LongestPeriodTicks = elapsedTicks;
            }
        }

        internal PerfCounterResult Clone()
        {
            return (PerfCounterResult) MemberwiseClone();
        }
    }
}
