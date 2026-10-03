using System.Drawing;
using System.Windows.Forms;

namespace AudioControl
{
    internal interface ITrayHostService
    {
        void HandleTrayMouseUp(MouseButtons button, Point anchorPoint);
        void RefreshState();
        void SetConnectedState(bool connected, string? currentPort);
        void SetInvertControlState(bool invert);
        void SetBalance(float displayVolume, float gamePercent, float chatPercent);
        void Close();
    }
}
