using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Verdant.Node.Hardware.PortExpander
{
    public enum Register
    {
        /// <summary>
        /// I/O Direction
        /// 0 = output
        /// 1 = input
        /// </summary>
        IODIR = 0x00,

        /// <summary>
        /// Input polarity
        /// 0 = normal
        /// 1 = invert
        /// </summary>
        IPOL = 0x01,

        /// <summary>
        /// Interrupt on change
        /// 0 = disable
        /// 1 = enable
        /// </summary>
        GPINTEN = 0x02,

        /// <summary>
        /// Default Compare for interrupt
        /// </summary>
        DEFVAL = 0x03,

        /// <summary>
        /// Interrupt Control
        /// 0 = compare with previous value
        /// 1 = compare with DEFVAL
        /// </summary>
        INTCON = 0x04,

        /// <summary>
        /// Config Register
        /// default is 0x00
        /// </summary>
        IOCON = 0x05,

        /// <summary>
        /// Pull-Up Resistor
        /// 0 = disabled
        /// 1 = enabled
        /// </summary>
        GPPU = 0x06,

        /// <summary>
        /// Interrupt Flag, read GPIO or INTCAP to clear
        /// </summary>
        INTF = 0x07,

        /// <summary>
        /// Interrupt Capture
        /// GPIO value at the time of interrupt
        /// </summary>
        INTCAP = 0x08,

        /// <summary>
        /// IO Port
        /// Writing will only set pins configured as output
        /// </summary>
        GPIO = 0x09,

        /// <summary>
        /// Output Latch
        /// </summary>
        OLAT = 0x0A,
    }

    public enum Pin
    {
        GP0 = 0x01,
        GP1 = 0x02,
        GP2 = 0x04,
        GP3 = 0x08,
        GP4 = 0x10,
        GP5 = 0x20,
        GP6 = 0x40,
        GP7 = 0x80,
        ALL = 0xFF,
    }

    public enum Direction
    {
        Output = 0x00,
        Input = 0x01,
    }

    public enum Polarity
    {
        Normal = 0x00,
        Invert = 0x01,
    }

    public enum InterruptControl
    {
        PreviousValue = 0x00,
        DefaultValue = 0x01,
    }

    public enum ConfigRegister
    {
        /// <summary>
        /// Sequential Operation
        /// 0 = enabled(default)
        /// 1 = disabled
        /// </summary>
        SEQOP = 0x20,

        /// <summary>
        /// SDA Slew Rate
        /// 0 = enabled(default)
        /// 1 = disabled
        /// </summary>
        DISSLW = 0x10,

        /// <summary>
        /// Hardware Address Enable for MCP23S08 SPI version only
        /// 0 = disabled(default)
        /// 1 = enabled
        /// </summary>
        HAEN = 0x08,

        /// <summary>
        /// INT pin as open-drain
        /// 0 = Active driver output(default)
        /// 1 = Open-drain output
        /// </summary>
        ODR = 0x04,

        /// <summary>
        /// INT polarity
        /// 0 = Active-low(default)
        /// 1 = Active-high
        /// </summary>
        INTPOL = 0x02,
    }

    public class MCP23008
    {
        I2CDevice i2c = null;
        I2CDevice.I2CTransaction[] writeTrans = new I2CDevice.I2CTransaction[1];
        I2CDevice.I2CTransaction[] readTrans = new I2CDevice.I2CTransaction[2];
        OutputPort resetPin;
        byte[] singleCommand = new byte[1];
        byte[] writeBuffer = new byte[2];
        byte[] readBuffer = new byte[1];

        /// <summary>
        /// Init with I2C address and optional hardware reset pin
        /// Set reset pin 6 high if not used
        /// </summary>
        /// <param name="address">0x20 to 0x27</param>
        /// <param name="resetPin">set null if not used</param>
        public MCP23008(byte address, OutputPort resetPin)
        {
            this.i2c = new I2CDevice(new I2CDevice.Configuration(address, 400));
            this.resetPin = resetPin;
            Init();
        }

        public MCP23008()
        {
            this.i2c = new I2CDevice(new I2CDevice.Configuration(0x20, 400));
            this.resetPin = null;
            Init();
        }

        private void Init()
        {
            Reset();
            ClearInterrupt();
        }

        /// <summary>
        /// Write single register
        /// </summary>
        /// <param name="reg">Register name</param>
        /// <param name="val">Register value</param>
        public void Write(Register reg, byte val)
        {
            writeBuffer[0] = (byte)reg;
            writeBuffer[1] = val;
            writeTrans[0] = I2CDevice.CreateWriteTransaction(writeBuffer);
            i2c.Execute(writeTrans, 100);
        }

        /// <summary>
        /// Read single register
        /// </summary>
        /// <param name="reg">Register name</param>
        /// <returns>Register value</returns>
        public byte Read(Register reg)
        {
            singleCommand[0] = (byte)reg;
            readTrans[0] = I2CDevice.CreateWriteTransaction(singleCommand);
            readTrans[1] = I2CDevice.CreateReadTransaction(readBuffer);
            i2c.Execute(readTrans, 100);
            return readBuffer[0];
        }

        /// <summary>
        /// Hardware or software reset
        /// </summary>
        public void Reset()
        {
            if (resetPin != null)
            {
                resetPin.Write(false);
                Thread.Sleep(5);
                resetPin.Write(true);
                Thread.Sleep(5);
            }
            else {
                //set default Reset values if not using hardware reset pin
                Write(Register.IODIR, 0xFF);
                Write(Register.IPOL, 0x00);
                Write(Register.GPINTEN, 0x00);
                Write(Register.DEFVAL, 0x00);
                Write(Register.INTCON, 0x00);
                Write(Register.IOCON, 0x00);
                Write(Register.GPPU, 0x00);
                Write(Register.INTF, 0x00);
                Write(Register.INTCAP, 0x00);
                Write(Register.GPIO, 0x00);
                Write(Register.OLAT, 0x00);
            }
        }

        /// <summary>
        /// Write GPIO pin(s)
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="value">value</param>
        public void WritePin(Pin pinMask, bool value)
        {
            SetRegisterForPin(Register.GPIO, pinMask, value);
        }

        /// <summary>
        /// Read single GPIO pin
        /// </summary>
        /// <param name="pin">pin</param>
        /// <returns>pin value</returns>
        public bool ReadPin(Pin pin)
        {
            byte current_GPIO = Read(Register.GPIO);
            if ((current_GPIO & (byte)pin) > 0)
            {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Read GPIO pin(s)
        /// </summary>
        /// <param name="pin">pin mask</param>
        /// <returns>masked value</returns>
        public byte ReadPins(Pin pinMask)
        {
            byte current_GPIO = Read(Register.GPIO);
            current_GPIO &= (byte)pinMask;
            return current_GPIO;
        }

        /// <summary>
        /// Set pin direction mask
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="dir">direction</param>
        public void PinDirection(Pin pinMask, Direction dir)
        {
            bool value = false;
            if (dir == Direction.Input)
            {
                value = true;
            }
            SetRegisterForPin(Register.IODIR, pinMask, value);
        }

        /// <summary>
        /// Set pin input polarity mask
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="pol">polarity</param>
        public void PinPolarity(Pin pinMask, Polarity pol)
        {
            bool value = false;
            if (pol == Polarity.Invert)
            {
                value = true;
            }
            SetRegisterForPin(Register.GPINTEN, pinMask, value);
        }

        /// <summary>
        /// Set pin interrupt on change mask
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="value">value</param>
        public void PinInterruptOnChange(Pin pinMask, bool value)
        {
            SetRegisterForPin(Register.GPINTEN, pinMask, value);
        }

        /// <summary>
        /// Set pin mask default value to cause interrupt
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="value">value</param>
        public void PinDefaultValue(Pin pinMask, bool value)
        {
            SetRegisterForPin(Register.DEFVAL, pinMask, value);
        }

        /// <summary>
        /// Set pin pullup resistor mask
        /// </summary>
        /// <param name="pin">pin</param>
        /// <param name="value">value</param>
        public void PinPullup(Pin pinMask, bool value)
        {
            SetRegisterForPin(Register.GPPU, pinMask, value);
        }

        /// <summary>
        /// Set interrupt compare mode
        /// </summary>
        /// <param name="pinMask">pins</param>
        /// <param name="ic">mode</param>
        public void PinInterruptControl(Pin pinMask, InterruptControl ic)
        {
            bool value = false;
            if (ic == InterruptControl.DefaultValue)
            {
                value = true;
            }
            SetRegisterForPin(Register.INTCON, pinMask, value);
        }

        /// <summary>
        /// Set pin value mask
        /// </summary>
        /// <param name="reg">register</param>
        /// <param name="pin">pin</param>
        /// <param name="value">value</param>
        private void SetRegisterForPin(Register reg, Pin pinMask, bool value)
        {
            byte current = Read(reg);
            if (value)
            {
                current |= (byte)pinMask;
            }
            else {
                current &= (byte)((int)~pinMask);
            }
            Write(reg, current);
        }

        /// <summary>
        /// Set configuration
        /// </summary>
        /// <param name="cr">config</param>
        public void SetConfigRegister(ConfigRegister cr)
        {
            Write(Register.IOCON, (byte)cr);
        }

        /// <summary>
        /// Clear INTF by reading INTCAP
        /// </summary>
        public void ClearInterrupt()
        {
            Read(Register.INTCAP);
        }

    }
}

