using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Parser
{
    public class ItemParser : IItemParser
    {
        private readonly ILogger<ItemParser> logger;
        private readonly IItemMetadataParser itemMetadataProvider;
        private readonly IModifierParser modifierParser;
        private readonly IPseudoModifierProvider pseudoModifierProvider;
        private readonly IParserPatterns patterns;
        private readonly ClusterJewelParser clusterJewelParser;

        public ItemParser(
            ILogger<ItemParser> logger,
            IItemMetadataParser itemMetadataProvider,
            IModifierParser modifierParser,
            IPseudoModifierProvider pseudoModifierProvider,
            IParserPatterns patterns,
            ClusterJewelParser clusterJewelParser)
        {
            this.logger = logger;
            this.itemMetadataProvider = itemMetadataProvider;
            this.modifierParser = modifierParser;
            this.pseudoModifierProvider = pseudoModifierProvider;
            this.patterns = patterns;
            this.clusterJewelParser = clusterJewelParser;
        }

        public Task<Item?> ParseItemAsync(string itemText)
        {
            return Task.Run(() => ParseItem(itemText));
        }

        public Item? ParseItem(string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
            {
                return null;
            }

            try
            {
                var parsingItem = new ParsingItem(itemText);
                var metadata = itemMetadataProvider.Parse(parsingItem);
                if (metadata == null || (string.IsNullOrEmpty(metadata?.Name) && string.IsNullOrEmpty(metadata?.Type)))
                {
                    throw new NotSupportedException("Item not found.");
                }

                parsingItem.Metadata = metadata;
                ParseRequirements(parsingItem);

                // Order of parsing is important
                var header = ParseHeader(parsingItem);
                var properties = ParseProperties(parsingItem);
                var influences = ParseInfluences(parsingItem);
                var sockets = ParseSockets(parsingItem);
                var modifierLines = ParseModifiers(parsingItem);
                var pseudoModifiers = ParsePseudoModifiers(modifierLines);
                var item = new Item(
                    metadata: metadata,
                    header: header,
                    properties: properties,
                    influences: influences,
                    sockets: sockets,
                    modifierLines: modifierLines,
                    pseudoModifiers: pseudoModifiers,
                    text: parsingItem.Text);

                clusterJewelParser.Parse(item);

                return item;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Could not parse item.");
                return null;
            }
        }

        public Header? ParseHeader(string itemText)
        {
            if (string.IsNullOrEmpty(itemText))
            {
                return null;
            }

            try
            {
                return ParseHeader(new ParsingItem(itemText));
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Could not parse item.");
                return null;
            }
        }

        private static Header ParseHeader(ParsingItem parsingItem)
        {
            return new Header()
            {
                Name = parsingItem.Blocks[0].Lines.ElementAtOrDefault(2)?.Text,
                Type = parsingItem.Blocks[0].Lines.ElementAtOrDefault(3)?.Text,
            };
        }

        private void ParseRequirements(ParsingItem parsingItem)
        {
            foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
            {
                if (TryParseValue(patterns.Requirements, block, out var match))
                {
                    block.Parsed = true;
                    return;
                }
            }
        }

        private Properties ParseProperties(ParsingItem parsingItem)
        {
            return parsingItem.Metadata?.Category switch
            {
                Category.Gem => ParseGemProperties(parsingItem),
                Category.Map or Category.Contract => ParseMapProperties(parsingItem),
                Category.Accessory => ParseAccessoryProperties(parsingItem),
                Category.Armour => ParseArmourProperties(parsingItem),
                Category.Weapon => ParseWeaponProperties(parsingItem),
                Category.Jewel => ParseJewelProperties(parsingItem),
                Category.Flask => ParseFlaskProperties(parsingItem),
                Category.Sanctum => ParseSanctumProperties(parsingItem),
                _ => new Properties(),
            };
        }

        private Properties ParseWeaponProperties(ParsingItem parsingItem)
        {
            var propertyBlock = parsingItem.Blocks[1];

            var properties = new Properties
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Scourged = GetBool(patterns.Scourged, parsingItem),

                Quality = GetInt(patterns.Quality, propertyBlock),
                AttacksPerSecond = GetDouble(patterns.AttacksPerSecond, propertyBlock),
                CriticalStrikeChance = GetDouble(patterns.CriticalStrikeChance, propertyBlock)
            };

            properties.ElementalDps = GetDps(patterns.ElementalDamage, propertyBlock, properties.AttacksPerSecond);
            properties.PhysicalDps = GetDps(patterns.PhysicalDamage, propertyBlock, properties.AttacksPerSecond);
            properties.DamagePerSecond = properties.ElementalDps + properties.PhysicalDps;

            return properties;
        }

        private Properties ParseArmourProperties(ParsingItem parsingItem)
        {
            var propertyBlock = parsingItem.Blocks[1];

            return new Properties()
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Scourged = GetBool(patterns.Scourged, parsingItem),

                Quality = GetInt(patterns.Quality, propertyBlock),
                Armor = GetInt(patterns.Armor, propertyBlock),
                EnergyShield = GetInt(patterns.EnergyShield, propertyBlock),
                Evasion = GetInt(patterns.Evasion, propertyBlock),
                ChanceToBlock = GetInt(patterns.ChanceToBlock, propertyBlock),
            };
        }

        private Properties ParseAccessoryProperties(ParsingItem parsingItem)
        {
            return new Properties()
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Scourged = GetBool(patterns.Scourged, parsingItem),
            };
        }

        private Properties ParseMapProperties(ParsingItem parsingItem)
        {
            var propertyBlock = parsingItem.Blocks[1];

            return new Properties()
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Scourged = GetBool(patterns.Scourged, parsingItem),
                Blighted = patterns.Blighted.IsMatch(parsingItem.Blocks[0].Lines[^1].Text),
                BlightRavaged = patterns.BlightRavaged.IsMatch(parsingItem.Blocks[0].Lines[^1].Text),

                ItemQuantity = GetInt(patterns.ItemQuantity, propertyBlock),
                ItemRarity = GetInt(patterns.ItemRarity, propertyBlock),
                MonsterPackSize = GetInt(patterns.MonsterPackSize, propertyBlock),
                MapTier = GetInt(patterns.MapTier, propertyBlock),
                Quality = GetInt(patterns.Quality, propertyBlock),
            };
        }

        private Properties ParseGemProperties(ParsingItem parsingItem)
        {
            var propertyBlock = parsingItem.Blocks[1];

            return new Properties()
            {
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Scourged = GetBool(patterns.Scourged, parsingItem),

                GemLevel = GetInt(patterns.Level, propertyBlock),
                Quality = GetInt(patterns.Quality, propertyBlock),
                AlternateQuality = GetBool(patterns.AlternateQuality, parsingItem),
            };
        }

        private Properties ParseJewelProperties(ParsingItem parsingItem)
        {
            return new Properties()
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
            };
        }

        private Properties ParseFlaskProperties(ParsingItem parsingItem)
        {
            return new Properties()
            {
                ItemLevel = GetInt(patterns.ItemLevel, parsingItem),
                Identified = !GetBool(patterns.Unidentified, parsingItem),
                IsRelic = GetBool(patterns.IsRelic, parsingItem),
                Corrupted = GetBool(patterns.Corrupted, parsingItem),
                Quality = GetInt(patterns.Quality, parsingItem),
            };
        }

        private Properties ParseSanctumProperties(ParsingItem parsingItem)
        {
            return new Properties
            {
                AreaLevel = GetInt(patterns.AreaLevel, parsingItem),
            };
        }

        private List<Socket> ParseSockets(ParsingItem parsingItem)
        {
            if (TryParseValue(patterns.Socket, parsingItem, out var match))
            {
                var groups = match.Groups.Values
                    .Where(x => !string.IsNullOrEmpty(x.Value))
                    .Skip(1)
                    .Select((x, Index) => new
                    {
                        x.Value,
                        Index,
                    })
                    .ToList();

                var result = new List<Socket>();

                foreach (var group in groups)
                {
                    var groupValue = group.Value.Replace("-", "").Trim();
                    while (groupValue.Length > 0)
                    {
                        switch (groupValue[0])
                        {
                            case 'B': result.Add(new Socket() { Group = group.Index, Colour = SocketColour.Blue }); break;
                            case 'G': result.Add(new Socket() { Group = group.Index, Colour = SocketColour.Green }); break;
                            case 'R': result.Add(new Socket() { Group = group.Index, Colour = SocketColour.Red }); break;
                            case 'W': result.Add(new Socket() { Group = group.Index, Colour = SocketColour.White }); break;
                            case 'A': result.Add(new Socket() { Group = group.Index, Colour = SocketColour.Abyss }); break;
                        }
                        groupValue = groupValue[1..];
                    }
                }

                return result;
            }

            return new List<Socket>();
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
            return TryParseValue(pattern, parsingItem, out var _);
        }

        private static int GetInt(Regex pattern, ParsingItem parsingItem)
        {
            if (TryParseValue(pattern, parsingItem, out var match) && int.TryParse(match.Groups[1].Value, out var result))
            {
                return result;
            }

            return default;
        }

        private static int GetInt(Regex pattern, ParsingBlock parsingBlock)
        {
            if (TryParseValue(pattern, parsingBlock, out var match) && int.TryParse(match.Groups[1].Value, out var result))
            {
                return result;
            }

            return default;
        }

        private static double GetDouble(Regex pattern, ParsingBlock parsingBlock)
        {
            if (TryParseValue(pattern, parsingBlock, out var match) && double.TryParse(match.Groups[1].Value.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return default;
        }

        private static double GetDps(Regex pattern, ParsingBlock parsingBlock, double attacksPerSecond)
        {
            if (TryParseValue(pattern, parsingBlock, out var match))
            {
                var matches = new Regex("(\\d+-\\d+)").Matches(match.Value);
                var dps = matches
                    .Select(x => x.Value.Split("-"))
                    .Sum(split =>
                    {
                        if (double.TryParse(split[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var minValue)
                         && double.TryParse(split[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var maxValue))
                        {
                            return (minValue + maxValue) / 2d;
                        }

                        return 0d;
                    });

                return Math.Round(dps * attacksPerSecond, 2);
            }

            return default;
        }

        private static bool TryParseValue(Regex pattern, ParsingItem parsingItem, out Match match)
        {
            foreach (var block in parsingItem.Blocks.Where(x => !x.Parsed))
            {
                if (TryParseValue(pattern, block, out match))
                {
                    return true;
                }
            }

            match = null!;
            return false;
        }

        private static bool TryParseValue(Regex pattern, ParsingBlock block, out Match match)
        {
            foreach (var line in block.Lines.Where(x => !x.Parsed))
            {
                match = pattern.Match(line.Text);
                if (match.Success)
                {
                    line.Parsed = true;
                    return true;
                }
            }

            match = null!;
            return false;
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
