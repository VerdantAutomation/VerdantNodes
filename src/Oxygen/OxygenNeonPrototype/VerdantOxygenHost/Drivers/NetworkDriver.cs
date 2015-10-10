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
        private NeonWifiDevice _wifi = new NeonWifiDevice();
        private PropertyDictionary _properties;
        private SntpClient _sntpClient;

        public void Start()
        {
            _properties = (PropertyDictionary)DiContainer.Instance.Resolve(typeof(PropertyDictionary));

            //TODO: Get this from EWR
            _wifi.Connect("CloudGate", "Escal8shun");

            _sntpClient = new SntpClient(_wifi, "time1.google.com");
            _sntpClient.Start();

            _properties.SetProperty(PropertyNames.IPAddressPropName, _wifi.StationIPAddress.ToString());
        }

        public void Stop()
        {
        }
    }
}
