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

public class ApiInvariantItemProvider
(
    ICacheProvider cacheProvider,
    ITradeApiClient tradeApiClient,
    ILogger<ApiInvariantItemProvider> logger,
    IGameLanguageProvider gameLanguageProvider,
    ISettingsService settingsService
) : BaseItemProvider(logger), IApiInvariantItemProvider
{
    public ItemApiInformation? UncutSkillGem { get; private set; }
    public ItemApiInformation? UncutSupportGem { get; private set; }
    public ItemApiInformation? UncutSpiritGem { get; private set; }

    /// <inheritdoc/>
    public int Priority => 100;

    /// <inheritdoc/>
    public async Task Initialize()
    {
        var game = await settingsService.GetGame();
        var cacheKey = $"{game.GetValueAttribute()}_InvariantItems";

        var result = await cacheProvider.GetOrSet(cacheKey, () => tradeApiClient.FetchData<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "items"), (cache) => cache.Result.Any());
        if (result == null) throw new SidekickException("Could not fetch items from the trade API.");

        await InitializeLanguage(gameLanguageProvider.InvariantLanguage);
        InitializeItems(game, result);

        UncutSkillGem = NameAndTypeDictionary.GetValueOrDefault("Uncut Skill Gem")?.FirstOrDefault();
        UncutSpiritGem = NameAndTypeDictionary.GetValueOrDefault("Uncut Spirit Gem")?.FirstOrDefault();
        UncutSupportGem = NameAndTypeDictionary.GetValueOrDefault("Uncut Support Gem")?.FirstOrDefault();
    }
}
