using System.Collections.Generic;

namespace Sidekick.Apis.PoeNinja.Api.Models
{
    public class SparkLine
    {
        public double TotalChange { get; set; }

        public List<double?> Data { get; set; }
    }
}
