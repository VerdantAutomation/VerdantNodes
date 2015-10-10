using System;
using Microsoft.SPOT;
using Verdant.Node.Common;

namespace VerdantOxygenHost.Agents
{
    class AgentFactory : IAgentFactory
    {
        public IAgent[] CreateAgents()
        {
            return new IAgent[]
                {
                    //new WatchdogAgent(),  Deprecated in Oxygen, supported in Oracs G30
                    new SenseAgent(),
                    new DisplayAgent(),
                    new ReportingAgent()
                };
        }
    }
}
