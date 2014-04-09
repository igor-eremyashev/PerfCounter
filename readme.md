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


License
----

MIT
