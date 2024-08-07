using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadatas.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public class InvariantMetadataProvider : IInvariantMetadataProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly ILogger<InvariantMetadataProvider> logger;

        public InvariantMetadataProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient,
            ILogger<InvariantMetadataProvider> logger)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
            this.logger = logger;
        }

        public Dictionary<string, ItemMetadata> IdDictionary { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            IdDictionary.Clear();

            var result = await cacheProvider.GetOrSet(
                "InvariantMetadata",
                () => poeTradeClient.Fetch<ApiCategory>("data/items", useDefaultLanguage: true));

            FillPattern(result.Result, "accessory", Category.Accessory, useRegex: true);
            FillPattern(result.Result, "armour", Category.Armour, useRegex: true);
            FillPattern(result.Result, "card", Category.DivinationCard);
            FillPattern(result.Result, "currency", Category.Currency);
            FillPattern(result.Result, "flask", Category.Flask, useRegex: true);
            FillPattern(result.Result, "gem", Category.Gem);
            FillPattern(result.Result, "jewel", Category.Jewel, useRegex: true);
            FillPattern(result.Result, "map", Category.Map, useRegex: true);
            FillPattern(result.Result, "weapon", Category.Weapon, useRegex: true);
            FillPattern(result.Result, "leaguestone", Category.Leaguestone);
            FillPattern(result.Result, "monster", Category.ItemisedMonster, useRegex: true);
            FillPattern(result.Result, "heistequipment", Category.HeistEquipment, useRegex: true);
            FillPattern(result.Result, "heistmission", Category.Contract, useRegex: true);
            FillPattern(result.Result, "logbook", Category.Logbook, useRegex: true);
            FillPattern(result.Result, "sanctum", Category.Sanctum, useRegex: true);
            FillPattern(result.Result, "memoryline", Category.MemoryLine, useRegex: true);
            FillPattern(result.Result, "tincture", Category.Tincture, useRegex: true);
            FillPattern(result.Result, "corpse", Category.Corpse, useRegex: true);
        }

        private void FillPattern(List<ApiCategory> categories, string id, Category category, bool useRegex = false)
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
