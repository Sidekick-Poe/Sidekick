namespace Sidekick.Apis.Poe.Items;

public class ItemApiInformation
{
    public Category Category { get; set; } = Category.Unknown;

    public string? InvariantId { get; set; }

    public string? InvariantCategoryId { get; set; }

    public string? InvariantText { get; set; }

    public string? InvariantName { get; set; }

    public string? Image { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public string? Discriminator { get; set; }

    public string? Text { get; set; }

    public bool IsUnique { get; set; }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Name))
        {
            return $"{Type} - {Name}";
        }

        return !string.IsNullOrEmpty(Type) ? Type : Name;
    }
}
