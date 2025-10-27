using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Clients.Models;
using Sidekick.Apis.Poe.Trade.Items.Models;
using Sidekick.Apis.Poe.Trade.Static;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Items;

public class ApiItemProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    ILogger<ApiItemProvider> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService,
    IApiStaticDataProvider apiStaticDataProvider
) : IApiItemProvider
{
    public Dictionary<string, List<(Regex Regex, ItemApiInformation Item)>> NameDictionary { get; private set; } = [];

    public List<(Regex Regex, ItemApiInformation Item)> TypePatterns { get; private set; } = [];

    public List<(Regex Regex, ItemApiInformation Item)> TextPatterns { get; private set; } = [];

    public List<ItemApiInformation> UniqueItems { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 200;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_Items";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.Language, "items"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch items from the trade API.");

        InitializeItems(game, result);
        UniqueItems = NameDictionary.SelectMany(x => x.Value)
            .Select(x => x.Item)
            .Where(x => x.IsUnique)
            .OrderByDescending(x => x.Name?.Length)
            .ToList();

        TypePatterns = TypePatterns.OrderByDescending(x => x.Item.Type?.Length ?? 0).ToList();
        TextPatterns = TextPatterns.OrderByDescending(x => x.Item.Text?.Length ?? 0).ToList();
    }

    private void InitializeItems(GameType game, FetchResult<ApiCategory> result)
    {
        NameDictionary.Clear();
        TypePatterns.Clear();
        TextPatterns.Clear();

        var categories = game switch
        {
            GameType.PathOfExile2 => ApiItemConstants.Poe2Categories,
            _ => ApiItemConstants.Poe1Categories,
        };

        foreach (var category in categories)
        {
            FillCategoryItems(result.Result, category.Key, category.Value);
        }
    }

    private void FillCategoryItems(List<ApiCategory> categories, string categoryId, Category category)
    {
        var categoryItems = categories.SingleOrDefault(x => x.Id == categoryId);
        if (categoryItems == null)
        {
            logger.LogWarning($"[MetadataProvider] The category '{categoryId}' could not be found in the metadata from the API.");
            return;
        }

        foreach (var entry in categoryItems.Entries)
        {
            var information = entry.ToItemApiInformation();
            information.Category = category;

            var apiData = apiStaticDataProvider.Get(information.Name, information.Type);
            information.InvariantId = apiData?.Id;
            information.InvariantText = apiData?.Text;
            information.Image = apiData?.Image;

            if (!string.IsNullOrEmpty(information.Name))
            {
                if (!NameDictionary.TryGetValue(information.Name, out var namePatterns))
                {
                    namePatterns = new List<(Regex Regex, ItemApiInformation Item)>();
                    NameDictionary.Add(information.Name, namePatterns);
                }

                namePatterns.Add((new Regex(Regex.Escape(information.Name)), information));
            }

            if (!string.IsNullOrEmpty(information.Type) && !information.IsUnique)
            {
                TypePatterns.Add((new Regex(Regex.Escape(information.Type)), information));
            }

            if (!string.IsNullOrEmpty(information.Text))
            {
                TextPatterns.Add((new Regex(Regex.Escape(information.Text)), information));
            }
        }
    }
}
