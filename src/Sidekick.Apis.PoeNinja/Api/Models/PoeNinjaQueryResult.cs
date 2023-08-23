using System.Collections.Generic;

namespace Sidekick.Apis.PoeNinja.Api.Models
{
    public class PoeNinjaQueryResult<T>
    {
        public List<T> Lines { get; set; }
    }
}
