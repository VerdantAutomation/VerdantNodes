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

            var engine = (IVerdantEngine)DiContainer.Instance.Resolve(typeof(IVerdantEngine));

            // Initialize the Verdant Node system
            engine.Initialize();

            // This function does not return
            engine.Run();
        }
    }
}
