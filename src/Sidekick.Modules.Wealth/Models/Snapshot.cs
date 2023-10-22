using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidekick.Modules.Wealth.Models
{
    public class Snapshot
    {

        public int SnapshotId { get; set; }

        public int RunId { get; set; }

        public string StashId { get; set; }

        public double Total { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
