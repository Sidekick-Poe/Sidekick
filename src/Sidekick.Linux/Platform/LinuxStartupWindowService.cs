using Microsoft.Extensions.Logging;
using Sidekick.Common.Ui.Overlay;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Linux.Platform;

public sealed class LinuxStartupWindowService(
    LinuxStartupWindowOptions options,
    IViewLocator viewLocator,
    IOverlayVisibilityService overlayVisibilityService,
    IHostApplicationLifetime lifetime,
    ILogger<LinuxStartupWindowService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.StartupUrl))
        {
            return Task.CompletedTask;
        }

        var url = options.StartupUrl!.StartsWith('/')
            ? options.StartupUrl
            : $"/{options.StartupUrl}";

        lifetime.ApplicationStarted.Register(() =>
        {
            logger.LogInformation("[Linux] Launching startup window at {Url}", url);
            _ = Task.Run(async () =>
            {
                await Task.Delay(250, CancellationToken.None);
                await viewLocator.Open(url);
                overlayVisibilityService.SetOverlayVisible(true);
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
