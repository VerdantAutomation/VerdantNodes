using System;
using System.Text;
using Verdant.Node.Common;

namespace Verdant.Node.Hardware.Display.Adafruit292.Shared
{
    class Adafruit292Driver : ICharacterLcdDriver
    {
        private Adafruit292CharacterLcd _lcd;

        public void Start()
        {
            _lcd = new Adafruit292CharacterLcd();
        }

        public void Stop()
        {
        }

        public int Columns
        {
            get { return _lcd.Columns; }
        }

        public int Rows
        {
            get { return _lcd.Rows; }
        }


        public void Backlight(bool on)
        {
            _lcd.Backlight(on);
        }

        public void Clear()
        {
            _lcd.Clear();
        }

        public void Home()
        {
            _lcd.Home();
        }

        public void Print(string text)
        {
            _lcd.Print(text);
        }

        public void SetCursor(byte col, byte row)
        {
            _lcd.SetCursor(col, row);
        }
    }
}
