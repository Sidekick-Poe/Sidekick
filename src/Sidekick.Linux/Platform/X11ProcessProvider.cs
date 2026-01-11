using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform;

namespace Sidekick.Linux.Platform;

public class X11ProcessProvider(ILogger<X11ProcessProvider> logger) : IProcessProvider, IDisposable
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMilliseconds(250);
    private static readonly TimeSpan LogInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    private readonly object cacheLock = new();

    private DateTimeOffset lastTitleRead;

    private string? lastTitle;

    private DateTimeOffset lastLogTime;

    private string? lastLoggedTitle;

    private Timer? pollTimer;

    private bool hasInitialized;

    private bool x11Unavailable;

    public string? ClientLogPath => null;

    public bool IsPathOfExileInFocus => IsActiveWindowMatch(
        "Path of Exile",
        "Path of Exile 2",
        "Path of Exiles",
        "Path of Exiles 2");

    public bool IsSidekickInFocus => IsActiveWindowMatch("Sidekick");

    public int Priority => 0;

    public Task Initialize()
    {
        if (hasInitialized)
        {
            return Task.CompletedTask;
        }

        hasInitialized = true;
        pollTimer = new Timer(_ => GetActiveWindowTitle(), null, PollInterval, PollInterval);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        pollTimer?.Dispose();
        pollTimer = null;
    }

    private bool IsActiveWindowMatch(params string[] titles)
    {
        var title = GetActiveWindowTitle();
        if (string.IsNullOrEmpty(title))
        {
            return false;
        }

        return titles.Any(candidate => title.Contains(candidate, StringComparison.OrdinalIgnoreCase));
    }

    private string? GetActiveWindowTitle()
    {
        lock (cacheLock)
        {
            if (x11Unavailable)
            {
                return null;
            }

            if (DateTimeOffset.UtcNow - lastTitleRead < CacheDuration)
            {
                return lastTitle;
            }

            lastTitleRead = DateTimeOffset.UtcNow;
            try
            {
                lastTitle = FetchActiveWindowTitle();
            }
            catch (DllNotFoundException)
            {
                x11Unavailable = true;
                logger.LogWarning("[Linux/X11] libX11 not available; focus detection disabled.");
                lastTitle = null;
            }
            catch (EntryPointNotFoundException)
            {
                x11Unavailable = true;
                logger.LogWarning("[Linux/X11] libX11 entry points missing; focus detection disabled.");
                lastTitle = null;
            }
            LogActiveWindowTitle();
            return lastTitle;
        }
    }

    private void LogActiveWindowTitle()
    {
        if (DateTimeOffset.UtcNow - lastLogTime < LogInterval)
        {
            return;
        }

        lastLogTime = DateTimeOffset.UtcNow;
        if (string.Equals(lastLoggedTitle, lastTitle, StringComparison.Ordinal))
        {
            return;
        }

        lastLoggedTitle = lastTitle;
        logger.LogDebug("[Linux/X11] Active window title: {Title}", lastTitle ?? "<none>");
    }

    private static string? FetchActiveWindowTitle()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        {
            return null;
        }

        var display = XOpenDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            var rootWindow = XDefaultRootWindow(display);
            var activeAtom = XInternAtom(display, "_NET_ACTIVE_WINDOW", true);
            if (activeAtom == IntPtr.Zero)
            {
                return null;
            }

            var status = XGetWindowProperty(
                display,
                rootWindow,
                activeAtom,
                IntPtr.Zero,
                new IntPtr(1024),
                false,
                IntPtr.Zero,
                out _,
                out _,
                out var nitems,
                out _,
                out var prop);

            if (status != Success || prop == IntPtr.Zero || nitems == UIntPtr.Zero)
            {
                if (prop != IntPtr.Zero)
                {
                    XFree(prop);
                }

                return null;
            }

            var window = ReadWindowId(prop);
            XFree(prop);
            if (window == IntPtr.Zero)
            {
                return null;
            }

            return FetchWindowTitle(display, window);
        }
        finally
        {
            XCloseDisplay(display);
        }
    }

    private static IntPtr ReadWindowId(IntPtr data)
    {
        if (data == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        if (IntPtr.Size == 8)
        {
            var windowId = Marshal.ReadInt32(data);
            return new IntPtr(windowId);
        }

        return Marshal.ReadIntPtr(data);
    }

    private static string? FetchWindowTitle(IntPtr display, IntPtr window)
    {
        var nameAtom = XInternAtom(display, "_NET_WM_NAME", true);
        var utf8Atom = XInternAtom(display, "UTF8_STRING", true);

        if (nameAtom != IntPtr.Zero && utf8Atom != IntPtr.Zero)
        {
            var status = XGetWindowProperty(
                display,
                window,
                nameAtom,
                IntPtr.Zero,
                new IntPtr(1024),
                false,
                utf8Atom,
                out _,
                out _,
                out var nitems,
                out _,
                out var prop);

            if (status == Success && prop != IntPtr.Zero && nitems != UIntPtr.Zero)
            {
                try
                {
                    var title = Marshal.PtrToStringUTF8(prop);
                    if (!string.IsNullOrEmpty(title))
                    {
                        return title;
                    }
                }
                finally
                {
                    XFree(prop);
                }
            }
            else if (prop != IntPtr.Zero)
            {
                XFree(prop);
            }
        }

        return FetchWindowTitleFallback(display, window);
    }

    private static string? FetchWindowTitleFallback(IntPtr display, IntPtr window)
    {
        var status = XFetchName(display, window, out var namePtr);
        if (status == 0 || namePtr == IntPtr.Zero)
        {
            return null;
        }

        try
        {
            return Marshal.PtrToStringAnsi(namePtr);
        }
        finally
        {
            XFree(namePtr);
        }
    }

    private const int Success = 0;

    [DllImport("libX11.so.6", EntryPoint = "XOpenDisplay")]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XCloseDisplay")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XDefaultRootWindow")]
    private static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XInternAtom")]
    private static extern IntPtr XInternAtom(IntPtr display, string atomName, bool onlyIfExists);

    [DllImport("libX11.so.6", EntryPoint = "XGetWindowProperty")]
    private static extern int XGetWindowProperty(
        IntPtr display,
        IntPtr window,
        IntPtr property,
        IntPtr longOffset,
        IntPtr longLength,
        bool delete,
        IntPtr reqType,
        out IntPtr actualType,
        out int actualFormat,
        out UIntPtr nitems,
        out UIntPtr bytesAfter,
        out IntPtr prop);

    [DllImport("libX11.so.6", EntryPoint = "XFetchName")]
    private static extern int XFetchName(IntPtr display, IntPtr window, out IntPtr windowName);

    [DllImport("libX11.so.6", EntryPoint = "XFree")]
    private static extern int XFree(IntPtr data);
}
