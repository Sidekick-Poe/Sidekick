using Sidekick.Apis.Poe2Scout.Categories.Models;
namespace Sidekick.Apis.Poe2Scout.Categories;

public interface IScoutCategoryProvider
{
    Task<List<ScoutCategory>> GetUniqueCategories();
    Task<List<ScoutCategory>> GetCurrencyCategories();
    Uri GetUri(string? category);
}
