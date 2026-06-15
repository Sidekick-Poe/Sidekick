using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Ui.Sections;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonUi(this IServiceCollection services)
    {
        services.AddScoped<ICurrentView, CurrentView>();

        services.AddSingleton<IViewPreferenceService, ViewPreferenceService>();
        services.AddSingleton<SectionService>();

        services.AddSidekickModule(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}
