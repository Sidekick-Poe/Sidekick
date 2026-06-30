using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Dialogs;

public static class StartupExtensions
{
    public static IServiceCollection AddSidekickCommonDialogs(this IServiceCollection services)
    {
        services.AddSingleton<BrowserDialogProvider>();
        services.AddSingleton<TransparentDialogProvider>();
        services.AddSingleton<DialogProvider>();
        return services;
    }
}
