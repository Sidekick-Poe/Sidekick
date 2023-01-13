using System;
using System.Collections.Generic;
using System.Linq;
using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Items.Modifiers;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade
{
    public class TradeFilterService : ITradeFilterService
    {
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly FilterResources resources;
        private readonly ISettings sidekickSettings;

        public TradeFilterService(
            IGameLanguageProvider gameLanguageProvider,
            FilterResources resources,
            ISettings sidekickSettings)
        {
            this.gameLanguageProvider = gameLanguageProvider;
            this.resources = resources;
            this.sidekickSettings = sidekickSettings;
        }

        public List<ModifierFilter> GetModifierFilters(Item item)
        {
            var result = new List<ModifierFilter>();

            // No filters for currencies and divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Currency
                || item.Metadata.Category == Category.Gem
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Undefined)
            {
                return result;
            }

            List<string> enabledModifiers = new();

            InitializeModifierFilters(result, item.ModifierLines, enabledModifiers);
            InitializePseudoFilters(result, item.PseudoModifiers, enabledModifiers);

            return result;
        }

        private void InitializeModifierFilters(List<ModifierFilter> filters, List<ModifierLine> modifierLines, List<string> enabledModifiers)
        {
            if (modifierLines.Count == 0) return;

            foreach (var modifierLine in modifierLines)
            {
                double? min = null, max = null;

                if (modifierLine.Modifier != null && !modifierLine.IsFuzzy)
                {
                    if (modifierLine.Modifier.OptionValue != null)
                    {
                        (min, max) = NormalizeValues(modifierLine.Modifier.OptionValue, modifierLine.Modifier.Category);
                    }
                    else
                    {
                        (min, max) = NormalizeValues(modifierLine.Modifier.Values, modifierLine.Modifier.Category);
                    }
                }

                filters.Add(new ModifierFilter()
                {
                    Enabled = false,
                    Line = modifierLine,
                    Min = min,
                    Max = max,
                });
            }
        }

        private void InitializePseudoFilters(List<ModifierFilter> filters, List<Modifier> modifiers, List<string> enabledModifiers)
        {
            if (modifiers.Count == 0) return;

            foreach (var modifier in modifiers)
            {
                double? min, max;

                if (modifier.OptionValue != null)
                {
                    (min, max) = NormalizeValues(modifier.OptionValue, modifier.Category);
                }
                else
                {
                    (min, max) = NormalizeValues(modifier.Values, modifier.Category);
                }

                filters.Add(new ModifierFilter()
                {
                    Enabled = false,
                    Line = new ModifierLine()
                    {
                        Text = modifier.Text,
                        Modifier = modifier,
                    },
                    Min = min,
                    Max = max,
                });
            }
        }

        private (double? Min, double? Max) NormalizeValues<T>(
            T value,
            ModifierCategory category,
            double delta = 5)
        {
            double? min = null;
            double? max = null;

            if (value is List<double> groupValue)
            {
                var itemValue = groupValue.OrderBy(x => x).FirstOrDefault();

                if (itemValue >= 0)
                {
                    min = itemValue;
                    if (sidekickSettings.Trade_Normalize_Values && category != ModifierCategory.Enchant)
                    {
                        min = NormalizeMinValue(min, delta);
                    }
                }
                else
                {
                    max = itemValue;
                    if (sidekickSettings.Trade_Normalize_Values && category != ModifierCategory.Enchant)
                    {
                        max = NormalizeMaxValue(max, delta);
                    }
                }

                if (!groupValue.Any())
                {
                    min = null;
                    max = null;
                }
            }

            return (min, max);
        }

        public PropertyFilters GetPropertyFilters(Item item)
        {
            var result = new PropertyFilters();

            // No filters for currencies and divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Currency
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Undefined)
            {
                return result;
            }

            // Armour
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Armour,
                gameLanguageProvider.Language.DescriptionArmour,
                item.Properties.Armor);
            // Evasion
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Evasion,
                gameLanguageProvider.Language.DescriptionEvasion,
                item.Properties.Evasion);
            // Energy shield
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_EnergyShield,
                gameLanguageProvider.Language.DescriptionEnergyShield,
                item.Properties.EnergyShield);
            // Block
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Block,
                gameLanguageProvider.Language.DescriptionChanceToBlock,
                item.Properties.ChanceToBlock,
                delta: 1);

            // Physical Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_PhysicalDps,
                resources.Filters_PDps,
                item.Properties.PhysicalDps);
            // Elemental Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_ElementalDps,
                resources.Filters_EDps,
                item.Properties.ElementalDps);
            // Total Dps
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_Dps,
                resources.Filters_Dps,
                item.Properties.DamagePerSecond);
            // Attacks per second
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_AttacksPerSecond,
                gameLanguageProvider.Language.DescriptionAttacksPerSecond,
                item.Properties.AttacksPerSecond,
                delta: 0.1);
            // Critical strike chance
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_CriticalStrikeChance,
                gameLanguageProvider.Language.DescriptionCriticalStrikeChance,
                item.Properties.CriticalStrikeChance,
                delta: 1);

            // Item quantity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemQuantity,
                gameLanguageProvider.Language.DescriptionItemQuantity,
                item.Properties.ItemQuantity);
            // Item rarity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemRarity,
                gameLanguageProvider.Language.DescriptionItemRarity,
                item.Properties.ItemRarity);
            // Monster pack size
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_MonsterPackSize,
                gameLanguageProvider.Language.DescriptionMonsterPackSize,
                item.Properties.MonsterPackSize);
            // Blighted
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Blighted,
                gameLanguageProvider.Language.PrefixBlighted,
                item.Properties.Blighted,
                enabled: item.Properties.Blighted);
            // Blight-ravaged
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_BlightRavaged,
                gameLanguageProvider.Language.PrefixBlightRavaged,
                item.Properties.BlightRavaged,
                enabled: item.Properties.BlightRavaged);
            // Map tier
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Tier,
                gameLanguageProvider.Language.DescriptionMapTier,
                item.Properties.MapTier,
                enabled: true,
                min: item.Properties.MapTier);

            // Quality
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Quality,
                gameLanguageProvider.Language.DescriptionQuality,
                item.Properties.Quality,
                enabled: item.Metadata.Rarity == Rarity.Gem,
                min: item.Metadata.Rarity == Rarity.Gem && item.Properties.Quality >= 20 ? item.Properties.Quality : null);
            // Gem level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_GemLevel,
                gameLanguageProvider.Language.DescriptionLevel,
                item.Properties.GemLevel,
                enabled: true,
                min: item.Properties.GemLevel);
            // Item level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_ItemLevel,
                gameLanguageProvider.Language.DescriptionItemLevel,
                item.Properties.ItemLevel,
                enabled: item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Metadata.Rarity != Rarity.Unique,
                min: item.Properties.ItemLevel >= 80 ? (double?)item.Properties.ItemLevel : null);
            // Corrupted
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Corrupted,
                gameLanguageProvider.Language.DescriptionCorrupted,
                item.Properties.Corrupted,
                enabled: item.Properties.Corrupted);
            // Crusader
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Crusader,
                gameLanguageProvider.Language.InfluenceCrusader,
                item.Influences.Crusader,
                enabled: item.Influences.Crusader);
            // Elder
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Elder,
                gameLanguageProvider.Language.InfluenceElder,
                item.Influences.Elder,
                enabled: item.Influences.Elder);
            // Hunter
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Hunter,
                gameLanguageProvider.Language.InfluenceHunter,
                item.Influences.Hunter,
                enabled: item.Influences.Hunter);
            // Redeemer
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Redeemer,
                gameLanguageProvider.Language.InfluenceRedeemer,
                item.Influences.Redeemer,
                enabled: item.Influences.Redeemer);
            // Shaper
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Shaper,
                gameLanguageProvider.Language.InfluenceShaper,
                item.Influences.Shaper,
                enabled: item.Influences.Shaper);
            // Warlord
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Warlord,
                gameLanguageProvider.Language.InfluenceWarlord, item.Influences.Warlord,
                enabled: item.Influences.Warlord);
            // Scourged
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Scourged,
                gameLanguageProvider.Language.DescriptionScourged,
                item.Properties.Scourged ? 1 : 0,
                enabled: item.Properties.Scourged,
                min: 1);

            return result;
        }

        private void InitializePropertyFilter<T>(List<PropertyFilter> filters,
            PropertyFilterType type,
            string label,
            T value,
            double delta = 5,
            bool enabled = false,
            double? min = null,
            double? max = null)
        {
            FilterValueType valueType;

            switch (value)
            {
                case bool boolValue:
                    valueType = FilterValueType.Boolean;
                    if (!boolValue) return;
                    break;

                case int intValue:
                    valueType = FilterValueType.Int;
                    if (intValue == 0) return;
                    min ??= sidekickSettings.Trade_Normalize_Values ? NormalizeMinValue(intValue, delta) : intValue;
                    break;

                case double doubleValue:
                    valueType = FilterValueType.Double;
                    if (doubleValue == 0) return;
                    min ??= sidekickSettings.Trade_Normalize_Values ? NormalizeMinValue(doubleValue, delta) : doubleValue;
                    break;

                default: return;
            }

            filters.Add(new PropertyFilter()
            {
                Enabled = enabled,
                Type = type,
                Value = value,
                ValueType = valueType,
                Text = label,
                Min = min,
                Max = max,
            });
        }

        /// <summary>
        /// Smallest positive value between a -5 delta or 90%.
        /// </summary>
        private static int? NormalizeMinValue(double? value, double delta)
        {
            if (value.HasValue)
            {
                if (value.Value > 0)
                {
                    return (int)Math.Max(Math.Min(value.Value - delta, value.Value * 0.9), 0);
                }
                else
                {
                    return (int)Math.Min(Math.Min(value.Value - delta, value.Value * 1.1), 0);
                }
            }

            return null;
        }

        /// <summary>
        /// Smallest positive value between a +5 delta or 110%.
        /// </summary>
        private static int? NormalizeMaxValue(double? value, double delta)
        {
            if (value.HasValue)
            {
                if (value.Value > 0)
                {
                    return (int)Math.Max(Math.Max(value.Value + delta, value.Value * 1.1), 0);
                }
                else
                {
                    return (int)Math.Min(Math.Max(value.Value + delta, value.Value * 0.9), 0);
                }
            }

            return null;
        }
    }
}
