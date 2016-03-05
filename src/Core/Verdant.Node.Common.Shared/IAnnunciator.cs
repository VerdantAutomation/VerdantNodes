using System;
using System.Text;

namespace Verdant.Node.Common
{
    public enum AnnunciatorState
    {
        Off = 0,
        On = 1,
        Slow = 2,   // Normal operation
        Medium = 3, // Initialization
        Fast = 4,   // Hard error
    }

    public interface IAnnunciator
    {
        void SetAnnunciatorState(AnnunciatorState state);
        AnnunciatorState CurrentState { get; }
    }
}
