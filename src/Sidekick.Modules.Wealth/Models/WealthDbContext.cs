using Microsoft.EntityFrameworkCore;

namespace Sidekick.Modules.Wealth.Models
{
    public class WealthDbContext : DbContext
    {
        public WealthDbContext(DbContextOptions<WealthDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Stash> Stashes { get; set; }
        public DbSet<Snapshot> Snapshots { get; set; }
    }
}
