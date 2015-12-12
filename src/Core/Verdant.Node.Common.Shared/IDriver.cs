using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface IDriver
    {
        void Start();
        void Stop();
    }
}
