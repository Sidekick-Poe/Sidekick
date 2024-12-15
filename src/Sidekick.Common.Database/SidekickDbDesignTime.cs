using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sidekick.Common.Database;

internal class SidekickDbDesignTime : IDesignTimeDbContextFactory<SidekickDbContext>
{
    public SidekickDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SidekickDbContext>();
        builder.UseSqlite("Filename=sidekick.db");
        return new SidekickDbContext(builder.Options);
    }
}
