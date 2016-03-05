using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface ISettingsProvider
    {
        void Initialize();
        void CommitChanges();

        // Node identity
        string NodeId { get; set; }
        string ConfigUrl { get; set; }
        string HaveConfigId { get; set; }

        // configuration values
        string WifiSSID { get; set; }
        string WifiPassword { get; set; }
        string GatewayUrl { get; set; }
        bool UseDhcp { get; set; }
        string IpAddress { get; set; }
        string IpGateway { get; set; }
        string IpNetmask { get; set; }
        string DnsServers { get; set; }

        // local statistics
        int BootCount { get; set; }
    }
}
