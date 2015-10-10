using System;
using Microsoft.SPOT;
using Verdant.Node.Common;
using IngenuityMicro.Hardware.Neon;
using PervasiveDigital.Net;
using Verdant.Node.Core;

namespace VerdantOxygenHost.Drivers
{
    class NetworkDriver : IDriver
    {
        private NeonWifiDevice _wifi;
        private PropertyDictionary _properties;
        private SntpClient _sntpClient;

        public void Start()
        {
            _properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));

            DiContainer.Instance.Register(typeof(INetworkAdapter), typeof(NeonWifiDevice)).AsSingleton();
            _wifi = (NeonWifiDevice)DiContainer.Instance.Resolve(typeof(INetworkAdapter));

            //TODO: Get this, the node id, Verdant server, etc., from EWR.  Need to create a settings service.
            _wifi.Connect("XXX", "XXX");

            _sntpClient = new SntpClient(_wifi, "time1.google.com");
            _sntpClient.Start();

            _properties.SetProperty(PropertyNames.IPAddressPropName, _wifi.StationIPAddress.ToString());

        }

        public void Stop()
        {
        }
    }
}
