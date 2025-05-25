using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Browser;

public class BrowserProvider(ILogger<BrowserProvider> logger, IServiceProvider serviceProvider) : IBrowserProvider
{
    public Uri SidekickWebsite => new("https://sidekick-poe.github.io/");

    public Uri GitHubRepository => new("https://github.com/Sidekick-Poe/Sidekick");

    public Uri DiscordServer => new("https://discord.gg/R9HyCpV");

    public void OpenUri(Uri uri)
    {
        logger.LogInformation("[Browser] Opening: {uri}", uri.AbsoluteUri);

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Browser] Failed to open URL: {uri}", uri.AbsoluteUri);
            var dialogs = serviceProvider.GetService<ISidekickDialogs>();
            dialogs?.OpenOkModal("Failed to open URL: " + uri.AbsoluteUri);
        }
    }
}
