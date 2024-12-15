using System.Globalization;
using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser
{
    public class ItemParser
    (
        ILogger<ItemParser> logger,
        IItemMetadataParser itemMetadataProvider,
        IModifierParser modifierParser,
        IPseudoModifierProvider pseudoModifierProvider,
        IParserPatterns patterns,
        ClusterJewelParser clusterJewelParser,
        IInvariantMetadataProvider invariantMetadataProvider,
        SocketParser socketParser,
        PropertyParser propertyParser,
        IMetadataProvider metadataProvider,
        IGameLanguageProvider gameLanguageProvider
    ) : IItemParser
    {
        public Task<Item> ParseItemAsync(string itemText)
        {
            return Task.Run(() => ParseItem(itemText));
        }

        public Item ParseItem(string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
            {
                throw new UnparsableException();
            }

            try
            {
                var parsingItem = new ParsingItem(itemText);
                var metadata = itemMetadataProvider.Parse(parsingItem);
                if (metadata == null || (string.IsNullOrEmpty(metadata.Name) && string.IsNullOrEmpty(metadata.Type)))
                {
                    throw new UnparsableException("Item was not found in the metadata provider.");
                }

                // Strip the Superior affix from the name
                parsingItem.Blocks.First()
                    .Lines.ForEach(x =>
                    {
                        x.Text = itemMetadataProvider.GetLineWithoutSuperiorAffix(x.Text);
                    });

                parsingItem.Metadata = metadata;
                ItemMetadata? invariant = null;
                if (invariantMetadataProvider.IdDictionary.TryGetValue(metadata.Id, out var invariantMetadata))
                {
                    invariant = invariantMetadata;
                }

                // Order of parsing is important    
                ParseRequirements(parsingItem);

                var header = ParseHeader(parsingItem);
                var properties = propertyParser.Parse(parsingItem);
                var influences = ParseInfluences(parsingItem);
                var sockets = socketParser.Parse(parsingItem);
                var modifierLines = ParseModifiers(parsingItem);
                var pseudoModifiers = parsingItem.Metadata.Game == GameType.PathOfExile ? ParsePseudoModifiers(modifierLines) : [];
                var item = new Item(metadata: metadata,
                                    invariant: invariant,
                                    header: header,
                                    properties: properties,
                                    influences: influences,
                                    sockets: sockets,
                                    modifierLines: modifierLines,
                                    pseudoModifiers: pseudoModifiers,
                                    text: parsingItem.Text);

                if (clusterJewelParser.TryParse(item, out var clusterInformation))
                {
                    item.AdditionalInformation = clusterInformation;
                }

                return item;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Could not parse item.");
                throw new UnparsableException();
            }
        }

        public Header ParseHeader(string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
            {
                throw new UnparsableException();
            }

            try
            {
                return ParseHeader(new ParsingItem(itemText));
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Could not parse item.");
                throw new UnparsableException();
            }
        }

        private Header ParseHeader(ParsingItem parsingItem)
        {
            var firstLine = parsingItem.Blocks[0].Lines[0].Text;
            string? apiItemCategoryId = null;

            if (firstLine.StartsWith(gameLanguageProvider.Language.Classes.Prefix))
            {
                var classLine = firstLine.Replace(gameLanguageProvider.Language.Classes.Prefix + ":", "").Trim();
                
                // Direct mapping for known item classes
                apiItemCategoryId = classLine switch
                {
                    "Bows" => "weapon.bow",
                    "Misc Map Items" => "map.fragment",
                    _ => null
                };

                // Fallback to fuzzy matching if no direct match
                if (apiItemCategoryId == null)
                {
                    var categoryToMatch = new ApiFilterOption { Text = classLine };
                    apiItemCategoryId = Process.ExtractOne(categoryToMatch, metadataProvider.ApiItemCategories, x => x.Text, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;
                }
            }

            return new Header()
            {
                Name = parsingItem.Blocks[0].Lines.ElementAtOrDefault(2)?.Text,
                Type = parsingItem.Blocks[0].Lines.ElementAtOrDefault(3)?.Text,
                ItemCategory = apiItemCategoryId
            };
        }

        private void ParseRequirements(ParsingItem parsingItem)
        {
            foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
            {
                if (!block.TryParseRegex(patterns.Requirements, out _))
                {
                    continue;
                }

                block.Parsed = true;
                return;
            }
        }

        private Influences ParseInfluences(ParsingItem parsingItem)
        {
            return parsingItem.Metadata?.Category switch
            {
                Category.Accessory or Category.Armour or Category.Weapon => new Influences()
                {
                    Crusader = GetBool(patterns.Crusader, parsingItem),
                    Elder = GetBool(patterns.Elder, parsingItem),
                    Hunter = GetBool(patterns.Hunter, parsingItem),
                    Redeemer = GetBool(patterns.Redeemer, parsingItem),
                    Shaper = GetBool(patterns.Shaper, parsingItem),
                    Warlord = GetBool(patterns.Warlord, parsingItem),
                },
                _ => new Influences(),
            };
        }

        private List<ModifierLine> ParseModifiers(ParsingItem parsingItem)
        {
            return parsingItem.Metadata?.Category switch
            {
                Category.DivinationCard or Category.Gem => new(),
                _ => modifierParser.Parse(parsingItem),
            };
        }

        private List<PseudoModifier> ParsePseudoModifiers(List<ModifierLine> modifierLines)
        {
            if (modifierLines.Count == 0)
            {
                return new();
            }

            return pseudoModifierProvider.Parse(modifierLines);
        }

        #region Helpers

        private static bool GetBool(Regex pattern, ParsingItem parsingItem)
        {
            return parsingItem.TryParseRegex(pattern, out _);
        }

        private static bool TrySetAdditionalInformation(Item item, object? additionalInformation)
        {
            if (additionalInformation == null)
            {
                return false;
            }

            item.AdditionalInformation = additionalInformation;
            return true;
        }

        #endregion Helpers
    }
}
