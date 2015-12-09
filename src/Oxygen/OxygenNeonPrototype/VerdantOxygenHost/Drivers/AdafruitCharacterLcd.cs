using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Verdant.Node.Common;

using VerdantOxygenHost.Drivers.PortExpander;

namespace VerdantOxygenHost.Drivers
{
    class AdafruitCharacterLcd : ICharacterLcdDriver
    {
        // commands
        private const byte LCD_CLEARDISPLAY = 0x01;
        private const byte LCD_RETURNHOME = 0x02;
        private const byte LCD_ENTRYMODESET = 0x04;
        private const byte LCD_DISPLAYCONTROL = 0x08;
        private const byte LCD_CURSORSHIFT = 0x10;
        private const byte LCD_FUNCTIONSET = 0x20;
        private const byte LCD_SETCGRAMADDR = 0x40;
        private const byte LCD_SETDDRAMADDR = 0x80;

        // flags for display entry mode
        private const byte LCD_ENTRYRIGHT = 0x00;
        private const byte LCD_ENTRYLEFT = 0x02;
        private const byte LCD_ENTRYSHIFTINCREMENT = 0x01;
        private const byte LCD_ENTRYSHIFTDECREMENT = 0x00;

        // flags for display on/off control
        private const byte LCD_DISPLAYON = 0x04;
        private const byte LCD_DISPLAYOFF = 0x00;
        private const byte LCD_CURSORON = 0x02;
        private const byte LCD_CURSOROFF = 0x00;
        private const byte LCD_BLINKON = 0x01;
        private const byte LCD_BLINKOFF = 0x00;

        // flags for display/cursor shift
        private const byte LCD_DISPLAYMOVE = 0x08;
        private const byte LCD_CURSORMOVE = 0x00;
        private const byte LCD_MOVERIGHT = 0x04;
        private const byte LCD_MOVELEFT = 0x00;

        private const int LCD_8BITMODE = 0x10;
        private const int LCD_4BITMODE = 0x00;
        private const int LCD_2LINE = 0x08;
        private const int LCD_1LINE = 0x00;
        private const int LCD_5x10DOTS = 0x04;
        private const int LCD_5x8DOTS = 0x00;

        private byte _displayFunction;
        private byte _displayControl;
        private byte _displayMode;
        private Pin _rs_pin; // LOW: command.  HIGH: character.
        //private byte _rw_pin; // LOW: write to LCD.  HIGH: read from LCD.
        private Pin _enable_pin; // activated by a HIGH pulse.
        private Pin[] _data_pins = new Pin[8];
        private Pin _data_mask;

        private int _rows = 4;
        private int _cols = 20;
        private MCP23008 _i2c;

        public void Start()
        {
            _i2c = new MCP23008(0x20, null);

            _rs_pin = Pin.GP1;
            _enable_pin = Pin.GP2;
            _data_pins[0] = Pin.GP3;
            _data_pins[1] = Pin.GP4;
            _data_pins[2] = Pin.GP5;
            _data_pins[3] = Pin.GP6;

            _data_mask = _data_pins[0] | _data_pins[1] | _data_pins[2] | _data_pins[3];

            _displayFunction = LCD_4BITMODE | LCD_2LINE | LCD_5x8DOTS;
            
            // turn on backlight
            _i2c.PinDirection(Pin.GP7, Direction.Output);
            _i2c.WritePin(Pin.GP7, true);

            for (int i = 0; i < 4; ++i)
            {
                _i2c.PinDirection(_data_pins[i], Direction.Output);
            }

            _i2c.PinDirection(_rs_pin, Direction.Output);
            _i2c.PinDirection(_enable_pin, Direction.Output);

            Thread.Sleep(50);  // minimum 40ms post-power-up delay

            _i2c.WritePin(_rs_pin, false);
            _i2c.WritePin(_enable_pin, false);

            // Send function set command sequence
            SendCommand((byte)(LCD_FUNCTIONSET | (byte)0x03));
            Thread.Sleep(5);  // wait more than 4.1ms
            SendCommand((byte)(LCD_FUNCTIONSET | (byte)0x03));
            Thread.Sleep(5);
            SendCommand((byte)(LCD_FUNCTIONSET | (byte)0x03));
            Thread.Sleep(1);
            SendCommand((byte)(LCD_FUNCTIONSET | (byte)0x02));
            Thread.Sleep(1);

            // finally, set # lines, font size, etc.
            SendCommand((byte)(LCD_FUNCTIONSET | _displayFunction));
            Thread.Sleep(1);

            _displayControl = LCD_DISPLAYON | LCD_CURSOROFF | LCD_BLINKOFF;
            EnableDisplay(true);

            // Initialize to default text direction (for romance languages)
            _displayMode = LCD_ENTRYLEFT | LCD_ENTRYSHIFTDECREMENT;
            // set the entry mode
            SendCommand((byte)(LCD_ENTRYMODESET | _displayMode));

            Thread.Sleep(100);
            Backlight(true);
            Clear();
            Print("Hello");
            SetCursor(0, 1);
            Print("World");
            SetCursor(0, 2);
            Print("This is ");
            SetCursor(0, 3);
            Print("a test");
        }

        public void Stop()
        {
        }

        public int Rows { get { return _rows; } }
        public int Columns { get { return _cols; } }

        public void Home()
        {
            SendCommand(LCD_RETURNHOME);
            Thread.Sleep(2);
        }

        public void Clear()
        {
            SendCommand(LCD_CLEARDISPLAY);
            Thread.Sleep(2);
        }

        public void Print(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            foreach (var ch in data)
            {
                Write(ch);
            }
        }

        public void EnableDisplay(bool fEnable)
        {
            if (fEnable)
                _displayControl |= LCD_DISPLAYON;
            else
            {
                _displayControl &= (byte) (~(LCD_DISPLAYON) & 0xFF);
            }
            SendCommand((byte)(LCD_DISPLAYCONTROL | _displayControl));
        }

        public void Backlight(bool fEnable)
        {
            _i2c.WritePin(Pin.GP7, fEnable);
        }

        public void SetCursor(byte col, byte row)
        {
            int[] rowOffsets = new[] { 0x00, 0x40, 0x14, 0x54 };
            if (row > _rows)
            {
                row = (byte)(_rows - 1); // we count rows starting w/0
            }
            SendCommand((byte)(LCD_SETDDRAMADDR | (col + rowOffsets[row])));
        }

        private void SendCommand(byte cmd)
        {
            Send(cmd, false);
        }

        private void Write(byte b)
        {
            Send(b, true);
        }

        private void Send(byte b, bool mode)
        {
            _i2c.WritePin(_rs_pin, mode);
            Write4Bits((byte)((b >> 4) & 0x0f));
            Write4Bits((byte)(b & 0x0f));
        }

        private void Write4Bits(byte b)
        {
            var value = _i2c.ReadPins(Pin.ALL);
            value &= (byte)~_data_mask;
            value |= (byte) (b << 3);
            value &= (byte)~((byte) _enable_pin);
            _i2c.Write(Register.GPIO, value);
            // pulse enable
            value |= (byte)_enable_pin;
            _i2c.Write(Register.GPIO, value);
            value &= (byte)~(byte)_enable_pin;
            _i2c.Write(Register.GPIO, value);
            Thread.Sleep(1);
        }
    }
}
