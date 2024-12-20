using Microsoft.EntityFrameworkCore;
using Sidekick.Common.Database.Converters;
using Sidekick.Common.Database.Tables;

namespace Sidekick.Common.Database;

public class SidekickDbContext : DbContext
{
    private static bool hasMigrated;

    public SidekickDbContext(DbContextOptions<SidekickDbContext> options)
        : base(options)
    {
        if (hasMigrated)
        {
            return;
        }

        Database.Migrate();
        hasMigrated = true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType
                    .ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    modelBuilder
                        .Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToUtcTicksConverter());
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Setting> Settings { get; init; }

    public DbSet<WealthItem> WealthItems { get; init; }

    public DbSet<WealthStash> WealthStashes { get; init; }

    public DbSet<WealthStashSnapshot> WealthStashSnapshots { get; init; }

    public DbSet<WealthFullSnapshot> WealthFullSnapshots { get; init; }

    public DbSet<ViewPreference> ViewPreferences { get; init; }
}
