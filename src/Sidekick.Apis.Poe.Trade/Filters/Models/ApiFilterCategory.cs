namespace Sidekick.Apis.Poe.Trade.Filters.Models;

public class ApiFilterCategory
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public List<ApiFilter> Filters { get; set; } = new();

}
