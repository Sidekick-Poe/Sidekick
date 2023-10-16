using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Modules.Wealth.Models
{
    public partial class Stash
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public double Total { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
