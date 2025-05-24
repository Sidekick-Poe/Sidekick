using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;

namespace Sidekick.Apis.Poe.Trade.Trade.Requests;

public class Query
{
    public Status Status { get; set; } = new();

    public string? Name { get; set; }

    public object? Type { get; set; }

    public string? Term { get; set; }

    public List<StatFilterGroup> Stats { get; set; } = [];

    public SearchFilters Filters { get; set; } = new();
}
