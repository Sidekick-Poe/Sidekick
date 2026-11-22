using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Debugging;
using Sidekick.Common.Platform.Linux.X11;

namespace Sidekick.Common.Platform.Linux;

/// <summary>
/// Finds out whether the game or Sidekick has focus, from the active X11 window. Path of Exile runs under Proton as an
/// X11 window (through XWayland on Wayland sessions).
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
public class ProcessProvider(X11Connection x11, DebugService debugService, ILogger<ProcessProvider> logger) : IProcessProvider
{
    /// <summary>
    /// The window class Wine gives to the game windows: the lowercase name of the executable, like
    /// "pathofexile_x64steam.exe". Matches the process names checked on Windows.
    /// </summary>
    private const string GameWindowClassPrefix = "pathofexile";

    /// <summary>
    /// The window classes Steam's Proton gives to the games instead, from their Steam app ids.
    /// </summary>
    private static readonly HashSet<string> SteamGameWindowClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "steam_app_238960",
        "steam_app_2694490",
    };

    private static readonly HashSet<string> GameWindowTitles = new(StringComparer.Ordinal)
    {
        "Path of Exile",
        "Path of Exile 2",
    };

    private volatile bool isActiveWindowKnown;

    private volatile bool isSidekickActive;

    /// <summary>
    /// Raised when the game gains or loses focus, on the X11 connection's thread.
    /// </summary>
    public event Action? GameFocusChanged;

    /// <summary>
    /// Gets the game window while it has focus, or 0.
    /// </summary>
    public nint GameWindow { get; private set; }

    public string? ClientLogPath
    {
        get
        {
            return null;
        }
    }

    public Task Initialize()
    {
        x11.ActiveWindowChanged += OnActiveWindowChanged;
        x11.Start();
        OnActiveWindowChanged(x11.ActiveWindow);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Without X11, or while a native Wayland window has focus (like the game in Proton's Wayland mode), Sidekick can't
    /// tell which window has focus, so it assumes the game does to keep the hotkeys working.
    /// </summary>
    public bool IsPathOfExileInFocus
    {
        get
        {
            if (debugService.HotkeysInAnyWindow || !x11.IsAvailable || !isActiveWindowKnown)
            {
                return true;
            }

            return GameWindow != 0;
        }
    }

    /// <inheritdoc/>
    public bool IsSidekickInFocus
    {
        get
        {
            if (!x11.IsAvailable)
            {
                return true;
            }

            return isSidekickActive;
        }
    }

    private void OnActiveWindowChanged(nint window)
    {
        isActiveWindowKnown = window != 0;
        isSidekickActive = window != 0 && x11.GetWindowProcessId(window) == Environment.ProcessId;

        var gameWindow = window != 0 && IsGameWindow(window) ? window : 0;
        if (gameWindow == GameWindow)
        {
            return;
        }

        GameWindow = gameWindow;
        logger.LogDebug("[ProcessProvider] Path of Exile focused: {Focused}.", gameWindow != 0);
        GameFocusChanged?.Invoke();
    }

    /// <summary>
    /// The game is found by its window class (the executable name, or the Steam app id under Steam's Proton), or by
    /// its exact window title.
    /// </summary>
    private bool IsGameWindow(nint window)
    {
        var (name, windowClass) = x11.GetWindowClass(window);
        if (IsGameWindowClass(name) || IsGameWindowClass(windowClass))
        {
            return true;
        }

        var title = x11.GetWindowTitle(window);
        return title != null && GameWindowTitles.Contains(title);
    }

    private static bool IsGameWindowClass(string? windowClass)
    {
        if (windowClass == null)
        {
            return false;
        }

        return windowClass.StartsWith(GameWindowClassPrefix, StringComparison.OrdinalIgnoreCase) || SteamGameWindowClasses.Contains(windowClass);
    }
}
