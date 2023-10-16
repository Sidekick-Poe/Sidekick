using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sidekick.Common.Game.Items;

namespace Sidekick.Modules.Wealth.Models
{
    public class WealthDbContext : DbContext
    {
        private static bool _created = false;
        public WealthDbContext()
        {
            Database.Migrate();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
        {
            optionbuilder.UseSqlite("Data Source=./Database/Wealth.db");
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Stash> Stashes { get; set; }
    }
}
