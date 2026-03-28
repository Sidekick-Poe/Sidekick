namespace Sidekick.Data.ItemDefinitions;

public class NinjaItemDefinition
{
    public required string Type { get; init; }
    public required string Url { get; init; }

    public NinjaStashDefinition? Stash { get; init; }
    public NinjaExchangeDefinition? Exchange { get; init; }
}
