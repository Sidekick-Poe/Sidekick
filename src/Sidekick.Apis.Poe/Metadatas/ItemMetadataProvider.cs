using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.Metadatas.Models;
using Sidekick.Apis.Poe.Parser;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Common.Cache;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Metadatas
{
    public class ItemMetadataProvider : IItemMetadataProvider
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly IPoeTradeClient poeTradeClient;
        private readonly IParserPatterns parserPatterns;

        public ItemMetadataProvider(
            ICacheProvider cacheProvider,
            IGameLanguageProvider gameLanguageProvider,
            IPoeTradeClient poeTradeClient,
            IParserPatterns parserPatterns)
        {
            this.cacheProvider = cacheProvider;
            this.gameLanguageProvider = gameLanguageProvider;
            this.poeTradeClient = poeTradeClient;
            this.parserPatterns = parserPatterns;
        }

        private Dictionary<string, List<ItemMetadata>> NameAndTypeDictionary { get; } = new();
        private List<(Regex Regex, ItemMetadata Item)> NameAndTypeRegex { get; } = new();
        private Regex Prefixes { get; set; } = null!;

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        private string GetLineWithoutPrefixes(string line) => Prefixes.Replace(line, string.Empty);

        /// <inheritdoc/>
        public async Task Initialize()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Item Metadata] Could not find a valid language.");
            }

            NameAndTypeDictionary.Clear();
            NameAndTypeRegex.Clear();

            var result = await cacheProvider.GetOrSet(
                "ItemMetadataProvider",
                () => poeTradeClient.Fetch<ApiCategory>("data/items"));

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

            Prefixes = new Regex("^(?:" +
                gameLanguageProvider.Language.PrefixSuperior + " |" +
                gameLanguageProvider.Language.PrefixBlighted + " |" +
                gameLanguageProvider.Language.PrefixBlightRavaged + " |" +
                gameLanguageProvider.Language.PrefixAnomalous + " |" +
                gameLanguageProvider.Language.PrefixDivergent + " |" +
                gameLanguageProvider.Language.PrefixPhantasmal + " )");
        }

        private void FillPattern(List<ApiItem> items, Category category, bool useRegex = false)
        {
            foreach (var item in items)
            {
                var header = new ItemMetadata()
                {
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

        public ItemMetadata? Parse(ParsingItem parsingItem)
        {
            var parsingBlock = parsingItem.Blocks.First();
            var itemRarity = GetRarity(parsingBlock);

            // If we find a Vaal Gem, we don't care about any other results
            if (itemRarity == Rarity.Gem &&
                parsingItem.Blocks.Count > 7 && // If the items has more than 7 blocks, it could be a vaal gem
                NameAndTypeDictionary.TryGetValue(parsingItem.Blocks[5].Lines[0].Text, out var vaalGem)) // The vaal gem name is always at the same position
            {
                parsingBlock.Parsed = true;
                return vaalGem.First();
            }

            // Get name and type text
            string? name = null;
            string? type = null;
            if (parsingBlock.Lines.Count >= 4)
            {
                name = parsingBlock.Lines[2].Text;
                type = parsingBlock.Lines[3].Text;
            }
            else if (parsingBlock.Lines.Count == 3)
            {
                type = parsingBlock.Lines[2].Text;
                name = parsingBlock.Lines[2].Text;
            }

            // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
            if (itemRarity == Rarity.Rare || itemRarity == Rarity.Magic)
            {
                name = null;
            }

            // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
            var results = new List<ItemMetadata>();

            // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
            if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(name, out var itemData))
            {
                results.AddRange(itemData);
            }
            // Here we check without any prefixes
            else if (!string.IsNullOrEmpty(name) && NameAndTypeDictionary.TryGetValue(GetLineWithoutPrefixes(name), out itemData))
            {
                results.AddRange(itemData);
            }
            // Now we check the type
            else if (!string.IsNullOrEmpty(type) && NameAndTypeDictionary.TryGetValue(type, out itemData))
            {
                results.AddRange(itemData);
            }
            // Finally. if we don't have any matches, we will look into our regex dictionary
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    results.AddRange(NameAndTypeRegex
                        .Where(pattern => pattern.Regex.IsMatch(name))
                        .Select(x => x.Item));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    results.AddRange(NameAndTypeRegex
                        .Where(pattern => pattern.Regex.IsMatch(type))
                        .Select(x => x.Item));
                }
            }

            // If we have a matching type, we narrow down our results
            if (results.Any(x => x.Type == type))
            {
                results = results.Where(x => x.Type == type).ToList();
            }

            // If we have a Unique item in our results, we sort for it so it comes first
            var result = results
                .OrderBy(x => x.Rarity == Rarity.Unique ? 0 : 1)
                .ThenBy(x => x.Rarity == Rarity.Unknown ? 0 : 1)
                .FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            parsingBlock.Parsed = true;

            // If we don't have the rarity from the metadata, we set it to the value from the text
            if (result.Rarity == Rarity.Unknown)
            {
                result.Rarity = itemRarity;
            }

            if (result.Category == Category.ItemisedMonster && result.Rarity == Rarity.Unique && string.IsNullOrEmpty(result.Name))
            {
                result.Name = name;
            }

            if (result.Class == Class.Undefined)
            {
                result.Class = GetClass(parsingBlock);
            }

            return result;
        }

        private Rarity GetRarity(ParsingBlock parsingBlock)
        {
            foreach (var pattern in parserPatterns.Rarity)
            {
                if (pattern.Value.IsMatch(parsingBlock.Lines[1].Text))
                {
                    return pattern.Key;
                }
            }

            throw new NotSupportedException("Item rarity is unknown.");
        }

        private Class GetClass(ParsingBlock parsingBlock)
        {
            foreach (var pattern in parserPatterns.Classes)
            {
                if (pattern.Value.IsMatch(parsingBlock.Lines[0].Text))
                {
                    return pattern.Key;
                }
            }

            return Class.Undefined;
        }
    }
}
