using System.Runtime.InteropServices;

namespace Sidekick.Common.Platform.Windows.DllImport;

[StructLayout(LayoutKind.Sequential)]
internal struct WinMessage
{
    public IntPtr Hwnd;
    public uint Message;
    public IntPtr WParam;
    public IntPtr LParam;
    public uint Time;
    public System.Drawing.Point Point;
}
