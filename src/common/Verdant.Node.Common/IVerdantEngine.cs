using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public static class EngineStates
    {
        public const string Startup = "startup";
        public const string Shutdown = "shutdown";
    }

    public interface IVerdantEngine
    {
        void Initialize();
        void ScheduleNextRun(IAgent agent, DateTime runAt);
        void Run();
        void Stop();
        void NavigateToEngineState(string newState);
    }
}
