PerfCounter
=========

A simple .NET performance counter/timer.

Usage
----

```
using (new PerfCounter("CounterName"))
{
    // Some operations here...
}
var elapsedTicks = PerfCounters.Results["CounterName"].ElapsedTicks;
```

Features
----

* Hit counter
* Elapsed ticks/milliseconds counter
* Average execution time measurement
* Shortest and longest execution time measurement

License
----

MIT
