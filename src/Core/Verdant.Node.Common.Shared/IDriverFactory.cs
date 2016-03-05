using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface IDriverFactory
    {
        // Low-level drivers that do not rely on settings
        IDriver[] CreatePrimaryDrivers();
        // Drivers that may be composed of lower-level drivers, or which use settings
        IDriver[] CreateSecondaryDrivers();
    }
}
