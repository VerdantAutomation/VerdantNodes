using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;

namespace VerdantOxygenHost.Agents
{
    class OnboardingAgent : IAgent
    {
        public DateTime Process(DateTime now)
        {
            throw new NotImplementedException();

            // when onboarding is complete....
            var vengine = (IVerdantEngine)DiContainer.Instance.Resolve(typeof(IVerdantEngine));
            vengine.NavigateToEngineState("running");
        }

        public DateTime Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
