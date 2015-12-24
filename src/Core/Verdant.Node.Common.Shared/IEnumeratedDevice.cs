using System;

namespace Verdant.Node.Common
{
    interface IEnumeratedDevice
    {
        Guid Id { get; }
        string Name { get; }
    }
}
