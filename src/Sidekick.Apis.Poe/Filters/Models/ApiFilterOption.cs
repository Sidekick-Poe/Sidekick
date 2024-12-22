namespace Sidekick.Apis.Poe.Filters.Models;

public class ApiFilterOption
{
    public string? Id { get; set; }
    public string? Text { get; set; }

    public override string ToString() => Text ?? Id ?? "";
}
