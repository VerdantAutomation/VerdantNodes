using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface ISettingsProvider
    {
        void Initialize(Type appType);
        void CommitChanges();

        // Node identity
        string NodeId { get; set; }
        string ConfigUrl { get; set; }
        string HaveConfigId { get; set; }

        // configuration vlues
        string WifiSSID { get; set; }
        string WifiPassword { get; set; }
        string GatewayUrl { get; set; }

        // local statistics
        int BootCount { get; set; }
        int DroppedMessages { get; set; }
    }
}
