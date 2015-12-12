using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Verdant.Node.Hardware.Sensor
{
    public class Hdc1008
    {

        private const int HDC1000_I2CADDR = 0x40;
        private const int HDC1000_TEMP = 0x00;
        private const int HDC1000_HUMID = 0x01;
        private const int HDC1000_CONFIG = 0x02;
        private const int HDC1000_CONFIG_RST = (1 << 15);
        private const int HDC1000_CONFIG_HEAT = (1 << 13);
        private const int HDC1000_CONFIG_MODE = (1 << 12);
        private const int HDC1000_CONFIG_BATT = (1 << 11);
        private const int HDC1000_CONFIG_TRES_14 = 0;
        private const int HDC1000_CONFIG_TRES_11 = (1 << 10);
        private const int HDC1000_CONFIG_HRES_14 = 0;
        private const int HDC1000_CONFIG_HRES_11 = (1 << 8);
        private const int HDC1000_CONFIG_HRES_8 = (1 << 9);

        private const int HDC1000_SERIAL1 = 0xFB;
        private const int HDC1000_SERIAL2 = 0xFC;
        private const int HDC1000_SERIAL3 = 0xFD;
        private const int HDC1000_MANUFID = 0xFE;
        private const int HDC1000_DEVICEID = 0xFF;

        private I2CDevice _i2c = null;
        private I2CDevice.I2CTransaction[] _trans1 = new I2CDevice.I2CTransaction[1];
        private byte[] _singleCommand = new byte[1];
        private byte[] _buffer2 = new byte[2];
        private byte[] _buffer4 = new byte[4];


        public Hdc1008(byte address = HDC1000_I2CADDR)
        {
            _i2c = new I2CDevice(new I2CDevice.Configuration(address, 400));

            Reset();
            if (Read16(HDC1000_MANUFID) != 0x5449)
                throw new Exception("Unexpected manuf value");
            if (Read16(HDC1000_DEVICEID) != 0x1000)
                throw new Exception("Unexpected device id value");
        }

        public void Reset()
        {
            // reset, and select 14 bit temp & humidity
            ushort config = HDC1000_CONFIG_RST | HDC1000_CONFIG_MODE | HDC1000_CONFIG_TRES_14 | HDC1000_CONFIG_HRES_14;
            _buffer2[0] = (byte)(config >> 8);
            _buffer2[1] = (byte)(config & 0xFF);
            _trans1[0] = I2CDevice.CreateWriteTransaction(_buffer2);
            _i2c.Execute(_trans1, 100);
            Thread.Sleep(15);
        }

        public float ReadTemperature()
        {
            var temp = (float)(Read32(HDC1000_TEMP) >> 16);
            temp /= 65536;
            temp *= 165;
            temp -= 40;

            temp = (float)System.Math.Round(temp * 10.0) / (float)10.0;

            return temp;
        }

        public float ReadHumidity()
        {
            var hum = (float)(Read32(HDC1000_TEMP) & 0xffff);
            hum /= 65536;
            hum *= 100;

            hum = (float)System.Math.Round(hum * (float)10.0) / (float)10.0;

            return hum;
        }

        private ushort Read16(byte addr, int delay = 0)
        {
            _singleCommand[0] = addr;
            _trans1[0] = I2CDevice.CreateWriteTransaction(_singleCommand);
            _i2c.Execute(_trans1, 100);

            if (delay > 0)
                Thread.Sleep(delay);

            _trans1[0] = I2CDevice.CreateReadTransaction(_buffer2);
            _i2c.Execute(_trans1, 100);

            return (ushort)(_buffer2[0] << 8 | _buffer2[1]);
        }

        private uint Read32(byte addr)
        {
            _singleCommand[0] = addr;
            _trans1[0] = I2CDevice.CreateWriteTransaction(_singleCommand);
            _i2c.Execute(_trans1, 100);

            Thread.Sleep(50);

            _trans1[0] = I2CDevice.CreateReadTransaction(_buffer4);
            _i2c.Execute(_trans1, 100);

            return (uint)(_buffer4[0] << 24 | _buffer4[1] << 16 | _buffer4[2] << 8 | _buffer4[3]);
        }
    }
}
