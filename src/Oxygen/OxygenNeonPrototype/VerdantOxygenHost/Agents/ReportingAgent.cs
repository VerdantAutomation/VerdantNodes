using System;
using Microsoft.SPOT;
using Verdant.Node.Common;

namespace VerdantOxygenHost.Agents
{
    class ReportingAgent : IAgent
    {
        private TimeSpan ProcessingInterval = new TimeSpan(0, 1, 0);

        public DateTime Process(DateTime now)
        {
            // Once per minute, send data
            Debug.Print("Reporting Agent Processing");

            return now + ProcessingInterval;
        }

        public DateTime Start()
        {
            return DateTime.Now + new TimeSpan(TimeSpan.TicksPerMinute);
        }

        public void Stop()
        {
        }
    }
}
