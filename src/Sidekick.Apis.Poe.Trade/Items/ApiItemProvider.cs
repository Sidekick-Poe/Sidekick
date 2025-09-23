using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Extensions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Clients;
using Sidekick.Apis.Poe.Trade.Items.Models;
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
    ISettingsService settingsService
) : BaseItemProvider(logger), IApiItemProvider
{
    public List<ItemApiInformation> UniqueItems { get; private set; } = [];

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_Items";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.Language, "items"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch items from the trade API.");

        await InitializeLanguage(gameLanguageProvider.Language);
        InitializeItems(game, result);
        UniqueItems = NameAndTypeDictionary.Values.SelectMany(x => x).Where(x => x.IsUnique).OrderByDescending(x => x.Name?.Length).ToList();
    }
}
