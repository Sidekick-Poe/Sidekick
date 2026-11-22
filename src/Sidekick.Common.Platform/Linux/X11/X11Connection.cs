using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Platform.Linux.X11;

/// <summary>
/// A key to grab: an X11 key symbol and the modifiers held with it.
/// </summary>
public readonly record struct X11Key(nint KeySymbol, uint Modifiers);

/// <summary>
/// Sidekick's own connection to the X11 display (XWayland on Wayland sessions), with a thread for its events. It
/// follows the active window, reads window properties and grabs keys. It knows nothing about the game or hotkeys:
/// <see cref="LinuxProcessProvider"/> and <see cref="LinuxHotkeyBlocker"/> decide what to do with it.
/// </summary>
[SupportedOSPlatform(PlatformNames.Linux)]
public sealed class X11Connection(ILogger<X11Connection> logger) : IDisposable
{
    /// <summary>
    /// X11 request codes, to tell grab failures apart from other errors.
    /// </summary>
    private const byte GrabButtonRequest = 28;
    private const byte GrabKeyRequest = 33;

    /// <summary>
    /// The predefined WM_NAME atom, the window title in Latin-1.
    /// </summary>
    private const nint WmNameAtom = 39;

    /// <summary>
    /// How long a grabbed key waits for the decision whether it is used. The keyboard is frozen meanwhile, and some
    /// checks can be slow. Past this, the key goes on to the window that has focus.
    /// </summary>
    private static readonly TimeSpan GrabDecisionTimeout = TimeSpan.FromMilliseconds(50);

    /// <summary>
    /// Lock state, NumLock and their combination change the modifier state of a key press, so every grab is made once
    /// for each of them.
    /// </summary>
    private static readonly uint[] IgnoredModifiers = [0, Xlib.LockMask, Xlib.Mod2Mask, Xlib.LockMask | Xlib.Mod2Mask];

    private const uint KeyModifiers = Xlib.ShiftMask | Xlib.ControlMask | Xlib.Mod1Mask | Xlib.Mod4Mask;

    /// <summary>
    /// Xlib has one error handler per process, shared with Avalonia. Kept in a static field so the delegate is never
    /// collected while Xlib holds it.
    /// </summary>
    private static Xlib.ErrorHandler? errorHandler;

    private static Xlib.ErrorHandler? previousErrorHandler;

    private static X11Connection? errorHandlerOwner;

    private readonly Lock sync = new();

    private readonly Dictionary<(int Keycode, uint Modifiers), X11Key> grabbedKeys = [];

    private readonly int[] wakePipe = [-1, -1];

    private nint display;

    private nint root;

    private nint activeWindowAtom;

    private nint pidAtom;

    private nint netWmNameAtom;

    private Thread? thread;

    private volatile bool disposed;

    /// <summary>
    /// Set when the event loop stopped on an error. Nothing would answer a grabbed key then, so nothing is grabbed again.
    /// </summary>
    private volatile bool loopStopped;

    private int grabErrors;

    private Func<X11Key, bool>? isKeyUsed;

    private Func<bool>? isScrollUsed;

    /// <summary>
    /// Raised on the connection's thread when another window gets focus, with its id, or 0 when no X11 window has
    /// focus (like a native Wayland window).
    /// </summary>
    public event Action<nint>? ActiveWindowChanged;

    /// <summary>
    /// Gets a value indicating whether Sidekick is connected to an X11 display.
    /// </summary>
    public bool IsAvailable => display != 0;

    /// <summary>
    /// Gets the window that has focus, or 0 when no X11 window has focus.
    /// </summary>
    public nint ActiveWindow { get; private set; }

    /// <summary>
    /// Connects to the X11 display and starts following the active window. Without a display (no X11 or XWayland),
    /// nothing happens and <see cref="IsAvailable"/> stays false.
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
                logger.LogWarning("[X11] No DISPLAY, the active window can't be followed.");
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

            if (Libc.pipe(wakePipe) != 0)
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
            ActiveWindow = ReadWindowProperty(root, activeWindowAtom);
            Xlib.XSync(display, false);
        }

        thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "X11 connection",
        };
        thread.Start();
        logger.LogInformation("[X11] Following the active window.");
    }

    /// <summary>
    /// Gets the two parts of a window's class (WM_CLASS): the instance name and the class name.
    /// </summary>
    public (string? Name, string? Class) GetWindowClass(nint window)
    {
        lock (sync)
        {
            if (display == 0 || Xlib.XGetClassHint(display, window, out var hint) == 0)
            {
                return (null, null);
            }

            try
            {
                var name = hint.ResName != 0 ? Marshal.PtrToStringAnsi(hint.ResName) : null;
                var windowClass = hint.ResClass != 0 ? Marshal.PtrToStringAnsi(hint.ResClass) : null;
                return (name, windowClass);
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
    }

    /// <summary>
    /// Gets a window's title.
    /// </summary>
    public string? GetWindowTitle(nint window)
    {
        lock (sync)
        {
            if (display == 0)
            {
                return null;
            }

            return ReadStringProperty(window, netWmNameAtom) ?? ReadStringProperty(window, WmNameAtom);
        }
    }

    /// <summary>
    /// Gets the id of the process that owns a window, or 0 when the window doesn't say.
    /// </summary>
    public long GetWindowProcessId(nint window)
    {
        lock (sync)
        {
            return display == 0 ? 0 : ReadWindowProperty(window, pidAtom);
        }
    }

    /// <summary>
    /// Marks a window as a dialog of another (WM_TRANSIENT_FOR), or removes the mark when <paramref name="parent"/> is 0.
    /// Window managers keep a dialog above its parent window.
    /// </summary>
    public void SetTransientFor(nint window, nint parent)
    {
        lock (sync)
        {
            if (display == 0)
            {
                return;
            }

            if (parent != 0)
            {
                Xlib.XSetTransientForHint(display, window, parent);
            }
            else
            {
                Xlib.XDeleteProperty(display, window, Xlib.WmTransientForAtom);
            }

            Xlib.XSync(display, false);
        }
    }

    /// <summary>
    /// Grabs keys on the whole display, replacing the previous grabs. The X server then waits for
    /// <paramref name="keyUsed"/> on each press: a used key is kept from the window that has focus, any other goes on to it.
    /// </summary>
    /// <param name="keys">The keys to grab.</param>
    /// <param name="grabCtrlScroll">Whether to also grab Ctrl+scroll, decided by <paramref name="scrollUsed"/>.</param>
    /// <param name="keyUsed">Whether a grabbed key press is used.</param>
    /// <param name="scrollUsed">Whether a grabbed Ctrl+scroll is used.</param>
    public void SetGrabs(IEnumerable<X11Key> keys, bool grabCtrlScroll, Func<X11Key, bool> keyUsed, Func<bool> scrollUsed)
    {
        lock (sync)
        {
            if (display == 0 || loopStopped || disposed)
            {
                return;
            }

            UngrabAll();
            isKeyUsed = keyUsed;
            isScrollUsed = scrollUsed;
            grabErrors = 0;

            foreach (var key in keys)
            {
                var keycode = Xlib.XKeysymToKeycode(display, key.KeySymbol);
                if (keycode == 0)
                {
                    logger.LogDebug("[X11] The keyboard layout has no key for the key symbol {KeySymbol:x}.", key.KeySymbol);
                    continue;
                }

                grabbedKeys[(keycode, key.Modifiers)] = key;
                foreach (var ignored in IgnoredModifiers)
                {
                    Xlib.XGrabKey(display, keycode, key.Modifiers | ignored, root, false, Xlib.GrabModeAsync, Xlib.GrabModeSync);
                }
            }

            if (grabCtrlScroll)
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
                logger.LogWarning("[X11] {Count} grabs failed, another program already uses some of these key combinations.", grabErrors);
            }

            logger.LogDebug("[X11] Grabbed {Count} keys.", grabbedKeys.Count);
        }

        Wake();
    }

    /// <summary>
    /// Releases every grab.
    /// </summary>
    public void ClearGrabs()
    {
        lock (sync)
        {
            if (display == 0)
            {
                return;
            }

            UngrabAll();
            Xlib.XSync(display, false);
        }

        Wake();
    }

    /// <summary>
    /// Gets a value indicating whether this key is grabbed.
    /// </summary>
    public bool IsGrabbed(X11Key key)
    {
        lock (sync)
        {
            if (display == 0)
            {
                return false;
            }

            var keycode = Xlib.XKeysymToKeycode(display, key.KeySymbol);
            return keycode != 0 && grabbedKeys.ContainsKey((keycode, key.Modifiers));
        }
    }

    /// <summary>
    /// Ends the keyboard grab that is active while the user still holds a grabbed key. Simulated keys would otherwise
    /// go to Sidekick instead of the window that has focus.
    /// </summary>
    public void ReleaseActiveKeyboardGrab()
    {
        lock (sync)
        {
            if (display == 0)
            {
                return;
            }

            Xlib.XUngrabKeyboard(display, 0);
            Xlib.XSync(display, false);
        }

        Wake();
    }

    private void Run()
    {
        var fds = new Libc.PollFd[]
        {
            new()
            {
                Fd = Xlib.XConnectionNumber(display),
                Events = Libc.POLLIN,
            },
            new()
            {
                Fd = wakePipe[0],
                Events = Libc.POLLIN,
            },
        };
        var wakeBuffer = new byte[64];
        var eventBuffer = Marshal.AllocHGlobal(Xlib.EventSize);
        try
        {
            while (!disposed)
            {
                var activeWindowChanged = false;
                lock (sync)
                {
                    while (!disposed && Xlib.XPending(display) > 0)
                    {
                        Xlib.XNextEvent(display, eventBuffer);
                        try
                        {
                            activeWindowChanged |= HandleEvent(eventBuffer);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "[X11] Could not handle an event.");
                        }
                    }
                }

                // Raised outside the lock, since the listeners call back into the connection, and can take their own locks.
                if (activeWindowChanged)
                {
                    RaiseActiveWindowChanged();
                }

                fds[0].Revents = 0;
                fds[1].Revents = 0;
                Libc.poll(fds, 2, -1);
                if ((fds[1].Revents & Libc.POLLIN) != 0)
                {
                    Libc.read(wakePipe[0], wakeBuffer, wakeBuffer.Length);
                }
            }
        }
        catch (Exception ex)
        {
            // Without the loop, nobody answers the synchronous grabs, which would freeze the keyboard.
            logger.LogError(ex, "[X11] The event loop stopped, releasing the grabs.");
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

    /// <returns>True when the active window changed.</returns>
    private bool HandleEvent(nint xEvent)
    {
        switch (Marshal.ReadInt32(xEvent, Xlib.EventOffsets.Type))
        {
            case Xlib.PropertyNotify:
                if (Marshal.ReadIntPtr(xEvent, Xlib.EventOffsets.PropertyAtom) != activeWindowAtom)
                {
                    return false;
                }

                var window = ReadWindowProperty(root, activeWindowAtom);
                if (window == ActiveWindow)
                {
                    return false;
                }

                ActiveWindow = window;
                return true;

            case Xlib.KeyPress:
                OnGrabbedKey(xEvent);
                return false;

            case Xlib.ButtonPress:
                OnGrabbedScroll();
                return false;

            default:
                return false;
        }
    }

    private void RaiseActiveWindowChanged()
    {
        try
        {
            ActiveWindowChanged?.Invoke(ActiveWindow);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[X11] A listener failed on an active window change.");
        }
    }

    /// <summary>
    /// Answers a grabbed key press: kept from the focused window when it is used, given back otherwise.
    /// </summary>
    private void OnGrabbedKey(nint xEvent)
    {
        var keycode = Marshal.ReadInt32(xEvent, Xlib.EventOffsets.InputKeycodeOrButton);
        var modifiers = (uint)Marshal.ReadInt32(xEvent, Xlib.EventOffsets.InputState) & KeyModifiers;
        var check = isKeyUsed;

        var used = check != null
                   && grabbedKeys.TryGetValue((keycode, modifiers), out var key)
                   && Decide(() => check(key), "a grabbed key");
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
    /// it answers. A check that fails or is too slow counts as unused, so the input goes on to the focused window.
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

            logger.LogWarning("[X11] Checking {Input} took too long, it goes to the focused window.", input);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[X11] Could not check {Input}.", input);
        }

        return false;
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
                Libc.write(wakePipe[1], [1], 1);
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
                Libc.close(fd);
            }

            wakePipe[0] = -1;
            wakePipe[1] = -1;
        }
    }
}
