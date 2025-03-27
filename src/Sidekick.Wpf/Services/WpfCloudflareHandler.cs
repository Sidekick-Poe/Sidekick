using System.Windows;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.CloudFlare;
using Sidekick.Common.Initialization;
using Sidekick.Wpf.Cloudflare;

namespace Sidekick.Wpf.Services;

public class WpfCloudflareHandler
(
    ICloudflareService cloudflareService,
    ILogger<WpfCloudflareHandler> logger
) : IDisposable
{
    public void Initialize()
    {
        cloudflareService.ChallengeStarted += CloudflareServiceOnChallengeStarted;
    }

    private void CloudflareServiceOnChallengeStarted(Uri uri)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var window = new CloudflareWindow(logger, cloudflareService, uri);
            window.Show();
        });
    }

    public void Dispose()
    {
        cloudflareService.ChallengeStarted -= CloudflareServiceOnChallengeStarted;
    }
}
