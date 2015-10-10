using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface IAgentFactory
    {
        IAgent[] CreateAgents();
    }
}
