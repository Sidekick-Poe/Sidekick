using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Common;
using Sidekick.Modules.Wealth.Models;

namespace Sidekick.Modules.Wealth
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickWealth(this IServiceCollection services)
        {
            services.AddSidekickModule(typeof(StartupExtensions).Assembly);

            services.AddSingleton<WealthParser>();
            services.AddDbContextPool<WealthDbContext>(o => o.UseSqlite("Data Source=" + SidekickPaths.GetDataFilePath("wealth.db")));

            return services;
        }
    }
}
