using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Requests.Filters
{
    internal class TypeFilters
    {
        public SearchFilterOption? Category { get; set; }

        public SearchFilterOption? Rarity { get; set; }

        /// <remarks>
        /// The item level filter for Path of Exile 2 is inside the type filters instead of the misc filters.
        /// </remarks>
        [JsonPropertyName("ilvl")]
        public StatFilterValue? ItemLevel { get; set; }
    }
}
