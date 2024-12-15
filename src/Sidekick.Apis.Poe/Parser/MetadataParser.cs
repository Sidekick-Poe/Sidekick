using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;
using Sidekick.Common.Extensions;

namespace Sidekick.Apis.Poe.Parser
{
    public class MetadataParser(
        IGameLanguageProvider gameLanguageProvider,
        IParserPatterns parserPatterns,
        IMetadataProvider data,
        ISettingsService settingsService) : IItemMetadataParser
    {
        private Regex Affixes { get; set; } = null!;
        private Regex SuperiorAffix { get; set; } = null!;
        private GameType Game { get; set; }
        private ISettingsService SettingsService { get; } = settingsService;

        /// <inheritdoc/>
        public int Priority => 200;

        private string GetLineWithoutAffixes(string line) => Affixes
                                                             .Replace(line, string.Empty)
                                                             .Trim(' ', ',');

        public string GetLineWithoutSuperiorAffix(string line) => SuperiorAffix
                                                                  .Replace(line, string.Empty)
                                                                  .Trim(' ', ',');

        /// <inheritdoc/>
        public async Task Initialize()
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

            var leagueId = await SettingsService.GetString(SettingKeys.LeagueId);
            Game = leagueId.GetGameFromLeagueId();
        }

        public ItemMetadata? Parse(ParsingItem parsingItem)
        {
            var parsingBlock = parsingItem.Blocks.First();
            parsingBlock.Parsed = true;

            var itemRarity = GetRarity(parsingBlock);

            var canBeVaalGem = itemRarity == Rarity.Gem && parsingItem.Blocks.Count > 7;
            if (canBeVaalGem
                && data.NameAndTypeDictionary.TryGetValue(
                    parsingItem
                        .Blocks[5]
                        .Lines[0].Text,
                    out var vaalGem))
            {
                var vaalGemMetadata = vaalGem.First();
                return new ItemMetadata
                {
                    Id = vaalGemMetadata.Id,
                    Name = vaalGemMetadata.Name,
                    Type = vaalGemMetadata.Type,
                    ApiType = vaalGemMetadata.ApiType,
                    ApiTypeDiscriminator = vaalGemMetadata.ApiTypeDiscriminator,
                    Category = vaalGemMetadata.Category,
                    Rarity = vaalGemMetadata.Rarity,
                    Game = Game
                };
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

            // For magic items, strip off the suffix from the name
            if (itemRarity == Rarity.Magic && name != null)
            {
                var parts = name.Split(" of ");
                if (parts.Length > 1)
                {
                    name = parts[0];
                }
            }

            // Handle bow types
            if (parsingBlock.Lines[0].Text.Contains("Item Class: Bows") || 
                (parsingItem.Blocks.Count > 1 && parsingItem.Blocks[1].Lines.Any(x => x.Text == "Bow")))
            {
                return new ItemMetadata
                {
                    Id = "weapon.bow",
                    Type = name,  // For magic items, use the stripped name as the type
                    Name = name,
                    Category = Category.Weapon,
                    Rarity = itemRarity,
                    Game = GameType.PathOfExile2  // Always use trade2 API for bows
                };
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

            return new ItemMetadata
            {
                Id = result.Id,
                Name = result.Name,
                Type = result.Type,
                ApiType = result.ApiType,
                ApiTypeDiscriminator = result.ApiTypeDiscriminator,
                Category = result.Category,
                Rarity = result.Rarity,
                Game = Game
            };
        }

        public ItemMetadata? Parse(
            string? name,
            string? type)
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
                    results.AddRange(
                        data
                            .NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(name))
                            .Select(x => x.Item));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    results.AddRange(
                        data
                            .NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(type))
                            .Select(x => x.Item));
                }
            }

            ItemMetadata? result = null;

            if (results.Any(x => x.Type == type))
            {
                result = results.FirstOrDefault(x => x.Type == type);
            }
            else if (results.Any(x => x.ApiType == type))
            {
                result = results.FirstOrDefault(x => x.ApiType == type);
            }
            else if (results.Any(x => x.Rarity == Rarity.Unique))
            {
                result = results.FirstOrDefault(x => x.Rarity == Rarity.Unique);
            }
            else if (results.Any(x => x.Rarity == Rarity.Unknown))
            {
                result = results.FirstOrDefault(x => x.Rarity == Rarity.Unknown);
            }
            else
            {
                result = results.FirstOrDefault();
            }

            if (result == null)
            {
                return null;
            }

            return new ItemMetadata
            {
                Id = result.Id,
                Name = result.Name,
                Type = result.Type,
                ApiType = result.ApiType,
                ApiTypeDiscriminator = result.ApiTypeDiscriminator,
                Category = result.Category,
                Rarity = result.Rarity,
                Game = Game
            };
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
