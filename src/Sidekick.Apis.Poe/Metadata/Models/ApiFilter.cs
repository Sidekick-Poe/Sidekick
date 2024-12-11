namespace Sidekick.Apis.Poe.Metadata.Models;

public class ApiFilter
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public List<ApiFilterFilters> Filters { get; set; } = new();
}
