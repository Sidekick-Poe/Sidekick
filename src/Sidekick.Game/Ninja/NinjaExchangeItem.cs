namespace Sidekick.Game.Ninja;

public class NinjaExchangeItem
{
    public bool Active { get; set; }

    public required string DetailsId { get; init; }

    public required string Type { get; init; }

    public string? Url { get; set; }

    public string? Name { get; set; }

    public string? TradeId { get; set; }

    public override string ToString()
    {
        return DetailsId;
    }
}
