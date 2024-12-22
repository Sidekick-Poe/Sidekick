using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Metadata
{
    public class MetadataProvider
    (
        ICacheProvider cacheProvider,
        IPoeTradeClient poeTradeClient,
        ILogger<MetadataProvider> logger,
        IGameLanguageProvider gameLanguageProvider,
        ISettingsService settingsService
    ) : IMetadataProvider
    {
        public Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; } = new();

        public List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; } = new();

        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            NameAndTypeDictionary.Clear();
            NameAndTypeRegex.Clear();

            var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
            var game = leagueId.GetGameFromLeagueId();
            var cacheKey = $"{game.GetValueAttribute()}_Metadata";

            var result = await cacheProvider.GetOrSet(cacheKey, () => poeTradeClient.Fetch<ApiCategory>(game, gameLanguageProvider.Language, "data/items"), (cache) => cache.Result.Any());

            var categories = game switch
            {
                GameType.PathOfExile2 => MetadataConstants.Poe2Categories,
                _ => MetadataConstants.Poe1Categories,
            };
            foreach (var category in categories)
            {
                FillPattern(game, result.Result, category.Key, category.Value.Category, category.Value.UseRegex);
            }
        }

        private void FillPattern(GameType game, List<ApiCategory> categories, string id, Category category, bool useRegex = false)
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
                var header = new ItemMetadata()
                {
                    Id = $"{category}.{i}",
                    Name = item.Name,
                    Type = item.Text ?? item.Type,
                    ApiType = item.Type,
                    ApiTypeDiscriminator = item.Discriminator,
                    Rarity = GetRarityForCategory(category, item),
                    Category = category,
                    Game = game,
                };

                var key = header.Name ?? header.Type ?? header.ApiType;
                if (key == null)
                {
                    continue;
                }

                FillDictionary(header, key);

                if (header.Rarity != Rarity.Unique && useRegex)
                {
                    NameAndTypeRegex.Add((new Regex(Regex.Escape(key)), header));
                }
            }
        }

        private void FillDictionary(ItemMetadata metadata, string key)
        {
            if (!NameAndTypeDictionary.TryGetValue(key, out var dictionaryEntry))
            {
                dictionaryEntry = new List<ItemMetadata>();
                NameAndTypeDictionary.Add(key, dictionaryEntry);
            }

            dictionaryEntry.Add(metadata);
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
