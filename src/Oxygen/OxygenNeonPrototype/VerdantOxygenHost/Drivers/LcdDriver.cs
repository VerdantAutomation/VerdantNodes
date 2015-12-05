using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

using Verdant.Node.Common;

namespace VerdantOxygenHost.Drivers
{
    /// <summary>
    /// Driver for the DFRobot I2C/TWI LCD1602 Module
    /// <a href="http://www.dfrobot.com/index.php?route=product/product&path=53&product_id=135" target="_blank" rel="nofollow">http://www.dfrobot.com/index.php?route=product/product&path=53&product_id=135</a>
    ///
    /// This display uses a JHD 162A LCD module with a DFRobot I2C Module
    /// The I2C module uses a PCA8574 I/O Expander at Address 0x27
    /// <a href="http://www.nxp.com/documents/data_sheet/PCA8574_PCA8574A.pdf" target="_blank" rel="nofollow">http://www.nxp.com/documents/data_sheet/PCA8574_PCA8574A.pdf</a>
    ///
    /// Code is adapted from the arduino code:
    /// <a href="http://www.dfrobot.com/image/data/DFR0063/Arduino_library.zip" target="_blank" rel="nofollow">http://www.dfrobot.com/image/data/DFR0063/Arduino_library.zip</a>
    ///
    /// The module should be connected to the I2C port on the FEZ - sda (Data2) and scl (Data3)
    ///
    /// Refer to documentation on the Hitachi HD44780 for more detailed operational information
    /// Eg: <a href="http://lcd-linux.sourceforge.net/pdfdocs/lcd1.pdf" target="_blank" rel="nofollow">http://lcd-linux.sourceforge.net/pdfdocs/lcd1.pdf</a>
    /// </summary>
    class LcdDriver : ILcd2x16Driver
    {
        // The following are the first 4 bits of each byte.
        const byte RS = 0x01; // Register select bit. 0=command 1=data
        const byte RW = 0x02; // Read/Write bit. We usually want to write (0).
        const byte EN = 0x04; // Enable bit. Data is set on the falling edge - see hitachi doco
        // flags for backlight control
        const byte LCD_BACKLIGHT = 0x08;
        const byte LCD_NOBACKLIGHT = 0x00;

        // The following are the high 4 bits - compounded with the flags below
        // Note that everything must be done in 4bit mode, so set 4bit mode first.

        // commands
        const byte LCD_CLEARDISPLAY = 0x01;
        const byte LCD_RETURNHOME = 0x02;
        const byte LCD_ENTRYMODESET = 0x04;
        const byte LCD_DISPLAYCONTROL = 0x08;
        const byte LCD_CURSORSHIFT = 0x10;
        const byte LCD_FUNCTIONSET = 0x20;
        const byte LCD_SETCGRAMADDR = 0x40;
        const byte LCD_SETDDRAMADDR = 0x80;

        // Flags to be used with the above commands

        // flags for display entry mode (0x04)
        const byte LCD_ENTRYRIGHT = 0x00;
        const byte LCD_ENTRYLEFT = 0x02;
        const byte LCD_ENTRYSHIFTINCREMENT = 0x01;
        const byte LCD_ENTRYSHIFTDECREMENT = 0x00;

        // flags for display on/off control (0x0 <img src="/img/forum/smilies/cool.gif" title="Cool" alt="Cool">
        const byte LCD_DISPLAYON = 0x04;
        const byte LCD_DISPLAYOFF = 0x00;
        const byte LCD_CURSORON = 0x02;
        const byte LCD_CURSOROFF = 0x00;
        const byte LCD_BLINKON = 0x01;
        const byte LCD_BLINKOFF = 0x00;

        // flags for display/cursor shift (0x10)
        const byte LCD_DISPLAYMOVE = 0x08;
        const byte LCD_CURSORMOVE = 0x00;
        const byte LCD_MOVERIGHT = 0x04;
        const byte LCD_MOVELEFT = 0x00;

        // flags for function set (0x20)
        const byte LCD_8BITMODE = 0x10;
        const byte LCD_4BITMODE = 0x00;
        const byte LCD_2LINE = 0x08;
        const byte LCD_1LINE = 0x00;
        const byte LCD_5x10DOTS = 0x04;
        const byte LCD_5x8DOTS = 0x00;


        private byte _backLight = LCD_BACKLIGHT; // default to backlight on

        private byte[] _cmd_arr = new byte[1];

        private byte[] _row_offsets = { 0x00, 0x40, 0x14, 0x54 };   // this is for a 20x4 display

        private I2CDevice _myI2C;
        I2CDevice.I2CTransaction[] _i2cXA = new I2CDevice.I2CTransaction[1];

        public LcdDriver()
        {
        }

        public void Backlight(bool on)
        {
            if (on)
                backLightOn();
            else
                backLightOff();
        }

        public void Clear()
        {
            write4bit(LCD_CLEARDISPLAY);
            Thread.Sleep(15);
            write4bit(LCD_RETURNHOME);
            Thread.Sleep(5);
        }

        public void Home()
        {
            setCursor(0, 1);
        }

        public void SetCursor(byte col, byte row)
        {
            setCursor(col, row);
        }

        public void Print(string text)
        {
            for (int i = 0; i < text.Length; i++)
                write4bit((byte)(text[i]), RS);
        }

        public void Start()
        {
            //ushort address = 0x27;  // DFRobot
            ushort address = 0x0; // Adafruit, with no jumpers soldered
            int clockRateKhz = 400;

            I2CDevice.Configuration cfg = new I2CDevice.Configuration(address, clockRateKhz);
            _myI2C = new I2CDevice(cfg);

            Thread.Sleep(15);

            // Set 4 Bit mode - copied from arduino code
            write(LCD_FUNCTIONSET | LCD_8BITMODE);
            write(LCD_FUNCTIONSET | LCD_8BITMODE);
            write(LCD_FUNCTIONSET | LCD_8BITMODE);
            write(LCD_FUNCTIONSET | LCD_4BITMODE);
            Thread.Sleep(1);
            // COMMAND | FLAG1 | FLAG2 | ...
            write4bit(LCD_FUNCTIONSET | LCD_4BITMODE | LCD_2LINE | LCD_5x8DOTS);
            Thread.Sleep(1);
            write4bit(LCD_ENTRYMODESET | LCD_ENTRYLEFT);
            Thread.Sleep(1);
            write4bit(LCD_DISPLAYCONTROL | LCD_DISPLAYON | LCD_CURSORON);
            Thread.Sleep(1);
            write4bit(LCD_CLEARDISPLAY);
            Thread.Sleep(15);

            Backlight(true);
        }

        public void Stop()
        {
            Backlight(false);
            Clear();
        }

        /// <summary>
        /// Writes a byte in 4bit mode.
        /// </summary>
        /// <param name="byteOut">The byte to write</param>
        /// <param name="mode">Additional Parameters - eg RS for data mode</param>
        public void write4bit(byte byteOut, byte mode = 0)
        {
            write((byte)(byteOut & 0xF0), mode);
            write((byte)((byteOut << 4) & 0xF0), mode);
        }


        /// <summary>
        /// Writes a byte to the I2C LCD.
        /// </summary>
        /// <param name="byteOut">The byte to write</param>
        /// <param name="mode">Additional Parameters - eg RS for data mode</param>
        public void write(byte byteOut, byte mode = 0)
        {
            // Write the byte
            // Set the En bit high
            _cmd_arr[0] = (byte)(byteOut | _backLight | mode | EN);
            _i2cXA[0] = I2CDevice.CreateWriteTransaction(_cmd_arr);
            _myI2C.Execute(_i2cXA, 500);

            // Set the En bit low
            _cmd_arr[0] = (byte)(byteOut | _backLight | mode);
            _i2cXA[0] = I2CDevice.CreateWriteTransaction(_cmd_arr);
            _myI2C.Execute(_i2cXA, 500);
        }

        /// <summary>
        /// Sets the cursor position. Zero based column and row.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public void setCursor(byte col, byte row)
        {
            write4bit((byte)(LCD_SETDDRAMADDR | (col + _row_offsets[row])));
        }

        /// <summary>
        /// Turn the backlight off.
        /// </summary>
        private void backLightOff()
        {
            _backLight = LCD_NOBACKLIGHT;
            _cmd_arr[0] = (byte)(_backLight | EN); // do not set Enable flag to low so we don't write to the LCD
            _i2cXA[0] = I2CDevice.CreateWriteTransaction(_cmd_arr);
            _myI2C.Execute(_i2cXA, 500);
        }

        /// <summary>
        /// Turn the backlight on.
        /// </summary>
        private void backLightOn()
        {
            _backLight = LCD_BACKLIGHT;
            _cmd_arr[0] = (byte)(_backLight | EN);
            _i2cXA[0] = I2CDevice.CreateWriteTransaction(_cmd_arr);
            _myI2C.Execute(_i2cXA, 500);
        }
    }
}
