using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Microsoft.SPOT.Hardware;

// This is deprecated for now - it will be supported in the Oracs port

namespace VerdantOxygenHost.Agents
{
    class WatchdogAgent : IAgent
    {
#if DEBUG
        public const int Interval = 120000;
#else
        public const int Interval = 5000;
#endif

        public DateTime Process(DateTime now)
        {
            
            // Come back and visit at least twice per watchdog interval
            return now + TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * Interval / 2);
        }

        public DateTime Start()
        {
            Watchdog.Behavior = WatchdogBehavior.HardReboot;
            Watchdog.Timeout = TimeSpan.FromTicks(TimeSpan.TicksPerMillisecond * Interval);
            Watchdog.Enabled = true;

            // Schedule an immediate callback
            return DateTime.MinValue;
        }

        public void Stop()
        {
        }
    }
}
