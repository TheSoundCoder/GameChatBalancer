using System;

namespace AudioControl
{
    internal interface ISoftwareControlHotkeyService : IDisposable
    {
        void UpdateConnectionState(bool connected);
    }
}
