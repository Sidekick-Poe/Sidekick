namespace Sidekick.Apis.Poe2Scout.Api;

internal record Poe2ScoutItem
{
    /// <summary>
    /// UniqueItemResponse.
    /// </summary>
    public string? Name { get; init; }

    public string? Type { get; init; }

    /// <summary>
    /// CurrencyItemResponse.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// Same categories as the official API, except for waystones.
    /// </summary>
    public string? CategoryApiId { get; init; }

    public decimal CurrentPrice { get; init; }

    public bool IsChanceable { get; init; }

    public List<Poe2ScoutPriceLog?>? PriceLogs { get; init; }
}
