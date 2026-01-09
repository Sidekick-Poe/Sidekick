using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Overlay;

namespace Sidekick.Linux.Platform;

public sealed class OverlayStateProvider : IOverlayStateProvider, IDisposable
{
    private readonly OverlayWidgetService widgetService;

    public OverlayStateProvider(OverlayWidgetService widgetService)
    {
        this.widgetService = widgetService;
        widgetService.WidgetsChanged += OnWidgetsChanged;
    }

    public bool HasOpenWidgets => widgetService.Widgets.Count > 0;

    public event Action? WidgetsChanged;

    public void Dispose()
    {
        widgetService.WidgetsChanged -= OnWidgetsChanged;
    }

    private void OnWidgetsChanged()
    {
        WidgetsChanged?.Invoke();
    }
}
