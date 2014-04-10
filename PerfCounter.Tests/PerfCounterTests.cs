namespace PerfCounter.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class PerfCounterTests
    {
        [Test]
        public void PerfCounterResultHitCountShouldReturnCorrectCount()
        {
            // Arrange.
            const string counterName = "HitCounterName";
            PerfCounters.Reset(counterName);

            // Act & Assert.
            for (var i = 0; i < 10; i++)
            {
                using (new PerfCounter(counterName))
                {
                }

                Assert.AreEqual(i + 1, PerfCounters.Results[counterName].HitCount);
            }
        }

        [Test]
        public void PerfCounterResultInitialPeriodTicksShouldPreserveInitialValue()
        {
            // Arrange.
            const string counterName = "CounterName";
            PerfCounters.Reset(counterName);

            // Act.
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(10);
            }

            var initialPeriodTicks1 = PerfCounters.Results[counterName].InitialPeriodTicks;

            using (new PerfCounter(counterName))
            {
                Thread.Sleep(10);
            }

            var initialPeriodTicks2 = PerfCounters.Results[counterName].InitialPeriodTicks;

            // Assert.
            Assert.AreEqual(initialPeriodTicks1, initialPeriodTicks2);
        }

        [Test]
        public void PerfCounterResultLastPeriodTicksShouldReflectLastPeriodValue()
        {
            // Arrange.
            const string counterName = "LastPeriodTicks";
            PerfCounters.Reset(counterName);

            // Act.
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(10);
            }

            var lastPeriodTicks1 = PerfCounters.Results[counterName].LastPeriodTicks;

            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }

            var lastPeriodTicks2 = PerfCounters.Results[counterName].LastPeriodTicks;

            // Assert.
            Assert.AreNotEqual(lastPeriodTicks1, lastPeriodTicks2);
            Assert.Less(Math.Abs(lastPeriodTicks1 - TimeSpan.FromMilliseconds(10).Ticks), TimeSpan.FromMilliseconds(1.5).Ticks);
            Assert.Less(Math.Abs(lastPeriodTicks2 - TimeSpan.FromMilliseconds(100).Ticks), TimeSpan.FromMilliseconds(1.5).Ticks);
        }

        [Test]
        public void PerfCounterResultLongestAndShortestPeriodsShouldBeInitializedCorrectly()
        {
            // Arrange.
            const string counterName = "LongestAndShortestPeriods";
            PerfCounters.Reset(counterName);

            // Act.
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }

            // Assert.
            Assert.AreNotEqual(0, PerfCounters.Results[counterName].ShortestPeriodTicks);
            Assert.AreNotEqual(0, PerfCounters.Results[counterName].LongestPeriodTicks);
        }

        [Test]
        public void PerfCounterResultShouldBeImmutable()
        {
            // Arrange.
            const string counterName = "CounterName";

            // Act.
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }

            var counterResult = PerfCounters.Results[counterName];
            var elapsedTicks1 = counterResult.ElapsedTicks;

            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }

            var elapsedTicks2 = counterResult.ElapsedTicks;

            // Assert.
            Assert.AreEqual(elapsedTicks1, elapsedTicks2);
        }

        [Test]
        public void PerfCounterResultShouldCalculateAverageTicksCorrectly()
        {
            // Arrange.
            const string counterName = "CounterName";
            PerfCounters.Reset(counterName);

            // Act.
            for (var i = 0; i < 1000; i++)
            {
                using (new PerfCounter(counterName))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            }

            // Assert.
            Assert.Less(Math.Abs(PerfCounters.Results[counterName].AverageTicks - TimeSpan.FromMilliseconds(10).Ticks), TimeSpan.FromMilliseconds(1.5).Ticks);
        }

        [Test]
        public void PerfCounterShouldMeasureElapsedMillisecondsWithReasonableAccuracy()
        {
            // Arrange.
            const string counterName = "CounterName";
            PerfCounters.Reset(counterName);

            // Act.
            var stopwatch = Stopwatch.StartNew();
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }
            stopwatch.Stop();

            // Assert.
            var counterElapsedMilliseconds = PerfCounters.Results[counterName].ElapsedMilliseconds;
            var stopwatchElapsedMilliseconds = stopwatch.Elapsed.Milliseconds;

            Console.WriteLine("Counter milliseconds: {0}", counterElapsedMilliseconds);
            Console.WriteLine("Stopwatch milliseconds: {0}", stopwatchElapsedMilliseconds);

            Assert.Less(Math.Abs(stopwatchElapsedMilliseconds - counterElapsedMilliseconds), 10);
        }

        [Test]
        public void PerfCounterShouldMeasureElapsedTicksWithReasonableAccuracy()
        {
            // Arrange.
            const string counterName = "CounterName";
            PerfCounters.Reset(counterName);

            // Act.
            var stopwatch = Stopwatch.StartNew();
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(100);
            }
            stopwatch.Stop();

            // Assert.
            var counterElapsedTicks = PerfCounters.Results[counterName].ElapsedTicks;
            var stopwatchElapsedTicks = stopwatch.Elapsed.Ticks;

            Console.WriteLine("Counter ticks: {0}", counterElapsedTicks);
            Console.WriteLine("Stopwatch ticks: {0}", stopwatchElapsedTicks);

            Assert.Less(Math.Abs(stopwatchElapsedTicks - counterElapsedTicks), TimeSpan.FromMilliseconds(10).Ticks);
        }

        [Test]
        public void PerfCounterShouldSumElapsedTicks()
        {
            // Arrange.
            const string counterName = "CounterName";
            PerfCounters.Reset(counterName);

            // Act.
            var stopwatch = new Stopwatch();

            for (var i = 0; i < 100; i++)
            {
                stopwatch.Start();
                using (new PerfCounter(counterName))
                {
                    Thread.Sleep(100);
                }
                stopwatch.Stop();
            }

            // Assert.
            var counterElapsedTicks = PerfCounters.Results[counterName].ElapsedTicks;
            var stopwatchElapsedTicks = stopwatch.Elapsed.Ticks;

            Console.WriteLine("Counter milliseconds: {0}", counterElapsedTicks/TimeSpan.TicksPerMillisecond);
            Console.WriteLine("Stopwatch milliseconds: {0}", stopwatchElapsedTicks/TimeSpan.TicksPerMillisecond);

            Assert.Less(Math.Abs(stopwatchElapsedTicks - counterElapsedTicks), TimeSpan.FromMilliseconds(10).Ticks);
        }

        [Test]
        public void PerfCounterWithoutNameShouldThrowPerfCounterNameNotSpecifiedException()
        {
            Assert.Throws<PerfCounterNameNotSpecifiedException>(() =>
                {
                    using (new PerfCounter())
                    {
                    }
                });
        }

        [Test]
        public void PerfCountersResetAllShouldResetAllCounters()
        {
            // Arrange.
            const string counterName1 = "CounterName1";
            const string counterName2 = "CounterName2";
            const string counterName3 = "CounterName3";
            const string counterName4 = "CounterName4";
            PerfCounters.Reset(counterName1);
            PerfCounters.Reset(counterName2);
            PerfCounters.Reset(counterName3);
            PerfCounters.Reset(counterName4);

            using (new PerfCounter(counterName1))
            using (new PerfCounter(counterName2))
            using (new PerfCounter(counterName3))
            using (new PerfCounter(counterName4))
            {
                Thread.Sleep(0);
            }

            // Act.
            PerfCounters.ResetAll();

            using (new PerfCounter(counterName1))
            {
                Thread.Sleep(0);
            }

            // Assert.
            Assert.Greater(PerfCounters.Results[counterName1].ElapsedTicks, 0);
            Assert.AreEqual(0, PerfCounters.Results[counterName2].ElapsedTicks);
            Assert.AreEqual(0, PerfCounters.Results[counterName3].ElapsedTicks);
            Assert.AreEqual(0, PerfCounters.Results[counterName4].ElapsedTicks);
        }

        [Test]
        public void PerfCountersResetShouldResetSpecifiedCounter()
        {
            // Arrange.
            const string counterName = "CounterName";

            // Act.
            using (new PerfCounter(counterName))
            {
                Thread.Sleep(10);
            }
            PerfCounters.Reset(counterName);

            // Assert.
            Assert.AreEqual(0, PerfCounters.Results[counterName].ElapsedTicks);
        }
    }
}
