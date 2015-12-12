using System;
using Microsoft.SPOT;

namespace Verdant.Node.Common
{
    public interface ICharacterLcdDriver : IDriver
    {
        int Rows { get; }
        int Columns { get; }

        void Home();
        void Clear();
        void Print(string text);
        void Backlight(bool on);
        void SetCursor(byte col, byte row);
    }
}
