using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Database
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSidekickDatabase(this IServiceCollection services)
        {
            services.AddDbContextPool<SidekickDbContext>(o => o.UseSqlite("Data Source=" + SidekickPaths.GetDataFilePath("sidekick.db")));

            return services;
        }
    }
}
