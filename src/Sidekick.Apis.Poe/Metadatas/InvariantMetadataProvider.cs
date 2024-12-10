using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadatas.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public class InvariantMetadataProvider(
        ICacheProvider cacheProvider,
        IPoeTradeClient poeTradeClient,
        ILogger<InvariantMetadataProvider> logger,
        IGameLanguageProvider gameLanguageProvider) : IInvariantMetadataProvider
    {
        public Dictionary<string, ItemMetadata> IdDictionary { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            IdDictionary.Clear();

            var result = await cacheProvider.GetOrSet(
                "InvariantMetadata",
                () => poeTradeClient.Fetch<ApiCategory>(GameType.PathOfExile, gameLanguageProvider.InvariantLanguage, "data/items"));

            FillPattern(result.Result, "accessory", Category.Accessory);
            FillPattern(result.Result, "armour", Category.Armour);
            FillPattern(result.Result, "card", Category.DivinationCard);
            FillPattern(result.Result, "currency", Category.Currency);
            FillPattern(result.Result, "flask", Category.Flask);
            FillPattern(result.Result, "gem", Category.Gem);
            FillPattern(result.Result, "jewel", Category.Jewel);
            FillPattern(result.Result, "map", Category.Map);
            FillPattern(result.Result, "weapon", Category.Weapon);
            FillPattern(result.Result, "leaguestone", Category.Leaguestone);
            FillPattern(result.Result, "monster", Category.ItemisedMonster);
            FillPattern(result.Result, "heistequipment", Category.HeistEquipment);
            FillPattern(result.Result, "heistmission", Category.Contract);
            FillPattern(result.Result, "logbook", Category.Logbook);
            FillPattern(result.Result, "sanctum", Category.Sanctum);
            FillPattern(result.Result, "memoryline", Category.MemoryLine);
            FillPattern(result.Result, "tincture", Category.Tincture);
            FillPattern(result.Result, "corpse", Category.Corpse);
        }

        private void FillPattern(List<ApiCategory> categories, string id, Category category)
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

                IdDictionary.Add($"{category}.{i}", new ItemMetadata()
                {
                    Id = $"{category}.{i}",
                    Name = item.Name,
                    Type = item.Text ?? item.Type,
                    ApiType = item.Type,
                    Rarity = GetRarityForCategory(category, item),
                    Category = category,
                    Game = GameType.PathOfExile,
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
