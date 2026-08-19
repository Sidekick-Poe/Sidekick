namespace Sidekick.Game.Ninja;

public class NinjaExchangeItem
{
    public required string Type { get; init; }
    public required string Url { get; init; }
    public required string DetailsId { get; init; }

    public string? Id { get; init; }

    public override string ToString()
    {
        return DetailsId;
    }
}
