using Sidekick.Common.Game.Items;

namespace Sidekick.Modules.Wealth.Models
{
    public partial class Item
    {
        public string Id { get; set; }

        public string Stash { get; set; }

        public string Name { get; set; }

        public Category Category { get; set; }

        public string Icon { get; set; }

        public string League { get; set; }

        public int Level { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

        public double Total { get; set; }

        public bool Removed { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
