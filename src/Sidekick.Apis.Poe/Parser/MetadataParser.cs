using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser
{
    public class MetadataParser
    (
        IGameLanguageProvider gameLanguageProvider,
        IParserPatterns parserPatterns,
        IMetadataProvider data
    ) : IItemMetadataParser
    {
        private Regex Affixes { get; set; } = null!;

        private Regex SuperiorAffix { get; set; } = null!;

        /// <inheritdoc/>
        public int Priority => 200;

        private string GetLineWithoutAffixes(string line) => Affixes.Replace(line, string.Empty).Trim(' ', ',');

        public string GetLineWithoutSuperiorAffix(string line) => SuperiorAffix.Replace(line, string.Empty).Trim(' ', ',');

        /// <inheritdoc/>
        public Task Initialize()
        {
            var getRegexLine = (string input) =>
            {
                if (input.StartsWith('/'))
                {
                    input = input.Trim('/');
                    return new Regex($"^{input} | {input}$");
                }

                input = Regex.Escape(input);
                return new Regex($"^{input} | {input}$");
            };

            Affixes = new Regex("(?:" + getRegexLine(gameLanguageProvider.Language.AffixSuperior) + "|" + getRegexLine(gameLanguageProvider.Language.AffixBlighted) + "|" + getRegexLine(gameLanguageProvider.Language.AffixBlightRavaged) + "|" + getRegexLine(gameLanguageProvider.Language.AffixAnomalous) + "|" + getRegexLine(gameLanguageProvider.Language.AffixDivergent) + "|" + getRegexLine(gameLanguageProvider.Language.AffixPhantasmal) + ")");
            SuperiorAffix = new Regex("(?:" + getRegexLine(gameLanguageProvider.Language.AffixSuperior) + ")");

            return Task.CompletedTask;
        }

        public ItemMetadata? Parse(ParsingItem parsingItem)
        {
            var parsingBlock = parsingItem.Blocks.First();
            parsingBlock.Parsed = true;

            var itemRarity = GetRarity(parsingBlock);

            var canBeVaalGem = itemRarity == Rarity.Gem && parsingItem.Blocks.Count > 7;
            if (canBeVaalGem && data.NameAndTypeDictionary.TryGetValue(parsingItem.Blocks[5].Lines[0].Text, out var vaalGem))
            {
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
                name = parsingBlock.Lines[2].Text;
                type = parsingBlock.Lines[2].Text;
            }
            else if (parsingBlock.Lines.Count == 2)
            {
                name = parsingBlock.Lines[1].Text;
                type = parsingBlock.Lines[0].Text;
            }

            // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
            if (itemRarity is Rarity.Rare or Rarity.Magic)
            {
                name = null;
            }

            var result = Parse(name, type);
            if (result == null)
            {
                return null;
            }

            // If we don't have the rarity from the metadata, we set it to the value from the text
            if (result.Rarity == Rarity.Unknown)
            {
                result.Rarity = itemRarity;
            }

            if (result.Rarity == Rarity.Unique)
            {
                result.Type = result.ApiType;
            }

            if (result.Category == Category.ItemisedMonster && result.Rarity == Rarity.Unique && string.IsNullOrEmpty(result.Name))
            {
                result.Name = name;
            }

            return result;
        }

        public ItemMetadata? Parse(string? name, string? type)
        {
            // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
            var results = new List<ItemMetadata>();

            // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
            if (!string.IsNullOrEmpty(name) && data.NameAndTypeDictionary.TryGetValue(name, out var itemData))
            {
                results.AddRange(itemData);
            }

            // Here we check without any prefixes
            else if (!string.IsNullOrEmpty(name) && data.NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(name), out itemData))
            {
                results.AddRange(itemData);
            }

            // Now we check the type
            else if (!string.IsNullOrEmpty(type) && data.NameAndTypeDictionary.TryGetValue(type, out itemData))
            {
                results.AddRange(itemData);
            }

            // Finally. if we don't have any matches, we will look into our regex dictionary
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    results.AddRange(data.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(name)).Select(x => x.Item));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    results.AddRange(data.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(type)).Select(x => x.Item));
                }
            }

            var orderedResults = results.OrderByDescending(x=>x.Type?.Length ?? x.Name?.Length ?? 0).ToList();

            if (orderedResults.Any(x => x.Type == type))
            {
                return orderedResults.FirstOrDefault(x => x.Type == type);
            }

            if (orderedResults.Any(x => x.ApiType == type))
            {
                return orderedResults.FirstOrDefault(x => x.ApiType == type);
            }

            if (orderedResults.Any(x => x.Rarity == Rarity.Unique))
            {
                return orderedResults.FirstOrDefault(x => x.Rarity == Rarity.Unique);
            }

            if (orderedResults.Any(x => x.Rarity == Rarity.Unknown))
            {
                return orderedResults.FirstOrDefault(x => x.Rarity == Rarity.Unknown);
            }

            return orderedResults.FirstOrDefault();
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

            return Rarity.Unknown;
        }
    }
}
