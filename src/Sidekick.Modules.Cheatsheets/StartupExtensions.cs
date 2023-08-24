using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Cheatsheets.Keybinds;
using Sidekick.Modules.Cheatsheets.Localization;

namespace Sidekick.Modules.Cheatsheets
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickCheatsheets(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddTransient<CheatsheetResources>();

            services.AddSidekickKeybind<OpenCheatsheetsKeybindHandler>();

            return services;
        }
    }
}
