using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT.Hardware;

using Verdant.Node.Common;

namespace Verdant.Node.Core.Drivers
{
    public class SpiChannelManager : IDriver, ISpiChannelManager
    {
        private readonly SPI[] _channels = new SPI[4];
        private readonly SpiLockObject[] _locks = { new SpiLockObject(), new SpiLockObject(), new SpiLockObject(), new SpiLockObject() };

        public SPI this[SPI.SPI_module index]
        {
            get { return _channels[(int)index]; }
            set { _channels[(int)index] = value; }
        }

        public IDisposable ObtainExclusiveAccess(SPI.Configuration config)
        {
            _locks[(int) config.SPI_mod].Enter();
            _channels[(int) config.SPI_mod].Config = config;
            return _locks[(int) config.SPI_mod];
        }

        public void Start()
        {
            // Register ourselves as also being a service
            DiContainer.Instance.Register(typeof(ISpiChannelManager), () => this);
        }

        public void Stop()
        {
        }
    }

    class SpiLockObject : IDisposable
    {
        private readonly object _lock = new object();
        private bool _locked = false;

        ~SpiLockObject()
        {
            Dispose();
        }

        public void Enter()
        {
            Monitor.Enter(_lock);
            _locked = true;
        }

        public void Dispose()
        {
            if (_locked)
            {
                _locked = false;
                Monitor.Exit(_lock);
            }
        }
    }
}
