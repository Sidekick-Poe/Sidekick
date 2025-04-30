using System.Runtime.InteropServices;

namespace Sidekick.Common.Platform.Linux.DllImport;

public static class LibX11
{
    [DllImport("libX11.so.6")]
    public static extern IntPtr XOpenDisplay(string? displayName);

    [DllImport("libX11.so.6")]
    public static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6")]
    public static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libX11.so.6")]
    public static extern int XGetInputFocus(IntPtr display, out IntPtr focus, out int revertTo);

    [DllImport("libX11.so.6")]
    public static extern int XGetWMName(IntPtr display, IntPtr window, ref XTextProperty textProperty);

    [DllImport("libX11.so.6")]
    public static extern void XFree(IntPtr data);

    [DllImport("libX11.so.6")]
    public static extern IntPtr XInternAtom(IntPtr display, string atomName, bool onlyIfExists);

    [DllImport("libX11.so.6")]
    public static extern int XGetWindowProperty(
        IntPtr display,
        IntPtr window,
        IntPtr property,
        long longOffset,
        long longLength,
        bool delete,
        IntPtr reqType,
        out IntPtr actualType,
        out int actualFormat,
        out ulong nitems,
        out ulong bytesAfter,
        out IntPtr prop);
}
