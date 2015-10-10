using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface ILcd2x16Driver : IDriver
    {
        void Home();
        void Clear();
        void Print(string text);
        void Backlight(bool on);
        void SetCursor(byte col, byte row);
    }
}
