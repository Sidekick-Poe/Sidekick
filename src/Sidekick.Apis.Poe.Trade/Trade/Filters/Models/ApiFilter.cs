namespace Sidekick.Apis.Poe.Trade.Trade.Filters.Models;

public class ApiFilter
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public ApiFilterOptions Option { get; set; } = new();
}
