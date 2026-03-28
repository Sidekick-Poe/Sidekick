namespace Sidekick.Data.ItemDefinitions;

public class BaseItemDefinition
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public ItemClassDefinition? ItemClass { get; init; }
    public BaseItemProperties? Properties { get; init; }
    public BaseItemRequirements? Requirements { get; init; }
}
