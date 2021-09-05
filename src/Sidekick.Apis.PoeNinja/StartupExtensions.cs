using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Api;
using Sidekick.Apis.PoeNinja.Repository;

namespace Sidekick.Apis.PoeNinja
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeNinjaApiClient, PoeNinjaApiClient>();
            services.AddTransient<IPoeNinjaRepository, PoeNinjaRepository>();
            services.AddTransient<IPoeNinjaClient, PoeNinjaClient>();

            return services;
        }
    }
}
