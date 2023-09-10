using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Apis.PoeNinja
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickPoeNinjaApi(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IPoeNinjaClient, PoeNinjaClient>();

            return services;
        }
    }
}
