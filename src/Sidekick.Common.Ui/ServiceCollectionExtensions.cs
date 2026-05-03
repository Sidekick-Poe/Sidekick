using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sidekick.Common.Ui.Dialogs;
using Sidekick.Common.Ui.Sections;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonUi(this IServiceCollection services)
    {
        services.TryAddTransient<DialogResources>();
        services.TryAddSingleton<ISidekickDialogs, DialogService>();
        services.AddSingleton((sp) => (DialogService)sp.GetRequiredService<ISidekickDialogs>());

        services.TryAddScoped<ICurrentView, CurrentView>();

        services.TryAddSingleton<IViewPreferenceService, ViewPreferenceService>();
        services.TryAddSingleton<SectionService>();

        services.AddSidekickModule(typeof(ConfirmationDialog).Assembly);

        return services;
    }
}
