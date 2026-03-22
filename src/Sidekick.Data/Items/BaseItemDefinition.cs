namespace Sidekick.Data.Items;

public class BaseItemDefinition
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public ItemClassDefinition2? ItemClass { get; init; }
    public ItemProperties? Properties { get; init; }
    public ItemRequirements? Requirements { get; init; }
}
