using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonUi(this IServiceCollection services)
    {
        services.AddSingleton<IViewPreferenceService, ViewPreferenceService>();

        return services;
    }
}
