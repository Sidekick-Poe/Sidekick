using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Browser;

public class BrowserProvider(ILogger<BrowserProvider> logger, ISidekickDialogs dialogs) : IBrowserProvider
{
    public void OpenUri(Uri uri)
    {
        logger.LogInformation("[Browser] Opening: {uri}", uri.AbsoluteUri);

        try
        {
            throw new Exception();
            Process.Start(new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Browser] Failed to open URL: {uri}", uri.AbsoluteUri);
            dialogs.OpenOkModal("Failed to open URL: " + uri.AbsoluteUri);
        }
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
