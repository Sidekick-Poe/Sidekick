namespace Sidekick.Apis.Poe2Scout.Categories.Models;

public class ApiCategoriesResult
{
    public List<ScoutCategory> UniqueCategories { get; set; } = [];

    public List<ScoutCategory> CurrencyCategories { get; set; } = [];
}
