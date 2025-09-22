namespace Sidekick.Apis.Poe.Items;

public class ItemHeader
{
    public Category Category { get; set; } = Category.Unknown;

    public string? ApiItemId { get; set; }

    public string? ApiName { get; set; }

    public string? ApiType { get; set; }

    public string? ApiDiscriminator { get; set; }

    public string? ApiText { get; set; }

    /// <inheritdoc />
    public override string? ToString()
    {
        if (!string.IsNullOrEmpty(ApiType) && !string.IsNullOrEmpty(ApiName))
        {
            return $"{ApiType} - {ApiName}";
        }

        return !string.IsNullOrEmpty(ApiType) ? ApiType : ApiName;
    }
}
