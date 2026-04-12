namespace Sidekick.Data.ItemDefinitions;

public class TradeItemDefinition
{
    public string? Id { get; init; }
    public string? Text { get; init; }
    public string? Image { get; init; }

    public string? Name { get; init; }
    public string? Type { get; init; }
    public string? Category { get; init; }
    public string? Discriminator { get; init; }
}