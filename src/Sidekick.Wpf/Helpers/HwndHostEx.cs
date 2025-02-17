using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Sidekick.Wpf.Helpers;

public class HwndHostEx(IntPtr handle) : HwndHost
{
    private const int GwlStyle = (-16);
    private const int WsChild = 0x40000000;

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        var href = new HandleRef();
        if (handle == IntPtr.Zero)
        {
            return href;
        }

        SetWindowLong(handle, GwlStyle, WsChild);
        SetParent(handle, hwndParent.Handle);
        href = new HandleRef(this, handle);

        return href;
    }

    protected override void DestroyWindowCore(HandleRef hwnd)
    {
    }
}
