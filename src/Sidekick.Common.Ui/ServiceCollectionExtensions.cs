using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common.Ui.Dialogs;
using Sidekick.Common.Ui.Sections;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Common.Ui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonUi(this IServiceCollection services)
    {
        services.AddTransient<DialogResources>();
        services.AddSingleton<ISidekickDialogs, DialogService>();
        services.AddSingleton((sp) => (DialogService)sp.GetRequiredService<ISidekickDialogs>());

        services.AddScoped<ICurrentView, CurrentView>();

        services.AddSingleton<IViewPreferenceService, ViewPreferenceService>();
        services.AddSingleton<SectionService>();

        services.AddSidekickModule(typeof(ConfirmationDialog).Assembly);

        return services;
    }
}
