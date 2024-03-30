using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadatas.Models;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public class MetadataProvider : IMetadataProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly ILogger<MetadataProvider> logger;

        public MetadataProvider(
            ICacheProvider cacheProvider,
            IPoeTradeClient poeTradeClient,
            ILogger<MetadataProvider> logger)
        {
            this.cacheProvider = cacheProvider;
            this.poeTradeClient = poeTradeClient;
            this.logger = logger;
        }

        public Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; } = new();
        public List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            NameAndTypeDictionary.Clear();
            NameAndTypeRegex.Clear();

            var result = await cacheProvider.GetOrSet(
                "Metadata",
                () => poeTradeClient.Fetch<ApiCategory>("data/items"));

            FillPattern(result.Result, "accessories", Category.Accessory, useRegex: true);
            FillPattern(result.Result, "armour", Category.Armour, useRegex: true);
            FillPattern(result.Result, "cards", Category.DivinationCard);
            FillPattern(result.Result, "currency", Category.Currency);
            FillPattern(result.Result, "flasks", Category.Flask, useRegex: true);
            FillPattern(result.Result, "gems", Category.Gem);
            FillPattern(result.Result, "jewels", Category.Jewel, useRegex: true);
            FillPattern(result.Result, "maps", Category.Map, useRegex: true);
            FillPattern(result.Result, "weapons", Category.Weapon, useRegex: true);
            FillPattern(result.Result, "leaguestones", Category.Leaguestone);
            FillPattern(result.Result, "monsters", Category.ItemisedMonster, useRegex: true);
            FillPattern(result.Result, "heistequipment", Category.HeistEquipment, useRegex: true);
            FillPattern(result.Result, "heistmission", Category.Contract, useRegex: true);
            FillPattern(result.Result, "logbook", Category.Logbook, useRegex: true);
            FillPattern(result.Result, "sanctum", Category.Sanctum, useRegex: true);
            FillPattern(result.Result, "memoryline", Category.MemoryLine, useRegex: true);
            FillPattern(result.Result, "azmeri", Category.Affliction, useRegex: true);
            FillPattern(result.Result, "necropolis", Category.EmbersOfTheAllflame, useRegex: true);
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

                var header = new ItemMetadata()
                {
                    Id = $"{category}.{i}",
                    Name = item.Name,
                    Type = item.Type,
                    Rarity = GetRarityForCategory(category, item),
                    Category = category,
                };

                var key = header.Name ?? header.Type;
                if (key == null)
                {
                    continue;
                }

                // If the item is unique, exclude it from the regex dictionary
                if (header.Rarity == Rarity.Unique)
                {
                    FillDictionary(header, key);
                    continue;
                }

                if (useRegex)
                {
                    NameAndTypeRegex.Add((new Regex(Regex.Escape(key)), header));
                }

                FillDictionary(header, key);
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
