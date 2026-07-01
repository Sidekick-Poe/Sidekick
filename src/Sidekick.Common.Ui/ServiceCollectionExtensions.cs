using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Ui.Sections;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonUi(this IServiceCollection services)
    {
        services.TryAddScoped<ICurrentView, CurrentView>();

        services.TryAddSingleton<SectionService>();

        services.AddSidekickModule(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}
