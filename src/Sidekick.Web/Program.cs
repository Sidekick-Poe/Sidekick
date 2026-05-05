using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Web;
using Velopack;

VelopackApp.Build().Run();

var host = new ServerAppHost(SidekickApplicationType.Web);
var runTask = host.Start();

// Open the browser
var applicationLifetime = host.Application.Services.GetRequiredService<IHostApplicationLifetime>();
applicationLifetime.ApplicationStarted.Register(() =>
{
    var browserProvider = host.Application.Services.GetService<IBrowserProvider>();
    if (browserProvider == null) return;

    // Get the first URL the app is listening on
    var url = host.Application.Urls.FirstOrDefault();
    if (!string.IsNullOrEmpty(url))
    {
        browserProvider.OpenUri(new Uri(url));
    }
});

await runTask;