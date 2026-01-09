using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;

namespace Sidekick.Linux.Platform;

public sealed class LinuxPoeWindowWatcher(
    IApplicationService applicationService,
    ILogger<LinuxPoeWindowWatcher> logger) : IInitializableService, IDisposable
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MissingThreshold = TimeSpan.FromSeconds(6);
    private static readonly string[] WindowTitles =
    [
        "Path of Exile",
        "Path of Exile 2",
    ];

    private Timer? timer;
    private bool inTick;
    private bool shutdownRequested;
    private DateTimeOffset? lastSeen;
    private string? armedWindowClass;
    private bool armed;
    private bool x11Unavailable;

    public int Priority => 130;

    public Task Initialize()
    {
        if (timer != null)
        {
            return Task.CompletedTask;
        }

        timer = new Timer(_ => Tick(), null, TimeSpan.Zero, PollInterval);
        return Task.CompletedTask;
    }

    private void Tick()
    {
        if (inTick || shutdownRequested || x11Unavailable)
        {
            return;
        }

        inTick = true;
        try
        {
            if (!TryOpenDisplay(out var display))
            {
                return;
            }

            try
            {
                if (!armed)
                {
                    if (TryGetActiveWindow(display, out var activeWindow)
                        && TryGetWindowTitle(display, activeWindow, out var title)
                        && IsExactPoeTitle(title)
                        && TryGetWindowClass(display, activeWindow, out var windowClass))
                    {
                        armedWindowClass = windowClass;
                        lastSeen = DateTimeOffset.UtcNow;
                        armed = true;
                    }

                    return;
                }

                if (string.IsNullOrEmpty(armedWindowClass))
                {
                    armed = false;
                    lastSeen = null;
                    return;
                }

                if (IsWindowClassPresent(display, armedWindowClass, out var present) && present)
                {
                    lastSeen = DateTimeOffset.UtcNow;
                    return;
                }

                if (lastSeen == null)
                {
                    return;
                }

                if (DateTimeOffset.UtcNow - lastSeen.Value < MissingThreshold)
                {
                    return;
                }

                shutdownRequested = true;
                logger.LogInformation("[Linux] PoE window class closed; shutting down Sidekick.");
                applicationService.Shutdown();
            }
            finally
            {
                XCloseDisplay(display);
            }
        }
        finally
        {
            inTick = false;
        }
    }

    private bool TryOpenDisplay(out IntPtr display)
    {
        display = IntPtr.Zero;

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        {
            x11Unavailable = true;
            logger.LogWarning("[Linux/X11] DISPLAY not set; window watcher disabled.");
            return false;
        }

        try
        {
            display = XOpenDisplay(IntPtr.Zero);
        }
        catch (DllNotFoundException)
        {
            x11Unavailable = true;
            logger.LogWarning("[Linux/X11] libX11 not available; window watcher disabled.");
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            x11Unavailable = true;
            logger.LogWarning("[Linux/X11] libX11 entry points missing; window watcher disabled.");
            return false;
        }

        if (display == IntPtr.Zero)
        {
            x11Unavailable = true;
            logger.LogWarning("[Linux/X11] XOpenDisplay failed; window watcher disabled.");
            return false;
        }

        return true;
    }

    private bool TryGetActiveWindow(IntPtr display, out IntPtr window)
    {
        window = IntPtr.Zero;
        var root = XDefaultRootWindow(display);
        var activeAtom = XInternAtom(display, "_NET_ACTIVE_WINDOW", true);
        if (activeAtom == IntPtr.Zero)
        {
            return false;
        }

        var status = XGetWindowProperty(
            display,
            root,
            activeAtom,
            IntPtr.Zero,
            new IntPtr(4),
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

            return false;
        }

        try
        {
            window = IntPtr.Size == 8
                ? new IntPtr(Marshal.ReadInt64(prop))
                : new IntPtr(Marshal.ReadInt32(prop));
            return window != IntPtr.Zero;
        }
        finally
        {
            XFree(prop);
        }
    }

    private bool TryGetWindowTitle(IntPtr display, IntPtr window, out string title)
    {
        title = string.Empty;
        var resolved = FetchWindowTitle(display, window);
        if (string.IsNullOrEmpty(resolved))
        {
            return false;
        }

        title = resolved;
        return true;
    }

    private static bool IsExactPoeTitle(string title)
    {
        return WindowTitles.Any(candidate => string.Equals(candidate, title, StringComparison.OrdinalIgnoreCase));
    }

    private bool TryGetWindowClass(IntPtr display, IntPtr window, out string windowClass)
    {
        windowClass = string.Empty;
        var classAtom = XInternAtom(display, "WM_CLASS", true);
        if (classAtom == IntPtr.Zero)
        {
            return false;
        }

        var status = XGetWindowProperty(
            display,
            window,
            classAtom,
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

            return false;
        }

        try
        {
            var length = (int)nitems;
            if (length <= 0)
            {
                return false;
            }

            var bytes = new byte[length];
            Marshal.Copy(prop, bytes, 0, bytes.Length);
            var parts = Encoding.UTF8.GetString(bytes).Split('\0', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return false;
            }

            windowClass = parts.Length > 1 ? parts[1] : parts[0];
            return !string.IsNullOrWhiteSpace(windowClass);
        }
        finally
        {
            XFree(prop);
        }
    }

    private bool IsWindowClassPresent(IntPtr display, string windowClass, out bool present)
    {
        present = false;
        var root = XDefaultRootWindow(display);
        var clientList = XInternAtom(display, "_NET_CLIENT_LIST", true);
        if (clientList == IntPtr.Zero)
        {
            return true;
        }

        var status = XGetWindowProperty(
            display,
            root,
            clientList,
            IntPtr.Zero,
            new IntPtr(4096),
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

            return true;
        }

        try
        {
            var count = (int)nitems;
            for (var i = 0; i < count; i++)
            {
                var window = IntPtr.Size == 8
                    ? new IntPtr(Marshal.ReadInt64(prop, i * IntPtr.Size))
                    : new IntPtr(Marshal.ReadInt32(prop, i * IntPtr.Size));
                if (window == IntPtr.Zero)
                {
                    continue;
                }

                if (!TryGetWindowClass(display, window, out var candidateClass))
                {
                    continue;
                }

                if (string.Equals(candidateClass, windowClass, StringComparison.OrdinalIgnoreCase))
                {
                    present = true;
                    return true;
                }
            }
        }
        finally
        {
            XFree(prop);
        }

        return true;
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
                var title = Marshal.PtrToStringUTF8(prop);
                XFree(prop);
                if (!string.IsNullOrEmpty(title))
                {
                    return title;
                }
            }

            if (prop != IntPtr.Zero)
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

    public void Dispose()
    {
        timer?.Dispose();
        timer = null;
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
