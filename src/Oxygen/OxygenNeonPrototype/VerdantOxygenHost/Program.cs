using System;
using Microsoft.SPOT;

using Verdant.Node.Core;
using Verdant.Node.Common;

namespace VerdantOxygenHost
{
    public class Program
    {
        public static void Main()
        {
            DiContainer.Instance.Install(
                new Verdant.Node.Core.Installer(),
                new VerdantOxygenHost.Installer()
                );

            // This should be persisted in EWR and always return the same value, but be unique for every node
            var properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));
            properties.SetProperty(PropertyNames.NodeIdPropName, Guid.NewGuid());

            //TODO: record number of reboots, boot time, dropped messages, etc as EWR values - pull those out into properties at startup

            // Initialize the Verdant Node framework
            var engine = (IVerdantEngine)DiContainer.Instance.Resolve(typeof(IVerdantEngine));
            engine.Initialize();

            // This function does not return
            engine.Run();
        }
    }
}
