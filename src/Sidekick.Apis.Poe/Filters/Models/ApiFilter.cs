namespace Sidekick.Apis.Poe.Filters.Models;

public class ApiFilter
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public List<ApiFilters> Filters { get; set; } = new();
}
