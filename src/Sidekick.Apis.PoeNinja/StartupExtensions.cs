using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.PoeNinja.Api;
using Sidekick.Apis.PoeNinja.Repository;
using Sidekick.Common;

namespace Sidekick.Apis.PoeNinja
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddTransient<IPoeNinjaApiClient, PoeNinjaApiClient>();
            services.AddTransient<IPoeNinjaRepository, PoeNinjaRepository>();

            services.AddSidekickInitializableService<IPoeNinjaClient, PoeNinjaClient>();

            return services;
        }
    }
}
