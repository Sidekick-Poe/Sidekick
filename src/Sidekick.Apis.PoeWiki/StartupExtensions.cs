using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;

namespace Sidekick.Apis.PoeWiki
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeWikiApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeWikiClient, PoeWikiClient>();

            services.AddSidekickInitializableService<IPoeWikiDataProvider, PoeWikiDataProvider>();

            return services;
        }
    }
}
