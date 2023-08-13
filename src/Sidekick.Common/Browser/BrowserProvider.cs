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
            logger.LogInformation("Opening in browser: {uri}", uri.AbsoluteUri);
            var psi = new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
