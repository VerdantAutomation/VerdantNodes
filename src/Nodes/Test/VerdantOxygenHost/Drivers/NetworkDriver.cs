using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Verdant.Node.Common;
using PervasiveDigital.Hardware.ESP8266;
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
            Thread.Sleep(2000);

            //TODO: Get this, the node id, Verdant server, etc., from EWR.  Need to create a settings service.
            _wifi.Connect("CloudGate", "Escal8shun");

            _sntpClient = new SntpClient(_wifi, "time1.google.com");
            _sntpClient.SetTime();

            _properties.SetProperty(PropertyNames.IPAddressPropName, _wifi.StationIPAddress.ToString());

        }

        public void Stop()
        {
        }
    }

    public sealed class NeonWifiDevice : Esp8266WifiDevice
    {
        public NeonWifiDevice()
            : this("COM2")
        {
        }

        public NeonWifiDevice(string comPortName) :
            base(new SerialPort(comPortName, 115200, Parity.None, 8, StopBits.One), new OutputPort((Cpu.Pin)((1 * 16) + 3), false), null)
        {
        }

        public NeonWifiDevice(string comPortName, OutputPort resetPin) :
            base(new SerialPort(comPortName, 115200, Parity.None, 8, StopBits.One), new OutputPort((Cpu.Pin)((1 * 16) + 3), false), resetPin)
        {
        }

    }
}
