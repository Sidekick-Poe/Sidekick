using System.Collections.Generic;

namespace Sidekick.Apis.PoeNinja.Api.Models
{
    public class PoeNinjaCacheItem<T>
    {
        public string Type { get; set; }

        public List<T> Items { get; set; }

        // public Dictionary<string, string> Translations { get; set; }
    }
}
