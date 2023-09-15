using System.Text.Json.Serialization;

namespace Sidekick.Apis.Poe.Trade.Filters
{
    internal class TradeFilter
    {
        public SearchFilterOption? Account { get; set; }

        [JsonPropertyName("indexed")]
        public SearchFilterOption? Listed { get; set; }

        public SearchFilterValue Price { get; set; } = new();

        [JsonPropertyName("sale_type")]
        public SearchFilterOption SaleType { get; set; } = new("priced");
    }
}
