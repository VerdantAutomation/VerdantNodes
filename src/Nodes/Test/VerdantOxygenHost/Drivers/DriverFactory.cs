using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using Verdant.Node.Core;

namespace VerdantOxygenHost.Drivers
{
    class DriverFactory : IDriverFactory
    {
        public IDriver[] CreateDrivers()
        {
            return new IDriver[]
                {
                    (IDriver)DiContainer.Instance.Resolve(typeof(ICharacterLcdDriver)),
                    new NetworkDriver()
                };
        }
    }
}
