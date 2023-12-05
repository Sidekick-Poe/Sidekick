using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sidekick.Modules.Wealth.Models
{
    internal class WealthDbDesignTime : IDesignTimeDbContextFactory<WealthDbContext>
    {
        public WealthDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<WealthDbContext>();
            builder.UseSqlite("Filename=Wealth.db");
            return new WealthDbContext(builder.Options);
        }
    }
}
