using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Headers;
using Sidekick.Apis.Poe.Parser.Metadata;
using Sidekick.Apis.Poe.Parser.Modifiers;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Parser.Properties;
using Sidekick.Apis.Poe.Parser.Sockets;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser
{
    public class ItemParser
    (
        ILogger<ItemParser> logger,
        IMetadataParser metadataProvider,
        IModifierParser modifierParser,
        IPseudoModifierProvider pseudoModifierProvider,
        IParserPatterns patterns,
        ClusterJewelParser clusterJewelParser,
        IInvariantMetadataProvider invariantMetadataProvider,
        ISocketParser socketParser,
        IPropertyParser propertyParser,
        IHeaderParser headerParser
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
                var metadata = metadataProvider.Parse(parsingItem);
                if (metadata == null || (string.IsNullOrEmpty(metadata.Name) && string.IsNullOrEmpty(metadata.Type)))
                {
                    throw new UnparsableException();
                }

                // Strip the Superior affix from the name
                parsingItem.Blocks.First()
                    .Lines.ForEach(x =>
                    {
                        x.Text = metadataProvider.GetLineWithoutSuperiorAffix(x.Text);
                    });

                parsingItem.Metadata = metadata;
                ItemMetadata? invariant = null;
                if (invariantMetadataProvider.IdDictionary.TryGetValue(metadata.Id, out var invariantMetadata))
                {
                    invariant = invariantMetadata;
                }

                // Order of parsing is important    
                ParseRequirements(parsingItem);

                var header = headerParser.Parse(parsingItem);
                var influences = ParseInfluences(parsingItem);
                var sockets = socketParser.Parse(parsingItem);
                var modifierLines = ParseModifiers(parsingItem);
                var properties = propertyParser.Parse(parsingItem, modifierLines);
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
                throw;
            }
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

        #endregion Helpers
    }
}
