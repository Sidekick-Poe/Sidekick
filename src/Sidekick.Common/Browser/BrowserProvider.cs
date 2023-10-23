using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Sidekick.Common.Browser
{
    public class BrowserProvider : IBrowserProvider
    {
        private readonly ILogger<BrowserProvider> logger;

        public BrowserProvider(ILogger<BrowserProvider> logger)
        {
            this.logger = logger;
        }

        public void OpenUri(Uri uri)
        {
            logger.LogInformation("[Browser] Opening: {uri}", uri.AbsoluteUri);

            Process.Start(new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            });
        }

        public void OpenSidekickWebsite()
        {
            OpenUri(new Uri("https://sidekick-poe.github.io/"));
        }
    }
}
