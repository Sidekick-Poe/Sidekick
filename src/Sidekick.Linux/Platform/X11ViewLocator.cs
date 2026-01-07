using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sidekick.Common;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Overlay;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Linux.Platform;

public sealed class X11ViewLocator(
    ILogger<X11ViewLocator> logger,
    IServiceProvider serviceProvider,
    IProcessProvider processProvider) : IViewLocator, IOverlayInputRegionService, IOverlayVisibilityService, IDisposable
{
    private readonly object syncLock = new();
    private IntPtr display = IntPtr.Zero;
    private WebWindowHost? overlayHost;
    private IntPtr overlayWindow;
    private bool overlayVisible;
    private bool x11Unavailable;
    private bool dependencyDialogShown;
    private bool kdeRuleApplied;
    private IReadOnlyList<OverlayRegion> pendingRegions = Array.Empty<OverlayRegion>();
    private readonly Dictionary<SidekickViewType, Guid> openWidgets = new();
    private int lastOverlayX;
    private int lastOverlayY;
    private int lastOverlayWidth;
    private int lastOverlayHeight;
    private DateTimeOffset lastOverlayRaise;
    private Timer? overlayMonitor;
    private int overlayMonitorActive;

    public bool SupportsMinimize => false;

    public bool SupportsMaximize => false;

    public void Open(SidekickViewType type, string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }

        if (!TryEnsureDisplay())
        {
            return;
        }

        EnsureOverlayWindow();
        var widgetService = serviceProvider.GetService<OverlayWidgetService>();
        if (widgetService == null)
        {
            return;
        }

        if (url == "/overlay" || url == "/")
        {
            widgetService.EnsureMenuWidget();
            return;
        }

        if (openWidgets.TryGetValue(type, out var existingId))
        {
            widgetService.CloseWidget(existingId);
            openWidgets.Remove(type);
        }

        var widget = widgetService.OpenWidget(url);
        openWidgets[type] = widget.Id;
    }

    public void Close(SidekickViewType type)
    {
        if (type == SidekickViewType.Overlay)
        {
            var overlayWidgetService = serviceProvider.GetService<OverlayWidgetService>();
            if (overlayWidgetService != null)
            {
                overlayWidgetService.ClearWidgets();
            }

            openWidgets.Clear();
            return;
        }

        if (!openWidgets.TryGetValue(type, out var widgetId))
        {
            return;
        }

        var widgetService = serviceProvider.GetService<OverlayWidgetService>();
        if (widgetService == null)
        {
            return;
        }

        widgetService.CloseWidget(widgetId);
        openWidgets.Remove(type);
    }

    public bool IsOverlayOpened()
    {
        return overlayWindow != IntPtr.Zero;
    }

    public void UpdateRegions(IReadOnlyList<OverlayRegion> regions)
    {
        pendingRegions = regions.ToList();
        if (overlayWindow == IntPtr.Zero)
        {
            return;
        }

        ApplyInputRegions(overlayWindow, pendingRegions);
    }

    public void SetOverlayVisible(bool isVisible)
    {
        if (!TryEnsureDisplay())
        {
            return;
        }

        EnsureOverlayWindow();
        if (overlayWindow == IntPtr.Zero)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var shouldRefresh = isVisible && overlayVisible && now - lastOverlayRaise >= OverlayRaiseInterval;
        if (overlayVisible == isVisible && !shouldRefresh)
        {
            return;
        }

        if (overlayVisible != isVisible)
        {
            overlayVisible = isVisible;
        }

        if (isVisible)
        {
            EnsureOverlayMonitor();
            UpdateOverlayBounds();
            ApplyWindowType(overlayWindow);
            ApplyWindowState(overlayWindow);
            ApplyNoDecorations(overlayWindow);
            ApplyInputRegions(overlayWindow, pendingRegions);
            XMapRaised(display, overlayWindow);
            XRaiseWindow(display, overlayWindow);
            ApplyWindowState(overlayWindow);
            lastOverlayRaise = now;
        }
        else
        {
            StopOverlayMonitor();
            XUnmapWindow(display, overlayWindow);
        }

        XFlush(display);
    }

    public void Dispose()
    {
        StopOverlayMonitor();
        if (display != IntPtr.Zero)
        {
            XCloseDisplay(display);
            display = IntPtr.Zero;
        }
    }

    private void EnsureOverlayWindow()
    {
        lock (syncLock)
        {
            if (overlayHost != null)
            {
                if (!overlayHost.IsRunning)
                {
                    overlayHost = null;
                    overlayWindow = IntPtr.Zero;
                }
                else if (overlayWindow != IntPtr.Zero && !IsWindowAlive(overlayWindow))
                {
                    overlayWindow = IntPtr.Zero;
                }
                else if (overlayWindow != IntPtr.Zero)
                {
                    return;
                }
            }

            var hasHost = overlayHost != null;
            if (!hasHost)
            {
                GetOverlayBounds(out var x, out var y, out var width, out var height, ShouldUseActiveWindowForBounds());

                var overlayUrl = BuildAbsoluteUrl("/overlay");
                if (!Uri.TryCreate(overlayUrl, UriKind.Absolute, out _))
                {
                    overlayUrl = "http://127.0.0.1:8080/overlay";
                    logger.LogWarning("[Linux/X11] Overlay URL was not absolute; falling back to {Url}", overlayUrl);
                }
                logger.LogInformation("[Linux/X11] Launching overlay window at {Url}", overlayUrl);

                overlayHost = new WebWindowHost(overlayUrl, "Sidekick Overlay", width, height, logger);
                overlayHost.Show();
            }

            if (overlayHost == null)
            {
                return;
            }

            overlayWindow = WaitForWindowByTitle(overlayHost.Title, TimeSpan.FromSeconds(4));
            if (overlayWindow == IntPtr.Zero)
            {
                logger.LogWarning("[Linux/X11] Failed to resolve overlay window handle.");
                return;
            }

            GetOverlayBounds(out var resolvedX, out var resolvedY, out var resolvedWidth, out var resolvedHeight, ShouldUseActiveWindowForBounds());
            logger.LogInformation("[Linux/X11] Resolved overlay window handle: {Handle}", overlayWindow);
            ApplyWindowType(overlayWindow);
            ApplyWindowState(overlayWindow);
            ApplyNoDecorations(overlayWindow);
            XMoveResizeWindow(display, overlayWindow, resolvedX, resolvedY, (uint)Math.Max(1, resolvedWidth), (uint)Math.Max(1, resolvedHeight));
            ApplyInputRegions(overlayWindow, pendingRegions);
            TryApplyKdeWindowRule();
            if (!overlayVisible)
            {
                XUnmapWindow(display, overlayWindow);
                XFlush(display);
            }
        }
    }

    private void UpdateOverlayBounds()
    {
        if (display == IntPtr.Zero || overlayWindow == IntPtr.Zero)
        {
            return;
        }

        GetOverlayBounds(out var x, out var y, out var width, out var height, ShouldUseActiveWindowForBounds());
        XMoveResizeWindow(display, overlayWindow, x, y, (uint)Math.Max(1, width), (uint)Math.Max(1, height));
        XFlush(display);
    }

    private void GetOverlayBounds(out int x, out int y, out int width, out int height, bool allowActiveWindow)
    {
        if (allowActiveWindow
            && TryGetActiveWindowBounds(out var activeX, out var activeY, out var activeWidth, out var activeHeight)
            && TryGetMonitorBoundsForPoint(activeX + (activeWidth / 2), activeY + (activeHeight / 2), out x, out y, out width, out height))
        {
            StoreLastOverlayBounds(x, y, width, height);
            return;
        }

        if (lastOverlayWidth > 0 && lastOverlayHeight > 0)
        {
            x = lastOverlayX;
            y = lastOverlayY;
            width = lastOverlayWidth;
            height = lastOverlayHeight;
            return;
        }

        if (TryGetPrimaryMonitorBounds(out x, out y, out width, out height))
        {
            StoreLastOverlayBounds(x, y, width, height);
            return;
        }

        var screen = XDefaultScreen(display);
        x = 0;
        y = 0;
        width = XDisplayWidth(display, screen);
        height = XDisplayHeight(display, screen);
        StoreLastOverlayBounds(x, y, width, height);
    }

    private bool ShouldUseActiveWindowForBounds()
    {
        if (processProvider.IsPathOfExileInFocus)
        {
            return true;
        }

        return lastOverlayWidth == 0 || lastOverlayHeight == 0;
    }

    private void StoreLastOverlayBounds(int x, int y, int width, int height)
    {
        lastOverlayX = x;
        lastOverlayY = y;
        lastOverlayWidth = width;
        lastOverlayHeight = height;
    }

    private bool TryGetMonitorBoundsForPoint(int x, int y, out int monitorX, out int monitorY, out int monitorWidth, out int monitorHeight)
    {
        monitorX = 0;
        monitorY = 0;
        monitorWidth = 0;
        monitorHeight = 0;

        if (display == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            var root = XDefaultRootWindow(display);
            var monitors = XRRGetMonitors(display, root, true, out var monitorCount);
            if (monitors == IntPtr.Zero || monitorCount <= 0)
            {
                return false;
            }

            try
            {
                var size = Marshal.SizeOf<XRRMonitorInfo>();
                XRRMonitorInfo? primary = null;
                for (var i = 0; i < monitorCount; i++)
                {
                    var infoPtr = IntPtr.Add(monitors, i * size);
                    var info = Marshal.PtrToStructure<XRRMonitorInfo>(infoPtr);
                    if (info.Primary != 0)
                    {
                        primary = info;
                    }

                    if (x >= info.X && x < info.X + info.Width && y >= info.Y && y < info.Y + info.Height)
                    {
                        monitorX = info.X;
                        monitorY = info.Y;
                        monitorWidth = info.Width;
                        monitorHeight = info.Height;
                        return true;
                    }
                }

                if (primary.HasValue)
                {
                    var info = primary.Value;
                    monitorX = info.X;
                    monitorY = info.Y;
                    monitorWidth = info.Width;
                    monitorHeight = info.Height;
                    return true;
                }
            }
            finally
            {
                XRRFreeMonitors(monitors);
            }
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }

        return false;
    }

    private bool TryGetPrimaryMonitorBounds(out int monitorX, out int monitorY, out int monitorWidth, out int monitorHeight)
    {
        monitorX = 0;
        monitorY = 0;
        monitorWidth = 0;
        monitorHeight = 0;

        if (display == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            var root = XDefaultRootWindow(display);
            var monitors = XRRGetMonitors(display, root, true, out var monitorCount);
            if (monitors == IntPtr.Zero || monitorCount <= 0)
            {
                return false;
            }

            try
            {
                var size = Marshal.SizeOf<XRRMonitorInfo>();
                XRRMonitorInfo? first = null;
                XRRMonitorInfo? primary = null;
                for (var i = 0; i < monitorCount; i++)
                {
                    var infoPtr = IntPtr.Add(monitors, i * size);
                    var info = Marshal.PtrToStructure<XRRMonitorInfo>(infoPtr);
                    first ??= info;
                    if (info.Primary != 0)
                    {
                        primary = info;
                        break;
                    }
                }

                var chosen = primary ?? first;
                if (!chosen.HasValue)
                {
                    return false;
                }

                var value = chosen.Value;
                monitorX = value.X;
                monitorY = value.Y;
                monitorWidth = value.Width;
                monitorHeight = value.Height;
                return true;
            }
            finally
            {
                XRRFreeMonitors(monitors);
            }
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    private bool TryGetActiveWindowBounds(out int x, out int y, out int width, out int height)
    {
        x = 0;
        y = 0;
        width = 0;
        height = 0;

        if (display == IntPtr.Zero)
        {
            return false;
        }

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

        var window = ReadWindowId(prop);
        XFree(prop);
        if (window == IntPtr.Zero)
        {
            return false;
        }

        if (overlayHost != null)
        {
            var title = FetchWindowTitle(window);
            if (!string.IsNullOrEmpty(title)
                && string.Equals(title, overlayHost.Title, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return TryGetWindowBounds(window, root, out x, out y, out width, out height);
    }

    private bool TryGetWindowBounds(IntPtr window, IntPtr root, out int x, out int y, out int width, out int height)
    {
        x = 0;
        y = 0;
        width = 0;
        height = 0;

        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return false;
        }

        var status = XGetGeometry(
            display,
            window,
            out _,
            out var winX,
            out var winY,
            out var winWidth,
            out var winHeight,
            out _,
            out _);

        if (status == 0 || winWidth == 0 || winHeight == 0)
        {
            return false;
        }

        if (!XTranslateCoordinates(display, window, root, 0, 0, out var rootX, out var rootY, out _))
        {
            rootX = winX;
            rootY = winY;
        }

        x = rootX;
        y = rootY;
        width = (int)winWidth;
        height = (int)winHeight;
        return true;
    }

    private bool IsWindowAlive(IntPtr window)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return false;
        }

        return XGetWindowAttributes(display, window, out _) != 0;
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

    private bool TryEnsureDisplay()
    {
        if (x11Unavailable)
        {
            return false;
        }

        if (display != IntPtr.Zero)
        {
            return true;
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        {
            logger.LogWarning("[Linux/X11] DISPLAY not set; overlay disabled.");
            x11Unavailable = true;
            return false;
        }

        try
        {
            display = XOpenDisplay(IntPtr.Zero);
        }
        catch (DllNotFoundException)
        {
            logger.LogWarning("[Linux/X11] libX11 not available; overlay disabled.");
            x11Unavailable = true;
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            logger.LogWarning("[Linux/X11] libX11 entry points missing; overlay disabled.");
            x11Unavailable = true;
            return false;
        }

        if (display == IntPtr.Zero)
        {
            logger.LogWarning("[Linux/X11] Unable to open X11 display; overlay disabled.");
            x11Unavailable = true;
            return false;
        }

        return true;
    }

    private void ApplyWindowState(IntPtr window)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return;
        }

        var wmState = XInternAtom(display, "_NET_WM_STATE", false);
        var stateAbove = XInternAtom(display, "_NET_WM_STATE_ABOVE", false);
        var stateSticky = XInternAtom(display, "_NET_WM_STATE_STICKY", false);
        var stateSkipTaskbar = XInternAtom(display, "_NET_WM_STATE_SKIP_TASKBAR", false);
        var stateSkipPager = XInternAtom(display, "_NET_WM_STATE_SKIP_PAGER", false);
        var stateStaysOnTop = XInternAtom(display, "_NET_WM_STATE_STAYS_ON_TOP", true);

        if (wmState == IntPtr.Zero || stateAbove == IntPtr.Zero || stateSticky == IntPtr.Zero)
        {
            return;
        }

        var states = new List<IntPtr>
        {
            stateAbove,
            stateSticky
        };

        if (stateSkipTaskbar != IntPtr.Zero)
        {
            states.Add(stateSkipTaskbar);
        }

        if (stateSkipPager != IntPtr.Zero)
        {
            states.Add(stateSkipPager);
        }

        if (stateStaysOnTop != IntPtr.Zero)
        {
            states.Add(stateStaysOnTop);
        }

        if (states.Count == 0)
        {
            return;
        }

        var size = Marshal.SizeOf<IntPtr>();
        var data = Marshal.AllocHGlobal(size * states.Count);
        try
        {
            for (var i = 0; i < states.Count; i++)
            {
                Marshal.WriteIntPtr(data, i * size, states[i]);
            }

            XChangeProperty(display, window, wmState, XA_ATOM, 32, PropModeReplace, data, states.Count);
            foreach (var state in states)
            {
                RequestWindowState(window, state, NetWmStateAdd);
            }
            XFlush(display);
        }
        finally
        {
            Marshal.FreeHGlobal(data);
        }
    }

    private void ApplyWindowType(IntPtr window)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return;
        }

        var wmWindowType = XInternAtom(display, "_NET_WM_WINDOW_TYPE", false);
        if (wmWindowType == IntPtr.Zero)
        {
            return;
        }

        var typeUtility = XInternAtom(display, "_NET_WM_WINDOW_TYPE_UTILITY", false);
        var typeNotification = XInternAtom(display, "_NET_WM_WINDOW_TYPE_NOTIFICATION", false);
        var types = new List<IntPtr>();
        if (typeUtility != IntPtr.Zero)
        {
            types.Add(typeUtility);
        }

        if (typeNotification != IntPtr.Zero)
        {
            types.Add(typeNotification);
        }

        if (types.Count == 0)
        {
            return;
        }

        var size = Marshal.SizeOf<IntPtr>();
        var data = Marshal.AllocHGlobal(size * types.Count);
        try
        {
            for (var i = 0; i < types.Count; i++)
            {
                Marshal.WriteIntPtr(data, i * size, types[i]);
            }

            XChangeProperty(display, window, wmWindowType, XA_ATOM, 32, PropModeReplace, data, types.Count);
            XFlush(display);
        }
        finally
        {
            Marshal.FreeHGlobal(data);
        }
    }

    private void RequestWindowState(IntPtr window, IntPtr stateAtom, int action)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero || stateAtom == IntPtr.Zero)
        {
            return;
        }

        var wmState = XInternAtom(display, "_NET_WM_STATE", false);
        if (wmState == IntPtr.Zero)
        {
            return;
        }

        var root = XDefaultRootWindow(display);
        var xEvent = new XEvent
        {
            ClientMessage = new XClientMessageEvent
            {
                Type = ClientMessage,
                SendEvent = true,
                Display = display,
                Window = window,
                MessageType = wmState,
                Format = 32,
                Data0 = new IntPtr(action),
                Data1 = stateAtom,
                Data2 = IntPtr.Zero,
                Data3 = IntPtr.Zero,
                Data4 = IntPtr.Zero
            }
        };

        XSendEvent(
            display,
            root,
            false,
            new IntPtr(SubstructureRedirectMask | SubstructureNotifyMask),
            ref xEvent);
    }

    private void ApplyNoDecorations(IntPtr window)
    {
        var motifHints = XInternAtom(display, "_MOTIF_WM_HINTS", false);
        if (motifHints == IntPtr.Zero)
        {
            return;
        }

        var hints = new MotifWmHints
        {
            Flags = MotifHintDecorations,
            Decorations = 0
        };

        var size = Marshal.SizeOf<MotifWmHints>();
        var data = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(hints, data, false);
            XChangeProperty(display, window, motifHints, motifHints, 32, PropModeReplace, data, size / 4);
        }
        finally
        {
            Marshal.FreeHGlobal(data);
        }
    }

    private void ApplyInputRegions(IntPtr window, IReadOnlyList<OverlayRegion> regions)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return;
        }

        try
        {
            if (regions.Count == 0)
            {
                XShapeCombineRectangles(display, window, ShapeInput, 0, 0, IntPtr.Zero, 0, ShapeSet, 0);
                XShapeCombineRectangles(display, window, ShapeBounding, 0, 0, IntPtr.Zero, 0, ShapeSet, 0);
                XFlush(display);
                return;
            }

            var inputRectangles = regions.Select(region => ToXRectangle(region)).ToArray();
            var boundingRectangles = regions.Select(region => ToXRectangle(region, BoundingPadding)).ToArray();
            ApplyShapeRectangles(window, ShapeInput, inputRectangles);
            ApplyShapeRectangles(window, ShapeBounding, boundingRectangles);
            XFlush(display);
        }
        catch (DllNotFoundException)
        {
            logger.LogWarning("[Linux/X11] libXext not available; click-through disabled. Install: Ubuntu: sudo apt-get install -y libxext6 | Fedora: sudo dnf install -y libXext | Arch: sudo pacman -S libxext");
            ShowDependencyWarning("X11 input shaping is unavailable. Install libXext: Ubuntu: sudo apt-get install -y libxext6 | Fedora: sudo dnf install -y libXext | Arch: sudo pacman -S libxext");
        }
        catch (EntryPointNotFoundException)
        {
            logger.LogWarning("[Linux/X11] libXext entry points missing; click-through disabled. Install: Ubuntu: sudo apt-get install -y libxext6 | Fedora: sudo dnf install -y libXext | Arch: sudo pacman -S libxext");
            ShowDependencyWarning("X11 input shaping failed. Install libXext: Ubuntu: sudo apt-get install -y libxext6 | Fedora: sudo dnf install -y libXext | Arch: sudo pacman -S libxext");
        }
    }

    private void ApplyShapeRectangles(IntPtr window, int shapeKind, XRectangle[] rectangles)
    {
        if (display == IntPtr.Zero || window == IntPtr.Zero)
        {
            return;
        }

        if (rectangles.Length == 0)
        {
            XShapeCombineRectangles(display, window, shapeKind, 0, 0, IntPtr.Zero, 0, ShapeSet, 0);
            return;
        }

        var size = Marshal.SizeOf<XRectangle>();
        var data = Marshal.AllocHGlobal(size * rectangles.Length);
        try
        {
            for (var i = 0; i < rectangles.Length; i++)
            {
                Marshal.StructureToPtr(rectangles[i], data + i * size, false);
            }

            XShapeCombineRectangles(display, window, shapeKind, 0, 0, data, rectangles.Length, ShapeSet, 0);
        }
        finally
        {
            Marshal.FreeHGlobal(data);
        }
    }

    private static XRectangle ToXRectangle(OverlayRegion region)
    {
        var x = (short)Math.Clamp(region.X, short.MinValue, short.MaxValue);
        var y = (short)Math.Clamp(region.Y, short.MinValue, short.MaxValue);
        var width = (ushort)Math.Clamp(region.Width, 1, ushort.MaxValue);
        var height = (ushort)Math.Clamp(region.Height, 1, ushort.MaxValue);
        return new XRectangle { X = x, Y = y, Width = width, Height = height };
    }

    private static XRectangle ToXRectangle(OverlayRegion region, int padding)
    {
        var padded = new OverlayRegion(
            region.X - padding,
            region.Y - padding,
            Math.Max(1, region.Width + padding * 2),
            Math.Max(1, region.Height + padding * 2));
        return ToXRectangle(padded);
    }

    private IntPtr WaitForWindowByTitle(string title, TimeSpan timeout)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            var handle = FindWindowByTitle(title);
            if (handle != IntPtr.Zero)
            {
                return handle;
            }

            Thread.Sleep(150);
        }

        return IntPtr.Zero;
    }

    private IntPtr FindWindowByTitle(string title)
    {
        if (display == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        var root = XDefaultRootWindow(display);
        var clientList = XInternAtom(display, "_NET_CLIENT_LIST", true);
        if (clientList == IntPtr.Zero)
        {
            return IntPtr.Zero;
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

        if (status != 0 || prop == IntPtr.Zero || nitems == UIntPtr.Zero)
        {
            if (prop != IntPtr.Zero)
            {
                XFree(prop);
            }

            return IntPtr.Zero;
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

                var windowTitle = FetchWindowTitle(window);
                if (!string.IsNullOrEmpty(windowTitle)
                    && windowTitle.Contains(title, StringComparison.OrdinalIgnoreCase))
                {
                    return window;
                }
            }
        }
        finally
        {
            XFree(prop);
        }

        return IntPtr.Zero;
    }

    private string? FetchWindowTitle(IntPtr window)
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

            if (status == 0 && prop != IntPtr.Zero && nitems != UIntPtr.Zero)
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

        var statusFallback = XFetchName(display, window, out var namePtr);
        if (statusFallback == 0 || namePtr == IntPtr.Zero)
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

    private static string BuildAbsoluteUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
        {
            if (string.Equals(absoluteUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(absoluteUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return absoluteUri.ToString();
            }
        }

        var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
            ?? Environment.GetEnvironmentVariable("DOTNET_URLS");
        if (!string.IsNullOrEmpty(urls))
        {
            var candidates = urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var candidate in candidates)
            {
                if (!Uri.TryCreate(candidate, UriKind.Absolute, out var baseUri))
                {
                    continue;
                }

                if (!string.Equals(baseUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(baseUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return new Uri(baseUri, url).ToString();
            }
        }

        var fallback = new UriBuilder(Uri.UriSchemeHttp, "localhost", 8080)
        {
            Path = url.TrimStart('/')
        };
        return fallback.Uri.ToString();
    }

    private void TryApplyKdeWindowRule()
    {
        if (kdeRuleApplied || !IsKdeSession())
        {
            return;
        }

        kdeRuleApplied = true;
        try
        {
            var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
            if (string.IsNullOrWhiteSpace(configHome))
            {
                configHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
            }

            var rulePath = Path.Combine(configHome, "kwinrulesrc");
            var existing = File.Exists(rulePath) ? File.ReadAllText(rulePath) : string.Empty;
            if (existing.Contains("Description=Sidekick Overlay", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var nextId = GetNextKdeRuleId(existing);
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(existing) && !existing.EndsWith('\n'))
            {
                builder.AppendLine();
            }

            builder.AppendLine($"[{nextId}]");
            builder.AppendLine("Description=Sidekick Overlay");
            builder.AppendLine("wmclass=dev.sidekick.overlay");
            builder.AppendLine("wmclassmatch=1");
            builder.AppendLine("title=Sidekick Overlay");
            builder.AppendLine("titlematch=1");
            builder.AppendLine("above=true");
            builder.AppendLine("aboverule=2");
            builder.AppendLine("noborder=true");
            builder.AppendLine("noborderrule=2");
            builder.AppendLine("skiptaskbar=true");
            builder.AppendLine("skiptaskbarrule=2");
            builder.AppendLine("skippager=true");
            builder.AppendLine("skippagerrule=2");

            File.AppendAllText(rulePath, builder.ToString());
            if (TryReloadKWin())
            {
                logger.LogInformation("[Linux/KWin] Applied overlay window rule.");
            }
            else
            {
                logger.LogWarning("[Linux/KWin] Wrote overlay window rule, but could not reload KWin. Run: qdbus org.kde.KWin /KWin reconfigure");
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Linux/KWin] Failed to apply overlay window rule.");
        }
    }

    private static bool IsKdeSession()
    {
        var desktop = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        if (!string.IsNullOrWhiteSpace(desktop))
        {
            if (desktop.Contains("KDE", StringComparison.OrdinalIgnoreCase)
                || desktop.Contains("Plasma", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return string.Equals(
            Environment.GetEnvironmentVariable("KDE_FULL_SESSION"),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }

    private static int GetNextKdeRuleId(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 1;
        }

        var matches = Regex.Matches(content, @"^\[(\d+)\]$", RegexOptions.Multiline);
        var max = 0;
        foreach (Match match in matches)
        {
            if (int.TryParse(match.Groups[1].Value, out var value) && value > max)
            {
                max = value;
            }
        }

        return max + 1;
    }

    private bool TryReloadKWin()
    {
        var candidates = new[] { "qdbus", "qdbus-qt5", "qdbus-qt6" };
        foreach (var candidate in candidates)
        {
            try
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = candidate,
                    Arguments = "org.kde.KWin /KWin reconfigure",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });

                if (process == null)
                {
                    continue;
                }

                if (!process.WaitForExit(2000))
                {
                    continue;
                }

                if (process.ExitCode == 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // ignore missing qdbus variants
            }
        }

        return false;
    }

    private void ShowDependencyWarning(string message)
    {
        lock (syncLock)
        {
            if (dependencyDialogShown)
            {
                return;
            }

            dependencyDialogShown = true;
        }

        _ = Task.Run(async () =>
        {
            var dialogs = serviceProvider.GetService<ISidekickDialogs>();
            if (dialogs != null)
            {
                await dialogs.OpenOkModal(message);
            }
        });
    }

    private void EnsureOverlayMonitor()
    {
        if (overlayMonitor != null)
        {
            return;
        }

        overlayMonitor = new Timer(_ => MonitorOverlay(), null, OverlayMonitorInterval, OverlayMonitorInterval);
    }

    private void StopOverlayMonitor()
    {
        overlayMonitor?.Dispose();
        overlayMonitor = null;
    }

    private void MonitorOverlay()
    {
        if (Interlocked.Exchange(ref overlayMonitorActive, 1) == 1)
        {
            return;
        }

        try
        {
            if (!overlayVisible)
            {
                return;
            }

            SetOverlayVisible(true);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "[Linux/X11] Overlay monitor refresh failed.");
        }
        finally
        {
            Interlocked.Exchange(ref overlayMonitorActive, 0);
        }
    }

    private static readonly TimeSpan OverlayRaiseInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan OverlayMonitorInterval = TimeSpan.FromSeconds(2);
    private const int Success = 0;
    private const int PropModeReplace = 0;
    private const int ShapeSet = 0;
    private const int ShapeBounding = 0;
    private const int ShapeInput = 2;
    private const int BoundingPadding = 0;
    private const int MotifHintDecorations = 1 << 1;
    private const int NetWmStateAdd = 1;
    private const int ClientMessage = 33;
    private const long SubstructureNotifyMask = 1L << 19;
    private const long SubstructureRedirectMask = 1L << 20;
    private static readonly IntPtr XA_ATOM = new(4);

    [StructLayout(LayoutKind.Sequential)]
    private struct XRectangle
    {
        public short X;
        public short Y;
        public ushort Width;
        public ushort Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XWindowAttributes
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int BorderWidth;
        public int Depth;
        public IntPtr Visual;
        public IntPtr Root;
        public int Class;
        public int BitGravity;
        public int WinGravity;
        public int BackingStore;
        public UIntPtr BackingPlanes;
        public UIntPtr BackingPixel;
        public int SaveUnder;
        public IntPtr Colormap;
        public int MapInstalled;
        public int MapState;
        public long AllEventMasks;
        public long YourEventMask;
        public long DoNotPropagateMask;
        public int OverrideRedirect;
        public IntPtr Screen;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MotifWmHints
    {
        public uint Flags;
        public uint Functions;
        public uint Decorations;
        public int InputMode;
        public uint Status;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XRRMonitorInfo
    {
        public IntPtr Name;
        public int Primary;
        public int Automatic;
        public int NOutput;
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int MWidth;
        public int MHeight;
        public IntPtr Outputs;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XClientMessageEvent
    {
        public int Type;
        public IntPtr Serial;
        [MarshalAs(UnmanagedType.Bool)]
        public bool SendEvent;
        public IntPtr Display;
        public IntPtr Window;
        public IntPtr MessageType;
        public int Format;
        public IntPtr Data0;
        public IntPtr Data1;
        public IntPtr Data2;
        public IntPtr Data3;
        public IntPtr Data4;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct XEvent
    {
        [FieldOffset(0)]
        public int Type;
        [FieldOffset(0)]
        public XClientMessageEvent ClientMessage;
    }

    [DllImport("libX11.so.6", EntryPoint = "XOpenDisplay")]
    private static extern IntPtr XOpenDisplay(IntPtr displayName);

    [DllImport("libX11.so.6", EntryPoint = "XCloseDisplay")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XInternAtom")]
    private static extern IntPtr XInternAtom(IntPtr display, string atomName, bool onlyIfExists);

    [DllImport("libX11.so.6", EntryPoint = "XDefaultRootWindow")]
    private static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XDefaultScreen")]
    private static extern int XDefaultScreen(IntPtr display);

    [DllImport("libX11.so.6", EntryPoint = "XDisplayWidth")]
    private static extern int XDisplayWidth(IntPtr display, int screenNumber);

    [DllImport("libX11.so.6", EntryPoint = "XDisplayHeight")]
    private static extern int XDisplayHeight(IntPtr display, int screenNumber);

    [DllImport("libX11.so.6", EntryPoint = "XGetGeometry")]
    private static extern int XGetGeometry(
        IntPtr display,
        IntPtr drawable,
        out IntPtr root,
        out int x,
        out int y,
        out uint width,
        out uint height,
        out uint borderWidth,
        out uint depth);

    [DllImport("libX11.so.6", EntryPoint = "XTranslateCoordinates")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool XTranslateCoordinates(
        IntPtr display,
        IntPtr srcW,
        IntPtr destW,
        int srcX,
        int srcY,
        out int destX,
        out int destY,
        out IntPtr child);

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

    [DllImport("libX11.so.6", EntryPoint = "XGetWindowAttributes")]
    private static extern int XGetWindowAttributes(
        IntPtr display,
        IntPtr window,
        out XWindowAttributes windowAttributes);

    [DllImport("libX11.so.6", EntryPoint = "XFetchName")]
    private static extern int XFetchName(IntPtr display, IntPtr window, out IntPtr windowName);

    [DllImport("libX11.so.6", EntryPoint = "XFree")]
    private static extern int XFree(IntPtr data);

    [DllImport("libX11.so.6", EntryPoint = "XChangeProperty")]
    private static extern int XChangeProperty(
        IntPtr display,
        IntPtr window,
        IntPtr property,
        IntPtr type,
        int format,
        int mode,
        IntPtr data,
        int nelements);

    [DllImport("libX11.so.6", EntryPoint = "XMapRaised")]
    private static extern int XMapRaised(IntPtr display, IntPtr window);

    [DllImport("libX11.so.6", EntryPoint = "XRaiseWindow")]
    private static extern int XRaiseWindow(IntPtr display, IntPtr window);

    [DllImport("libX11.so.6", EntryPoint = "XUnmapWindow")]
    private static extern int XUnmapWindow(IntPtr display, IntPtr window);

    [DllImport("libX11.so.6", EntryPoint = "XMoveWindow")]
    private static extern int XMoveWindow(IntPtr display, IntPtr window, int x, int y);

    [DllImport("libX11.so.6", EntryPoint = "XMoveResizeWindow")]
    private static extern int XMoveResizeWindow(IntPtr display, IntPtr window, int x, int y, uint width, uint height);

    [DllImport("libX11.so.6", EntryPoint = "XSendEvent")]
    private static extern int XSendEvent(
        IntPtr display,
        IntPtr window,
        [MarshalAs(UnmanagedType.Bool)] bool propagate,
        IntPtr eventMask,
        ref XEvent sendEvent);

    [DllImport("libX11.so.6", EntryPoint = "XFlush")]
    private static extern int XFlush(IntPtr display);

    [DllImport("libXrandr.so.2", EntryPoint = "XRRGetMonitors")]
    private static extern IntPtr XRRGetMonitors(IntPtr display, IntPtr window, [MarshalAs(UnmanagedType.Bool)] bool getActive, out int count);

    [DllImport("libXrandr.so.2", EntryPoint = "XRRFreeMonitors")]
    private static extern void XRRFreeMonitors(IntPtr monitors);

    [DllImport("libXext.so.6", EntryPoint = "XShapeCombineRectangles")]
    private static extern void XShapeCombineRectangles(
        IntPtr display,
        IntPtr dest,
        int destKind,
        int xOff,
        int yOff,
        IntPtr rectangles,
        int nRectangles,
        int op,
        int ordering);
}
