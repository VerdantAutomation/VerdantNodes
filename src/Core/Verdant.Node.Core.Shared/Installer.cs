using System;
using Microsoft.SPOT;
using Verdant.Node.Core;
using Verdant.Node.Common;

namespace Verdant.Node.Core
{
    public class Installer : IContainerInstaller
    {
        public void Install(Container container)
        {
            container.Register(typeof(IVerdantEngine), typeof(VerdantEngine)).AsSingleton();
            container.Register(typeof(PropertyDictionary), typeof(PropertyDictionary)).AsSingleton();
            container.Register(typeof(ISpiChannelManager), typeof(ISpiChannelManager)).AsSingleton();
        }
    }
}
