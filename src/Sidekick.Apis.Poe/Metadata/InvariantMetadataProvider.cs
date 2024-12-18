using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Metadata
{
    public class InvariantMetadataProvider
    (
        ICacheProvider cacheProvider,
        IPoeTradeClient poeTradeClient,
        ILogger<InvariantMetadataProvider> logger,
        IGameLanguageProvider gameLanguageProvider,
        ISettingsService settingsService
    ) : IInvariantMetadataProvider
    {
        public Dictionary<string, ItemMetadata> IdDictionary { get; } = new();

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            IdDictionary.Clear();

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var game = leagueId.GetGameFromLeagueId();
            var cacheKey = $"{game.GetValueAttribute()}_InvariantMetadata";

            var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.InvariantLanguage, "data/items"));

            var categories = game switch
            {
                GameType.PathOfExile2 => MetadataConstants.Poe2Categories,
                _ => MetadataConstants.Poe1Categories,
            };
            foreach (var category in categories)
            {
                FillPattern(game, result.Result, category.Key, category.Value.Category);
            }
        }

        private void FillPattern(GameType game, List<ApiCategory> categories, string id, Category category)
        {
            var categoryItems = categories.SingleOrDefault(x => x.Id == id);

            if (categoryItems == null)
            {
                logger.LogWarning($"[MetadataProvider] The category '{id}' could not be found in the metadata from the API.");
                return;
            }

            var items = categoryItems.Entries;

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];

                IdDictionary.Add($"{category}.{i}",
                                 new ItemMetadata()
                                 {
                                     Id = $"{category}.{i}",
                                     Name = item.Name,
                                     Type = item.Text ?? item.Type,
                                     ApiType = item.Type,
                                     Rarity = GetRarityForCategory(category, item),
                                     Category = category,
                                     Game = game,
                                 });
            }
        }

        private static Rarity GetRarityForCategory(Category category, ApiItem item)
        {
            if (item.Flags?.Unique ?? false)
            {
                return Rarity.Unique;
            }

            return category switch
            {
                Category.DivinationCard => Rarity.DivinationCard,
                Category.Gem => Rarity.Gem,
                Category.Currency => Rarity.Currency,
                _ => Rarity.Unknown
            };
        }
    }
}
