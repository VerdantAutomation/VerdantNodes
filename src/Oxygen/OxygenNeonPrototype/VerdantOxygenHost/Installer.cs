using System;
using Microsoft.SPOT;
using Verdant.Node.Core;
using Verdant.Node.Common;

namespace VerdantOxygenHost
{
    class Installer : IContainerInstaller
    {
        public void Install(Container container)
        {
            container.Register(typeof(IAgentFactory), typeof(Agents.AgentFactory)).AsSingleton();
            container.Register(typeof(ILcd2x16Driver), typeof(Drivers.LcdDriver)).AsSingleton();
            container.Register(typeof(IDriverFactory), typeof(Drivers.DriverFactory)).AsSingleton();
        }
    }
}
