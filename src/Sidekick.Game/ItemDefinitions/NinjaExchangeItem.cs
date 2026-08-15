namespace Sidekick.Game.ItemDefinitions;

public class NinjaExchangeItem
{
    public required string Type { get; init; }
    public required string Url { get; init; }

    public string? Id { get; init; }
    public string? DetailsId { get; init; }

    public override string ToString()
    {
        return DetailsId ?? string.Empty;
    }
}
