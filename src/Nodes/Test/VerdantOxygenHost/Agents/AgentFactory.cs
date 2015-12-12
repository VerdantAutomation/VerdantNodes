using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;

using PervasiveDigital.Utilities;

namespace VerdantOxygenHost.Agents
{
    class AgentFactory : IAgentFactory
    {
        private string _currentState;

        public IAgent[] CreateAgentsForState(string state)
        {
            if (state == EngineStates.Startup)
            {
                _currentState = "running";

                //// Get settings - if we know our ssid, password and server, then enter runtime, else enter onboarding state
                //var settings = (ISettingsProvider)DiContainer.Instance.Resolve(typeof(ISettingsProvider));

                //if (StringUtilities.IsNullOrEmpty(settings.WifiSSID) || !StringUtilities.IsNullOrEmpty(settings.WifiPassword) || !StringUtilities.IsNullOrEmpty(settings.GatewayUrl))
                //{
                //    _currentState = "onboarding";
                //}
                //else
                //{
                //    _currentState = "running";
                //}
            }

            switch (_currentState)
            {
                default:
                case "onboarding":
                    return new IAgent[]
                    {
                        //new WatchdogAgent(),  Deprecated in Oxygen, supported in Oracs G30
                        new OnboardingAgent()
                    };
                case "running":
                    return new IAgent[]
                        {
                        //new WatchdogAgent(),  Deprecated in Oxygen, supported in Oracs G30
                        new SenseAgent(),
                        new DisplayAgent(),
                        new ReportingAgent()
                        };
                case EngineStates.Shutdown:
                    return null;
            }
        }
    }
}
