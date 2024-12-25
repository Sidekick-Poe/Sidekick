using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Browser;

public class BrowserProvider(ILogger<BrowserProvider> logger) : IBrowserProvider
{
    public void OpenUri(Uri uri)
    {
        logger.LogInformation("[Browser] Opening: {uri}", uri.AbsoluteUri);

        Process.Start(
            new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true,
            });
    }

    public void OpenSidekickWebsite()
    {
        OpenUri(new Uri("https://sidekick-poe.github.io/"));
    }

    public void OpenGitHubRepository()
    {
        OpenUri(new Uri("https://github.com/Sidekick-Poe/Sidekick"));
    }
}
