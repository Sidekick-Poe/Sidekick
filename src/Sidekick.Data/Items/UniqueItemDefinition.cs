namespace Sidekick.Data.Items;

public class UniqueItemDefinition
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? Image { get; init; }
    public ItemClassDefinition? ItemClass { get; init; }
}