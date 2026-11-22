using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Sidekick.Common.Platform.Linux.X11;

/// <summary>
/// The parts of libX11 and libc used to follow the active window and grab hotkeys. Window, Atom, KeySym and Time are
/// C longs in Xlib, so they are nint here (Sidekick only runs on 64-bit Linux).
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
internal static class Xlib
{
    private const string LibX11 = "libX11.so.6";

    public const int KeyPress = 2;
    public const int ButtonPress = 4;
    public const int PropertyNotify = 28;

    public const long PropertyChangeMask = 1L << 22;
    public const uint ButtonPressMask = 1 << 2;
    public const uint ButtonReleaseMask = 1 << 3;

    public const uint ShiftMask = 1 << 0;
    public const uint LockMask = 1 << 1;
    public const uint ControlMask = 1 << 2;
    public const uint Mod1Mask = 1 << 3;
    public const uint Mod2Mask = 1 << 4;
    public const uint Mod4Mask = 1 << 6;
    public const uint AnyModifier = 1 << 15;

    public const int AnyKey = 0;
    public const uint AnyButton = 0;
    public const uint Button4 = 4;
    public const uint Button5 = 5;

    public const int GrabModeSync = 0;
    public const int GrabModeAsync = 1;

    public const int AsyncPointer = 0;
    public const int ReplayPointer = 2;
    public const int AsyncKeyboard = 3;
    public const int ReplayKeyboard = 5;

    /// <summary>
    /// The size of the XEvent union on 64-bit systems.
    /// </summary>
    public const int EventSize = 192;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int ErrorHandler(nint display, nint errorEvent);

    [DllImport(LibX11)]
    public static extern nint XOpenDisplay(nint displayName);

    [DllImport(LibX11)]
    public static extern int XCloseDisplay(nint display);

    [DllImport(LibX11)]
    public static extern nint XDefaultRootWindow(nint display);

    [DllImport(LibX11)]
    public static extern int XConnectionNumber(nint display);

    [DllImport(LibX11)]
    public static extern nint XInternAtom(nint display, string atomName, bool onlyIfExists);

    [DllImport(LibX11)]
    public static extern int XSelectInput(nint display, nint window, nint eventMask);

    [DllImport(LibX11)]
    public static extern int XGetWindowProperty(nint display, nint window, nint property, nint longOffset, nint longLength, bool delete, nint reqType, out nint actualType, out int actualFormat, out nint itemCount, out nint bytesAfter, out nint data);

    [DllImport(LibX11)]
    public static extern int XGetClassHint(nint display, nint window, out XClassHint classHint);

    [DllImport(LibX11)]
    public static extern int XFree(nint data);

    /// <summary>
    /// The predefined WM_TRANSIENT_FOR atom.
    /// </summary>
    public const nint WmTransientForAtom = 68;

    [DllImport(LibX11)]
    public static extern int XSetTransientForHint(nint display, nint window, nint propWindow);

    [DllImport(LibX11)]
    public static extern int XDeleteProperty(nint display, nint window, nint property);

    [DllImport(LibX11)]
    public static extern byte XKeysymToKeycode(nint display, nint keysym);

    [DllImport(LibX11)]
    public static extern int XGrabKey(nint display, int keycode, uint modifiers, nint grabWindow, bool ownerEvents, int pointerMode, int keyboardMode);

    [DllImport(LibX11)]
    public static extern int XUngrabKey(nint display, int keycode, uint modifiers, nint grabWindow);

    [DllImport(LibX11)]
    public static extern int XGrabButton(nint display, uint button, uint modifiers, nint grabWindow, bool ownerEvents, uint eventMask, int pointerMode, int keyboardMode, nint confineTo, nint cursor);

    [DllImport(LibX11)]
    public static extern int XUngrabButton(nint display, uint button, uint modifiers, nint grabWindow);

    [DllImport(LibX11)]
    public static extern int XUngrabKeyboard(nint display, nint time);

    [DllImport(LibX11)]
    public static extern int XAllowEvents(nint display, int eventMode, nint time);

    [DllImport(LibX11)]
    public static extern int XPending(nint display);

    [DllImport(LibX11)]
    public static extern int XNextEvent(nint display, nint eventReturn);

    [DllImport(LibX11)]
    public static extern int XSync(nint display, bool discard);

    [DllImport(LibX11)]
    public static extern nint XSetErrorHandler(ErrorHandler? handler);

    [DllImport("libc", SetLastError = true)]
    public static extern int poll([In, Out] PollFd[] fds, nuint nfds, int timeout);

    [DllImport("libc", SetLastError = true)]
    public static extern int pipe([Out] int[] fds);

    [DllImport("libc", SetLastError = true)]
    public static extern nint read(int fd, byte[] buffer, nint count);

    [DllImport("libc", SetLastError = true)]
    public static extern nint write(int fd, byte[] buffer, nint count);

    [DllImport("libc", SetLastError = true)]
    public static extern int close(int fd);

    public const short POLLIN = 1;

    [StructLayout(LayoutKind.Sequential)]
    public struct PollFd
    {
        public int Fd;
        public short Events;
        public short Revents;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XClassHint
    {
        public nint ResName;
        public nint ResClass;
    }

    /// <summary>
    /// Offsets of the XEvent fields read by Sidekick, on 64-bit systems. Key and button events share their layout.
    /// </summary>
    public static class EventOffsets
    {
        public const int Type = 0;
        public const int PropertyAtom = 40;
        public const int InputState = 80;
        public const int InputKeycodeOrButton = 84;
    }

    /// <summary>
    /// Offsets of the XErrorEvent fields read by Sidekick, on 64-bit systems.
    /// </summary>
    public static class ErrorOffsets
    {
        public const int RequestCode = 33;
    }
}
