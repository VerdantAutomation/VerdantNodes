using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface IVerdantEngine
    {
        void Initialize();
        void ScheduleNextRun(IAgent agent, DateTime runAt);
        void Run();
        void Stop();
    }
}
