using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface IDriverFactory
    {
        IDriver[] CreateDrivers();
    }
}
