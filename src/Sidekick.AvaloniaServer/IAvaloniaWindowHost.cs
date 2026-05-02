using Sidekick.Common.Ui.Views;

namespace Sidekick.AvaloniaServer;

/// <summary>
/// Provides window-level operations from the Blazor Server layer back to the Avalonia host.
/// Implemented by AvaloniaViewLocator in the Sidekick.Avalonia project.
/// </summary>
public interface IAvaloniaWindowHost
{
    void MinimizeWindow(SidekickViewType type);
    void MaximizeWindow(SidekickViewType type);
}
