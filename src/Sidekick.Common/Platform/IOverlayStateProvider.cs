namespace Sidekick.Common.Platform;

public interface IOverlayStateProvider
{
    bool HasOpenWidgets { get; }

    event Action? WidgetsChanged;
}
