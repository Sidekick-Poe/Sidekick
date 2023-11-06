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

        public InvariantMetadataProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
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

            FillPattern(result.Result[0].Entries, Category.Accessory, useRegex: true);
            FillPattern(result.Result[1].Entries, Category.Armour, useRegex: true);
            FillPattern(result.Result[2].Entries, Category.DivinationCard);
            FillPattern(result.Result[3].Entries, Category.Currency);
            FillPattern(result.Result[4].Entries, Category.Flask, useRegex: true);
            FillPattern(result.Result[5].Entries, Category.Gem);
            FillPattern(result.Result[6].Entries, Category.Jewel, useRegex: true);
            FillPattern(result.Result[7].Entries, Category.Map, useRegex: true);
            FillPattern(result.Result[8].Entries, Category.Weapon, useRegex: true);
            FillPattern(result.Result[9].Entries, Category.Leaguestone);
            FillPattern(result.Result[10].Entries, Category.ItemisedMonster, useRegex: true);
            FillPattern(result.Result[11].Entries, Category.HeistEquipment, useRegex: true);
            FillPattern(result.Result[12].Entries, Category.Contract, useRegex: true);
            FillPattern(result.Result[13].Entries, Category.Logbook, useRegex: true);
            FillPattern(result.Result[14].Entries, Category.Sanctum, useRegex: true);
            FillPattern(result.Result[15].Entries, Category.Sentinel, useRegex: true);
            FillPattern(result.Result[16].Entries, Category.MemoryLine, useRegex: true);
        }

        private void FillPattern(List<ApiItem> items, Category category, bool useRegex = false)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];

                IdDictionary.Add($"{category}.{i}", new ItemMetadata()
                {
                    Id = $"{category}.{i}",
                    Name = item.Name,
                    Type = item.Type,
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
