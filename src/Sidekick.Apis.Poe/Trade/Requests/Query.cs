using Sidekick.Apis.Poe.Trade.Filters;

namespace Sidekick.Apis.Poe.Trade.Requests
{
    internal class Query
    {
        public Status Status { get; set; } = new();
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Term { get; set; }
        public List<StatFilterGroup> Stats { get; set; } = new();
        public SearchFilters Filters { get; set; } = new();
    }
}
