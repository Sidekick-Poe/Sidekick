namespace Sidekick.Apis.Poe.Models;

public class ItemApiInformation
{
    public string? ApiItemId { get; init; }

    public string? ApiName { get; init; }

    public string? ApiType { get; init; }

    public string? ApiDiscriminator { get; init; }

    public string? ApiText { get; init; }

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
