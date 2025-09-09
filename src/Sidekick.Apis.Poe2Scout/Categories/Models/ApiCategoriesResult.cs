using System.Text.Json.Serialization;
namespace Sidekick.Apis.Poe2Scout.Categories.Models;

public class ApiCategoriesResult
{
    [JsonPropertyName("unique_categories")]
    public List<ScoutCategory> UniqueCategories { get; set; } = [];

    [JsonPropertyName("currency_categories")]
    public List<ScoutCategory> CurrencyCategories { get; set; } = [];
}
