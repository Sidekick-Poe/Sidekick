using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sidekick.Common.Browser;
using Sidekick.Common.Cache;
using Sidekick.Common.Folder;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Localization;
using Sidekick.Common.Logging;
using Sidekick.Common.Platform.Input;
using Sidekick.Common.Settings;

namespace Sidekick.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickPoeApi(
        this IServiceCollection services)
    {
        services.AddSidekickInitializableService<IGameLanguageProvider, GameLanguageProvider>();

        return services;
    }
}
