using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Dialogs.Browsers;
using Sidekick.Common.Dialogs.Transparent;

namespace Sidekick.Common.Dialogs;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonDialogs(this IServiceCollection services)
    {
        services.AddSingleton<BrowserWindowProvider>();
        services.AddSingleton<TransparentWindowProvider>();
        return services;
    }
}
