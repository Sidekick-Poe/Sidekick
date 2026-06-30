namespace Sidekick.Data.Trade;

public class TradeStatDefinition
{
    public required string Id { get; init; }

    public required string Text { get; init; }

    public string? OptionText { get; init; }
}
