using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Platform.Input;

namespace Sidekick.Common.Platform.Linux.X11;

/// <summary>
/// Follows the active X11 window and, while Path of Exile has focus, grabs Sidekick's hotkeys so the game does not
/// receive them. Path of Exile runs under Proton as an X11 window (through XWayland on Wayland), so this works on X11
/// and Wayland sessions alike. Grabs use the synchronous mode: the X server waits for Sidekick to say whether a hotkey
/// was used, and gives the key back to the game when no handler wanted it (for example Esc with no overlay open).
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
public sealed class X11Session(ILogger<X11Session> logger) : IHotkeyBlocker, IDisposable
{
    /// <summary>
    /// X11 request codes, to tell grab failures apart from other errors.
    /// </summary>
    private const byte GrabButtonRequest = 28;
    private const byte GrabKeyRequest = 33;

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

    /// <summary>
    /// The predefined WM_NAME atom, the window title in Latin-1.
    /// </summary>
    private const nint WmNameAtom = 39;

    /// <summary>
    /// How long a grabbed key waits for Sidekick to decide whether a hotkey uses it. The keyboard is frozen meanwhile,
    /// and some checks ask the UI thread (is the overlay open?), which can be busy. Past this, the key goes to the game.
    /// </summary>
    private static readonly TimeSpan GrabDecisionTimeout = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// How long grabs stay off after Sidekick simulates keys, so the X server handles the simulated keys before the
    /// grabs come back. A simulated hotkey (like the Find item hotkey sent again after a failed copy) would otherwise
    /// be grabbed by Sidekick instead of reaching the game.
    /// </summary>
    private static readonly TimeSpan SimulatedInputDelay = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Lock state, NumLock and their combination change the modifier state of a key press, so every grab is made once
    /// for each of them.
    /// </summary>
    private static readonly uint[] IgnoredModifiers = [0, Xlib.LockMask, Xlib.Mod2Mask, Xlib.LockMask | Xlib.Mod2Mask];

    private const uint HotkeyModifiers = Xlib.ShiftMask | Xlib.ControlMask | Xlib.Mod1Mask | Xlib.Mod4Mask;

    /// <summary>
    /// Xlib has one error handler per process, shared with Avalonia. Kept in a static field so the delegate is never
    /// collected while Xlib holds it.
    /// </summary>
    private static Xlib.ErrorHandler? errorHandler;

    private static Xlib.ErrorHandler? previousErrorHandler;

    private static X11Session? errorHandlerOwner;

    private readonly Lock sync = new();

    private readonly Dictionary<(int Keycode, uint Modifiers), string> grabbedKeys = [];

    private nint display;

    private nint root;

    private nint activeWindowAtom;

    private nint pidAtom;

    private nint netWmNameAtom;

    private readonly int[] wakePipe = [-1, -1];

    private Thread? thread;

    private volatile bool disposed;

    /// <summary>
    /// Set when the event loop stopped on an error. Nothing would answer a grabbed key then, so nothing is grabbed again.
    /// </summary>
    private volatile bool loopStopped;

    private int grabErrors;

    private int suspendCount;

    private Func<IEnumerable<string>>? getKeybinds;

    private Func<string, bool>? isKeybindUsed;

    private Func<bool>? isScrollUsed;

    /// <summary>
    /// Gets a value indicating whether Sidekick is connected to an X11 display.
    /// </summary>
    public bool IsAvailable => display != 0;

    /// <summary>
    /// Gets a value indicating whether an X11 window has focus. When a native Wayland window has focus, X11 has no
    /// active window, so Sidekick can't tell which program it is.
    /// </summary>
    public bool IsActiveWindowKnown { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the active window is Path of Exile.
    /// </summary>
    public bool IsPathOfExileActive { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the active window is one of Sidekick's windows.
    /// </summary>
    public bool IsSidekickActive { get; private set; }

    /// <summary>
    /// Connects to the X11 display and starts following the active window. Without a display (no X11 or XWayland),
    /// Sidekick keeps working without grabs and focus checks.
    /// </summary>
    public void Start()
    {
        lock (sync)
        {
            if (display != 0 || disposed)
            {
                return;
            }

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
            {
                logger.LogWarning("[X11] No DISPLAY, hotkeys can't be grabbed and the game focus can't be checked.");
                return;
            }

            try
            {
                display = Xlib.XOpenDisplay(0);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "[X11] libX11 could not be loaded.");
                return;
            }

            if (display == 0)
            {
                logger.LogWarning("[X11] Could not connect to the X11 display.");
                return;
            }

            if (Xlib.pipe(wakePipe) != 0)
            {
                logger.LogWarning("[X11] Could not create the wake pipe.");
                Xlib.XCloseDisplay(display);
                display = 0;
                return;
            }

            InstallErrorHandler();

            root = Xlib.XDefaultRootWindow(display);
            activeWindowAtom = Xlib.XInternAtom(display, "_NET_ACTIVE_WINDOW", false);
            pidAtom = Xlib.XInternAtom(display, "_NET_WM_PID", false);
            netWmNameAtom = Xlib.XInternAtom(display, "_NET_WM_NAME", false);
            Xlib.XSelectInput(display, root, (nint)Xlib.PropertyChangeMask);
            UpdateActiveWindow();
            Xlib.XSync(display, false);
        }

        thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "X11 hotkeys",
        };
        thread.Start();
        logger.LogInformation("[X11] Following the active window.");
    }

    /// <inheritdoc/>
    public void SetSource(Func<IEnumerable<string>> keybinds, Func<string, bool> isKeybindUsed, Func<bool> isScrollUsed)
    {
        lock (sync)
        {
            getKeybinds = keybinds;
            this.isKeybindUsed = isKeybindUsed;
            this.isScrollUsed = isScrollUsed;
            ApplyGrabs();
        }

        Wake();
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        lock (sync)
        {
            ApplyGrabs();
        }

        Wake();
    }

    /// <summary>
    /// Ends the grab that is active while the user still holds the hotkey, which would otherwise catch the simulated
    /// keys. The grabs themselves are only released when a simulated key is grabbed (like the Find item hotkey sent
    /// again after a failed copy), so other input, like the next Ctrl+scroll tick, stays blocked meanwhile.
    /// </summary>
    public bool Suspend(IReadOnlyCollection<string> keyStrokes)
    {
        lock (sync)
        {
            if (display == 0 || loopStopped)
            {
                return false;
            }

            Xlib.XUngrabKeyboard(display, 0);
            var release = keyStrokes.Any(IsGrabbed);
            if (release)
            {
                suspendCount++;
                UngrabAll();
            }

            Xlib.XSync(display, false);
            return release;
        }
    }

    private bool IsGrabbed(string keyStroke)
    {
        if (!X11KeySymbols.TryParse(keyStroke, out var keySymbol, out var modifiers))
        {
            return false;
        }

        var keycode = Xlib.XKeysymToKeycode(display, keySymbol);
        return keycode != 0 && grabbedKeys.ContainsKey((keycode, modifiers));
    }

    /// <inheritdoc/>
    public void Resume()
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(SimulatedInputDelay);
            lock (sync)
            {
                if (display == 0 || suspendCount == 0)
                {
                    return;
                }

                suspendCount--;
                ApplyGrabs();
            }

            Wake();
        });
    }

    private void Run()
    {
        var fds = new Xlib.PollFd[]
        {
            new()
            {
                Fd = Xlib.XConnectionNumber(display),
                Events = Xlib.POLLIN,
            },
            new()
            {
                Fd = wakePipe[0],
                Events = Xlib.POLLIN,
            },
        };
        var wakeBuffer = new byte[64];
        var eventBuffer = Marshal.AllocHGlobal(Xlib.EventSize);
        try
        {
            while (!disposed)
            {
                lock (sync)
                {
                    while (!disposed && Xlib.XPending(display) > 0)
                    {
                        Xlib.XNextEvent(display, eventBuffer);
                        try
                        {
                            HandleEvent(eventBuffer);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "[X11] Could not handle an event.");
                        }
                    }
                }

                fds[0].Revents = 0;
                fds[1].Revents = 0;
                Xlib.poll(fds, 2, -1);
                if ((fds[1].Revents & Xlib.POLLIN) != 0)
                {
                    Xlib.read(wakePipe[0], wakeBuffer, wakeBuffer.Length);
                }
            }
        }
        catch (Exception ex)
        {
            // Without the loop, nobody answers the synchronous grabs, which would freeze the keyboard.
            logger.LogError(ex, "[X11] The event loop stopped, releasing the hotkeys.");
            lock (sync)
            {
                loopStopped = true;
                UngrabAll();
                Xlib.XSync(display, false);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(eventBuffer);
        }
    }

    private void HandleEvent(nint xEvent)
    {
        switch (Marshal.ReadInt32(xEvent, Xlib.EventOffsets.Type))
        {
            case Xlib.PropertyNotify:
                if (Marshal.ReadIntPtr(xEvent, Xlib.EventOffsets.PropertyAtom) == activeWindowAtom)
                {
                    UpdateActiveWindow();
                }

                break;

            case Xlib.KeyPress:
                OnGrabbedKey(xEvent);
                break;

            case Xlib.ButtonPress:
                OnGrabbedScroll();
                break;
        }
    }

    /// <summary>
    /// Decides what happens to a grabbed key press: kept from the game when a handler uses it, given back otherwise.
    /// The handler itself runs from the global hook.
    /// </summary>
    private void OnGrabbedKey(nint xEvent)
    {
        var keycode = Marshal.ReadInt32(xEvent, Xlib.EventOffsets.InputKeycodeOrButton);
        var modifiers = (uint)Marshal.ReadInt32(xEvent, Xlib.EventOffsets.InputState) & HotkeyModifiers;
        var check = isKeybindUsed;

        var used = check != null
                   && grabbedKeys.TryGetValue((keycode, modifiers), out var keybind)
                   && Decide(() => check(keybind), keybind);
        Xlib.XAllowEvents(display, used ? Xlib.AsyncKeyboard : Xlib.ReplayKeyboard, 0);
    }

    private void OnGrabbedScroll()
    {
        var check = isScrollUsed;
        var used = check != null && Decide(check, "Ctrl+scroll");
        Xlib.XAllowEvents(display, used ? Xlib.AsyncPointer : Xlib.ReplayPointer, 0);
    }

    /// <summary>
    /// Runs a check for a grabbed input within <see cref="GrabDecisionTimeout"/>, since the input stays frozen until
    /// Sidekick answers. A check that fails or is too slow counts as unused, so the input goes to the game.
    /// </summary>
    private bool Decide(Func<bool> check, string input)
    {
        try
        {
            var task = Task.Run(check);
            if (task.Wait(GrabDecisionTimeout))
            {
                return task.Result;
            }

            logger.LogWarning("[X11] Checking {Input} took too long, it goes to the game.", input);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[X11] Could not check {Input}.", input);
        }

        return false;
    }

    private void UpdateActiveWindow()
    {
        var window = ReadWindowProperty(root, activeWindowAtom);
        var wasGame = IsPathOfExileActive;

        IsActiveWindowKnown = window != 0;
        IsPathOfExileActive = window != 0 && IsGameWindow(window);
        IsSidekickActive = window != 0 && ReadWindowProperty(window, pidAtom) == Environment.ProcessId;

        if (wasGame != IsPathOfExileActive)
        {
            logger.LogDebug("[X11] Path of Exile focused: {Focused}.", IsPathOfExileActive);
            ApplyGrabs();
        }
    }

    /// <summary>
    /// The game is found by its window class (the executable name, or the Steam app id under Steam's Proton), or by
    /// its exact window title.
    /// </summary>
    private bool IsGameWindow(nint window)
    {
        if (Xlib.XGetClassHint(display, window, out var hint) != 0)
        {
            try
            {
                var name = hint.ResName != 0 ? Marshal.PtrToStringAnsi(hint.ResName) : null;
                var windowClass = hint.ResClass != 0 ? Marshal.PtrToStringAnsi(hint.ResClass) : null;
                if (IsGameWindowClass(name) || IsGameWindowClass(windowClass))
                {
                    return true;
                }
            }
            finally
            {
                if (hint.ResName != 0)
                {
                    Xlib.XFree(hint.ResName);
                }

                if (hint.ResClass != 0)
                {
                    Xlib.XFree(hint.ResClass);
                }
            }
        }

        var title = ReadStringProperty(window, netWmNameAtom) ?? ReadStringProperty(window, WmNameAtom);
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

    /// <summary>
    /// Reads an 8-bit text property, like a window title.
    /// </summary>
    private string? ReadStringProperty(nint window, nint property)
    {
        if (Xlib.XGetWindowProperty(display, window, property, 0, 256, false, 0, out _, out var format, out var count, out _, out var data) != 0)
        {
            return null;
        }

        try
        {
            return format == 8 && count > 0 ? Marshal.PtrToStringUTF8(data, (int)count) : null;
        }
        finally
        {
            if (data != 0)
            {
                Xlib.XFree(data);
            }
        }
    }

    /// <summary>
    /// Reads the first value of a 32-bit property, like a window id or a process id. Xlib returns 32-bit values as C longs.
    /// </summary>
    private nint ReadWindowProperty(nint window, nint property)
    {
        if (Xlib.XGetWindowProperty(display, window, property, 0, 1, false, 0, out _, out var format, out var count, out _, out var data) != 0)
        {
            return 0;
        }

        try
        {
            return format == 32 && count > 0 ? Marshal.ReadIntPtr(data) : 0;
        }
        finally
        {
            if (data != 0)
            {
                Xlib.XFree(data);
            }
        }
    }

    /// <summary>
    /// Grabs the hotkeys while the game has focus, and releases them otherwise. Must be called with the lock held.
    /// </summary>
    private void ApplyGrabs()
    {
        if (display == 0)
        {
            return;
        }

        UngrabAll();
        if (!IsPathOfExileActive || suspendCount > 0 || getKeybinds == null || loopStopped || disposed)
        {
            Xlib.XSync(display, false);
            return;
        }

        List<string> keybinds;
        bool grabScroll;
        try
        {
            keybinds = getKeybinds().Distinct().ToList();
            grabScroll = isScrollUsed?.Invoke() == true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[X11] Could not read the hotkeys to grab.");
            Xlib.XSync(display, false);
            return;
        }

        grabErrors = 0;
        foreach (var keybind in keybinds)
        {
            if (!X11KeySymbols.TryParse(keybind, out var keySymbol, out var modifiers))
            {
                logger.LogDebug("[X11] No X11 key for the hotkey {Keybind}.", keybind);
                continue;
            }

            var keycode = Xlib.XKeysymToKeycode(display, keySymbol);
            if (keycode == 0)
            {
                logger.LogDebug("[X11] The keyboard layout has no key for the hotkey {Keybind}.", keybind);
                continue;
            }

            grabbedKeys[(keycode, modifiers)] = keybind;
            foreach (var ignored in IgnoredModifiers)
            {
                Xlib.XGrabKey(display, keycode, modifiers | ignored, root, false, Xlib.GrabModeAsync, Xlib.GrabModeSync);
            }
        }

        if (grabScroll)
        {
            foreach (var button in new[] { Xlib.Button4, Xlib.Button5 })
            {
                foreach (var ignored in IgnoredModifiers)
                {
                    Xlib.XGrabButton(display, button, Xlib.ControlMask | ignored, root, false, Xlib.ButtonPressMask | Xlib.ButtonReleaseMask, Xlib.GrabModeSync, Xlib.GrabModeAsync, 0, 0);
                }
            }
        }

        // Grab errors arrive asynchronously; XSync makes them reach the error handler now.
        Xlib.XSync(display, false);
        if (grabErrors > 0)
        {
            logger.LogWarning("[X11] {Count} hotkey grabs failed, another program already uses some of these key combinations.", grabErrors);
        }

        logger.LogDebug("[X11] Grabbed {Count} hotkeys.", grabbedKeys.Count);
    }

    private void UngrabAll()
    {
        Xlib.XUngrabKey(display, Xlib.AnyKey, Xlib.AnyModifier, root);
        Xlib.XUngrabButton(display, Xlib.AnyButton, Xlib.AnyModifier, root);
        grabbedKeys.Clear();
    }

    /// <summary>
    /// Wakes the event loop to read events that a call outside the loop pulled into Xlib's queue. Under the lock, so
    /// Dispose can't close the pipe in between.
    /// </summary>
    private void Wake()
    {
        lock (sync)
        {
            if (wakePipe[1] >= 0 && !loopStopped)
            {
                Xlib.write(wakePipe[1], [1], 1);
            }
        }
    }

    /// <summary>
    /// Handles X11 errors on Sidekick's own connection (a grab taken by another program, a window closed while being
    /// read) and passes every other error to the handler that was there before, Avalonia's.
    /// </summary>
    private void InstallErrorHandler()
    {
        errorHandlerOwner = this;
        errorHandler ??= OnError;
        var previous = Xlib.XSetErrorHandler(errorHandler);
        if (previous != 0 && previousErrorHandler == null)
        {
            previousErrorHandler = Marshal.GetDelegateForFunctionPointer<Xlib.ErrorHandler>(previous);
        }
    }

    private static int OnError(nint errorDisplay, nint errorEvent)
    {
        var owner = errorHandlerOwner;
        if (owner == null || errorDisplay != owner.display)
        {
            return previousErrorHandler?.Invoke(errorDisplay, errorEvent) ?? 0;
        }

        var request = Marshal.ReadByte(errorEvent, Xlib.ErrorOffsets.RequestCode);
        if (request is GrabKeyRequest or GrabButtonRequest)
        {
            owner.grabErrors++;
        }

        return 0;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        Wake();
        var stopped = thread?.Join(TimeSpan.FromSeconds(1)) ?? true;

        lock (sync)
        {
            if (display != 0)
            {
                UngrabAll();
                Xlib.XSync(display, false);

                // A thread that is still running may use the display, so it stays open until the process exits.
                if (stopped)
                {
                    Xlib.XCloseDisplay(display);
                    display = 0;
                }
            }

            if (!stopped)
            {
                return;
            }

            foreach (var fd in wakePipe.Where(fd => fd >= 0))
            {
                Xlib.close(fd);
            }

            wakePipe[0] = -1;
            wakePipe[1] = -1;
        }
    }
}
