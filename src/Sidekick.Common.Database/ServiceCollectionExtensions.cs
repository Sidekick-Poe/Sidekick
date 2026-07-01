using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sidekick.Common.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSidekickCommonDatabase(this IServiceCollection services, string databasePath)
    {
        services.AddDbContextPool<SidekickDbContext>(o => o.UseSqlite("Data Source=" + databasePath));
        return services;
    }

    public static void UseSidekickCommonDatabase(this IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SidekickDbContext>();
        // dbContext.Database.Migrate();
    }
}
