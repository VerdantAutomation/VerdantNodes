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
            container.Register(typeof(IDriverFactory), typeof(Drivers.DriverFactory)).AsSingleton();

            // Services
            container.Register(typeof(ISettingsProvider), typeof(Services.SettingsProvider)).AsSingleton();
        }
    }
}
