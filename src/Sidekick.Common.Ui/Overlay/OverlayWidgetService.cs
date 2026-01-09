using System.Threading;
using Microsoft.Extensions.Logging;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Ui.Overlay;

public sealed class OverlayWidgetService(
    ISettingsService settingsService,
    ILogger<OverlayWidgetService> logger) : IInitializableService
{
    private const int DefaultWidgetWidth = 600;
    private const int DefaultWidgetHeight = 420;

    private readonly object syncLock = new();
    private readonly List<OverlayWidgetState> widgets = [];
    private readonly Dictionary<string, OverlayWidgetLayout> layouts = new();
    private Timer? saveTimer;
    private bool initialized;
    private int viewportWidth;
    private int viewportHeight;

    public event Action? WidgetsChanged;

    public int Priority => 150;

    public IReadOnlyList<OverlayWidgetState> Widgets
    {
        get
        {
            lock (syncLock)
            {
                return widgets.ToList();
            }
        }
    }

    public async Task Initialize()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        var savedLayouts = await settingsService.GetObject(
            SettingKeys.OverlayWidgetLayouts,
            () => new List<OverlayWidgetLayout>());

        foreach (var layout in savedLayouts)
        {
            if (!layouts.ContainsKey(layout.Key))
            {
                layouts.Add(layout.Key, layout);
            }
        }
    }

    public void SetViewportSize(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            return;
        }

        lock (syncLock)
        {
            viewportWidth = width;
            viewportHeight = height;
        }
    }

    public OverlayWidgetState OpenWidget(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("Widget url must be set.", nameof(url));
        }

        lock (syncLock)
        {
            var layoutKey = GetLayoutKey(url);
            var layout = GetLayout(layoutKey, DefaultWidgetWidth, DefaultWidgetHeight);
            var widget = new OverlayWidgetState
            {
                Id = Guid.NewGuid(),
                Key = layoutKey,
                Title = BuildTitle(url),
                Url = url,
                X = layout.X,
                Y = layout.Y,
                Width = layout.Width,
                Height = layout.Height
            };

            widgets.Add(widget);
            WidgetsChanged?.Invoke();
            return widget;
        }
    }

    public void CloseWidget(Guid widgetId)
    {
        lock (syncLock)
        {
            var widget = widgets.FirstOrDefault(x => x.Id == widgetId);
            if (widget == null)
            {
                return;
            }

            widgets.Remove(widget);
            WidgetsChanged?.Invoke();
        }
    }

    public void ClearWidgets()
    {
        lock (syncLock)
        {
            widgets.Clear();
            WidgetsChanged?.Invoke();
        }
    }

    public void UpdateBounds(Guid widgetId, int x, int y, int width, int height)
    {
        lock (syncLock)
        {
            var widget = widgets.FirstOrDefault(item => item.Id == widgetId);
            if (widget == null)
            {
                return;
            }

            widget.X = x;
            widget.Y = y;
            widget.Width = width;
            widget.Height = height;

            if (!string.IsNullOrEmpty(widget.Key))
            {
                layouts[widget.Key] = new OverlayWidgetLayout(widget.Key, x, y, width, height);
                ScheduleSave();
            }
        }
    }

    private OverlayWidgetLayout GetLayout(string? key, int width, int height)
    {
        if (!string.IsNullOrEmpty(key) && layouts.TryGetValue(key, out var layout))
        {
            return layout;
        }

        var (x, y) = GetCenteredPosition(width, height);
        return new OverlayWidgetLayout(key ?? string.Empty, x, y, width, height);
    }

    private static string? GetLayoutKey(string url)
    {
        if (!url.StartsWith('/'))
        {
            return null;
        }

        var trimmed = url.Trim('/');
        if (string.IsNullOrEmpty(trimmed))
        {
            return null;
        }

        return trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
    }

    private (int X, int Y) GetCenteredPosition(int width, int height)
    {
        if (viewportWidth <= 0 || viewportHeight <= 0)
        {
            return (24, 24);
        }

        var x = Math.Max(0, (viewportWidth - width) / 2);
        var y = Math.Max(0, (viewportHeight - height) / 2);
        return (x, y);
    }

    private void ScheduleSave()
    {
        saveTimer?.Dispose();
        saveTimer = new Timer(
            _ => SaveLayouts(),
            null,
            TimeSpan.FromSeconds(1),
            Timeout.InfiniteTimeSpan);
    }

    private void SaveLayouts()
    {
        List<OverlayWidgetLayout> snapshot;
        lock (syncLock)
        {
            snapshot = layouts.Values.ToList();
        }

        try
        {
            settingsService.Set(SettingKeys.OverlayWidgetLayouts, snapshot).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Overlay] Failed to persist widget layout.");
        }
    }

    private static string BuildTitle(string url)
    {
        if (!url.StartsWith('/'))
        {
            return "Sidekick";
        }

        var trimmed = url.Trim('/');
        if (string.IsNullOrEmpty(trimmed))
        {
            return "Sidekick";
        }

        var parts = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts[0] switch
        {
            "trade" => "Price Check",
            "map" => "Map Check",
            "wealth" => "Wealth",
            "settings" => "Settings",
            "initialize" => "Initialization",
            _ => "Sidekick"
        };
    }

    public sealed class OverlayWidgetState
    {
        public Guid Id { get; init; }
        public string? Key { get; init; }
        public string Title { get; init; } = "Sidekick";
        public string? Url { get; init; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public sealed record OverlayWidgetLayout(
        string Key,
        int X,
        int Y,
        int Width,
        int Height);
}
