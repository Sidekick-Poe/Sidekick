using Sidekick.Common.Initialization;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Overlay;
using Sidekick.Common.Ui.Views;
using Microsoft.Extensions.Logging;

namespace Sidekick.Linux.Platform;

public sealed class LinuxOverlayFocusWatcher(
    IProcessProvider processProvider,
    IViewLocator viewLocator,
    IOverlayVisibilityService overlayVisibilityService,
    ILogger<LinuxOverlayFocusWatcher> logger) : IInitializableService, IDisposable
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);
    private Timer? timer;
    private bool overlayInitialized;
    private bool overlayVisible;
    private bool inTick;

    public int Priority => 120;

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
        if (inTick)
        {
            return;
        }

        inTick = true;
        try
        {
            var shouldShow = processProvider.IsPathOfExileInFocus || processProvider.IsSidekickInFocus;
            if (shouldShow && !overlayInitialized)
            {
                overlayInitialized = true;
                _ = viewLocator.Open("/overlay");
            }

            if (shouldShow != overlayVisible)
            {
                overlayVisible = shouldShow;
                overlayVisibilityService.SetOverlayVisible(shouldShow);
                logger.LogDebug("[Overlay] Visibility set to {Visible}", shouldShow);
            }
        }
        finally
        {
            inTick = false;
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
        timer = null;
    }
}
