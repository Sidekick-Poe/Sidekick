using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.PoeWiki
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeWikiApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeWikiClient, PoeWikiClient>();
            services.AddSingleton<IPoeWikiDataProvider, PoeWikiDataProvider>();

            return services;
        }
    }
}
