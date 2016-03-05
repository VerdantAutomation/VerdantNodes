using System;
using System.Text;
using Microsoft.SPOT.Hardware;

namespace Verdant.Node.Common
{
    public interface ISpiChannelManager
    {
        SPI this[SPI.SPI_module index] { get; set; }
        IDisposable ObtainExclusiveAccess(SPI.Configuration config);
    }
}
