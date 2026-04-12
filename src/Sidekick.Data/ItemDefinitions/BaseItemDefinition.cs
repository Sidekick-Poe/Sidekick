namespace Sidekick.Data.ItemDefinitions;

public class BaseItemDefinition
{
    public string? Id { get; init; }
    public string? Name { get; init; }
    public string? ItemClassId { get; init; }
    public BaseItemProperties? Properties { get; init; }
    public BaseItemRequirements? Requirements { get; init; }
}
