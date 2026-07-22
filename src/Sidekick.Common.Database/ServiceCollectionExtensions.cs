using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SidekickDbContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = sp.GetRequiredService<ILogger<SidekickDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}
