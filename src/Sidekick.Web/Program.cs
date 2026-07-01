using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common;
using Sidekick.Common.Browser;
using Sidekick.Common.Platform;
using Sidekick.Common.Ui.Views;
using Sidekick.Web;
using Sidekick.Web.Services;
using Velopack;

VelopackApp.Build().Run();

var host = new ServerAppHost(SidekickApplicationType.Web);
host.Start(services =>
{
    services.TryAddSingleton<IViewLocator, WebViewLocator>();
    services.TryAddSingleton(sp => (WebViewLocator)sp.GetRequiredService<IViewLocator>());
    services.AddSidekickInitializableService<IApplicationService, WebApplicationService>();
});

// Open the browser
var applicationLifetime = host.Application.Services.GetRequiredService<IHostApplicationLifetime>();
applicationLifetime.ApplicationStarted.Register(() =>
{
    var browserProvider = host.Application.Services.GetService<IBrowserProvider>();

    // Get the first URL the app is listening on
    var url = host.Application.Urls.FirstOrDefault();
    if (!string.IsNullOrEmpty(url) && browserProvider != null)
    {
        browserProvider.OpenUri(new Uri(url));
    }
});

await host.RunTask;
