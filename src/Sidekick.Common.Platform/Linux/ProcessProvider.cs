using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Linux.DllImport;

namespace Sidekick.Common.Platform.Linux.Processes;

public class ProcessProvider(ILogger<ProcessProvider> logger) : IProcessProvider
{
    private const string PATH_OF_EXILE_TITLE = "Path of Exile";
    private const string PATH_OF_EXILE_2_TITLE = "Path of Exile 2";
    private const string SIDEKICK_TITLE = "Sidekick";

    private static readonly List<string> PossibleProcessNames = new()
    {
        "PathOfExile",
        "PathOfExile_x64",
        "PathOfExile_KG",
        "PathOfExile_x64_KG",
        "PathOfExileSteam",
        "PathOfExile_x64Steam"
    };

    public string? ClientLogPath
    {
        get
        {
            var directory = Path.GetDirectoryName(GetPathOfExileProcess()?.MainModule?.FileName);
            if (directory == null)
            {
                return null;
            }

            return Path.Combine(directory, "logs", "Client.txt");
        }
    }

    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    public bool IsPathOfExileInFocus
    {
        get
        {
            var focusedWindow = GetFocusedWindow();
            return focusedWindow is PATH_OF_EXILE_TITLE or PATH_OF_EXILE_2_TITLE;
        }
    }

    /// <inheritdoc/>
    public bool IsSidekickInFocus
    {
        get
        {
            if (IsCurrentProcessFocused())
            {
                Console.WriteLine("The current process is focused.");
            }
            else
            {
                Console.WriteLine("The current process is not focused.");
            }

            return GetFocusedWindow()?.StartsWith(SIDEKICK_TITLE) ?? false;
        }
    }

    /// <inheritdoc/>
    public int Priority => 0;

    private DateTimeOffset PreviousFocusedWindowAttempt { get; set; }

    private string? PreviousFocusedWindow { get; set; }

    private string? GetFocusedWindow()
    {
        if (DateTimeOffset.Now - PreviousFocusedWindowAttempt < TimeSpan.FromSeconds(3))
        {
            return PreviousFocusedWindow;
        }

        try
        {
            // Open a connection to the X server
            IntPtr display = LibX11.XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                logger.LogWarning("Unable to open X display.");
                return null;
            }

            // Get the currently focused window
            IntPtr focusedWindow;
            int revertTo;
            LibX11.XGetInputFocus(display, out focusedWindow, out revertTo);

            if (focusedWindow == IntPtr.Zero)
            {
                LibX11.XCloseDisplay(display);
                return null;
            }

            // Try to get the WM_NAME property
            XTextProperty windowNameProperty = new();
            if (LibX11.XGetWMName(display, focusedWindow, ref windowNameProperty) != 0 && windowNameProperty.value != IntPtr.Zero)
            {
                string windowName = Marshal.PtrToStringAnsi(windowNameProperty.value) ?? string.Empty;
                LibX11.XFree(windowNameProperty.value);
                LibX11.XCloseDisplay(display);

                // Cache the result
                PreviousFocusedWindow = windowName;
                PreviousFocusedWindowAttempt = DateTimeOffset.Now;

                return windowName;
            }

            // Fallback to _NET_WM_NAME
            IntPtr atomNetWmName = LibX11.XInternAtom(display, "_NET_WM_NAME", false);
            IntPtr actualType;
            int actualFormat;
            ulong nitems, bytesAfter;
            IntPtr prop;

            if (LibX11.XGetWindowProperty(display, focusedWindow, atomNetWmName, 0, 1024, false, (IntPtr)31, out actualType, out actualFormat, out nitems, out bytesAfter, out prop) == 0 && prop != IntPtr.Zero)
            {
                string windowName = Marshal.PtrToStringAnsi(prop) ?? string.Empty;
                LibX11.XFree(prop);
                LibX11.XCloseDisplay(display);

                // Cache the result
                PreviousFocusedWindow = windowName;
                PreviousFocusedWindowAttempt = DateTimeOffset.Now;

                return windowName;
            }

            LibX11.XCloseDisplay(display);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[ProcessProvider] Failed to grab the focused window: {0}", ex.Message);
            PreviousFocusedWindow = null;
            return null;
        }
    }

    private bool IsCurrentProcessFocused()
    {
        try
        {
            // Open a connection to the X server
            IntPtr display = LibX11.XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                logger.LogWarning("Unable to open X display.");
                return false;
            }

            // Get the currently focused window
            IntPtr focusedWindow;
            int revertTo;
            LibX11.XGetInputFocus(display, out focusedWindow, out revertTo);

            if (focusedWindow == IntPtr.Zero)
            {
                LibX11.XCloseDisplay(display);
                return false;
            }

            // Get the _NET_WM_PID property
            IntPtr atomNetWmPid = LibX11.XInternAtom(display, "_NET_WM_PID", false);
            IntPtr actualType;
            int actualFormat;
            ulong nitems, bytesAfter;
            IntPtr prop;

            if (LibX11.XGetWindowProperty(display, focusedWindow, atomNetWmPid, 0, 1, false, (IntPtr)6, // 6 = XA_CARDINAL
                out actualType, out actualFormat, out nitems, out bytesAfter, out prop) == 0 && prop != IntPtr.Zero)
            {
                // Read the process ID from the property
                int windowPid = Marshal.ReadInt32(prop);
                LibX11.XFree(prop);

                // Compare with the current process ID
                int currentPid = Process.GetCurrentProcess().Id;
                LibX11.XCloseDisplay(display);

                return windowPid == currentPid;
            }

            LibX11.XCloseDisplay(display);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[ProcessProvider] Failed to determine if the current process is focused: {0}", ex.Message);
            return false;
        }
    }

    private static Process? GetPathOfExileProcess()
    {
        foreach (var processName in PossibleProcessNames)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (process != null)
            {
                return process;
            }
        }

        return null;
    }
}
