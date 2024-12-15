using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade
{
    public class TradeFilterService
    (
        IGameLanguageProvider gameLanguageProvider,
        FilterResources resources
    ) : ITradeFilterService
    {
        public IEnumerable<ModifierFilter> GetModifierFilters(Item item)
        {
            // No filters for divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Gem
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Unknown)
            {
                yield break;
            }

            foreach (var modifierLine in item.ModifierLines)
            {
                yield return new ModifierFilter(modifierLine);
            }
        }

        public IEnumerable<PseudoModifierFilter> GetPseudoModifierFilters(Item item)
        {
            // No filters for divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Gem
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Unknown
                || item.Metadata.Category == Category.Currency)
            {
                yield break;
            }

            foreach (var modifier in item.PseudoModifiers)
            {
                yield return new PseudoModifierFilter(modifier);
            }
        }

        public PropertyFilters GetPropertyFilters(Item item)
        {
            // No filters for currencies and divination cards, etc.
            if (item.Metadata.Category == Category.DivinationCard
                || item.Metadata.Category == Category.Currency
                || item.Metadata.Category == Category.ItemisedMonster
                || item.Metadata.Category == Category.Leaguestone
                || item.Metadata.Category == Category.Unknown)
            {
                return new();
            }

            var result = new PropertyFilters();

            // Armour properties
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Armour, gameLanguageProvider.Language?.DescriptionArmour, item.Properties.Armor);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Evasion, gameLanguageProvider.Language?.DescriptionEvasion, item.Properties.Evasion);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_EnergyShield, gameLanguageProvider.Language?.DescriptionEnergyShield, item.Properties.EnergyShield);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Block, gameLanguageProvider.Language?.DescriptionChanceToBlock, item.Properties.ChanceToBlock);

            // Weapon properties
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_Dps, resources.Filters_Dps, item.Properties.TotalDps, enabled: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_PhysicalDps, resources.Filters_PDps, item.Properties.PhysicalDps, enabled: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ElementalDps, resources.Filters_EDps, item.Properties.ElementalDps, enabled: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ChaosDps, "Chaos DPS", item.Properties.ChaosDps, enabled: false);

            // Handle base damage values
            if (item.Properties.PhysicalDamage.HasValue())
            {
                InitializeDamageRangeFilter(result.Weapon, 
                    PropertyFilterType.Weapon_Damage, 
                    gameLanguageProvider.Language?.DescriptionDamage, 
                    ConvertFromCommonDamageRange(item.Properties.PhysicalDamage), 
                    enabled: false);
            }
            else if (item.Properties.ElementalDamages.Any(x => x.HasValue()) || item.Properties.ChaosDamage.HasValue())
            {
                // For non-physical weapons, use total damage for the damage filter
                var totalMin = item.Properties.ElementalDamages.Sum(x => x.Min) + (item.Properties.ChaosDamage?.Min ?? 0);
                var totalMax = item.Properties.ElementalDamages.Sum(x => x.Max) + (item.Properties.ChaosDamage?.Max ?? 0);
                
                InitializePropertyFilter(result.Weapon,
                    PropertyFilterType.Weapon_Damage,
                    gameLanguageProvider.Language?.DescriptionDamage,
                    new DamageRange { Min = totalMin, Max = totalMax },
                    enabled: false);
            }

            if (item.Properties.ElementalDamages.Any(x => x.HasValue()))
            {
                var totalMin = item.Properties.ElementalDamages.Sum(x => x.Min);
                var totalMax = item.Properties.ElementalDamages.Sum(x => x.Max);

                // Create a display value with individual ranges
                var displayRanges = item.Properties.ElementalDamages.ToList();

                // Try to match damage types from modifiers
                var damageTypes = new List<DamageType>();
                foreach (var modifier in item.ModifierLines)
                {
                    if (modifier.Text.Contains("Fire Damage"))
                        damageTypes.Add(DamageType.Fire);
                    else if (modifier.Text.Contains("Cold Damage"))
                        damageTypes.Add(DamageType.Cold);
                    else if (modifier.Text.Contains("Lightning Damage"))
                        damageTypes.Add(DamageType.Lightning);
                }

                // Assign damage types if we found matches
                for (int i = 0; i < displayRanges.Count && i < damageTypes.Count; i++)
                {
                    displayRanges[i].Type = damageTypes[i];
                }

                // Create a custom property filter that uses different values for display vs filtering
                InitializePropertyFilter(result.Weapon,
                    PropertyFilterType.Weapon_ElementalDamage,
                    gameLanguageProvider.Language?.DescriptionElementalDamage,
                    displayRanges,
                    enabled: false,
                    min: totalMin.ToDecimal(),
                    max: totalMax.ToDecimal());
            }

            if (item.Properties.ChaosDamage.HasValue())
            {
                InitializeDamageRangeFilter(result.Weapon,
                    PropertyFilterType.Weapon_ChaosDamage,
                    "Chaos Damage",
                    ConvertFromCommonDamageRange(item.Properties.ChaosDamage),
                    enabled: false);
            }

            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_AttacksPerSecond, gameLanguageProvider.Language?.DescriptionAttacksPerSecond, item.Properties.AttacksPerSecond, delta: 0.1, enabled: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_CriticalStrikeChance, gameLanguageProvider.Language?.DescriptionCriticalStrikeChance, item.Properties.CriticalStrikeChance, enabled: false);

            // Map properties
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_ItemQuantity, gameLanguageProvider.Language?.DescriptionItemQuantity, item.Properties.ItemQuantity);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_ItemRarity, gameLanguageProvider.Language?.DescriptionItemRarity, item.Properties.ItemRarity);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_MonsterPackSize, gameLanguageProvider.Language?.DescriptionMonsterPackSize, item.Properties.MonsterPackSize);
            InitializeBooleanFilter(result.Map, PropertyFilterType.Map_Blighted, gameLanguageProvider.Language?.AffixBlighted, item.Properties.Blighted);
            InitializeBooleanFilter(result.Map, PropertyFilterType.Map_BlightRavaged, gameLanguageProvider.Language?.AffixBlightRavaged, item.Properties.BlightRavaged);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_Tier, gameLanguageProvider.Language?.DescriptionMapTier, item.Properties.MapTier, enabled: true);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_AreaLevel, gameLanguageProvider.Language?.DescriptionAreaLevel, item.Properties.AreaLevel, enabled: true);

            // Misc properties
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_Quality, gameLanguageProvider.Language?.DescriptionQuality, item.Properties.Quality, 
                enabled: item.Metadata.Rarity == Rarity.Gem);
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_GemLevel, gameLanguageProvider.Language?.DescriptionLevel, item.Properties.GemLevel, enabled: true);
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_ItemLevel, gameLanguageProvider.Language?.DescriptionItemLevel, item.Properties.ItemLevel,
                enabled: item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Metadata.Rarity != Rarity.Unique);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Corrupted, gameLanguageProvider.Language?.DescriptionCorrupted, item.Properties.Corrupted);

            // Influence properties
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Crusader, gameLanguageProvider.Language?.InfluenceCrusader, item.Influences.Crusader);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Elder, gameLanguageProvider.Language?.InfluenceElder, item.Influences.Elder);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Hunter, gameLanguageProvider.Language?.InfluenceHunter, item.Influences.Hunter);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Redeemer, gameLanguageProvider.Language?.InfluenceRedeemer, item.Influences.Redeemer);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Shaper, gameLanguageProvider.Language?.InfluenceShaper, item.Influences.Shaper);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Warlord, gameLanguageProvider.Language?.InfluenceWarlord, item.Influences.Warlord);

            return result;
        }

        private void InitializePropertyFilter(List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            object value,
            double? delta = null,
            bool? enabled = null,
            decimal? min = null,
            decimal? max = null)
        {
            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            filters.Add(new PropertyFilter(
                enabled: enabled,
                type: type,
                text: label,
                value: value,
                min: min,
                max: max,
                delta: delta ?? 1));
        }

        private void InitializeDamageRangeFilter(List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            DamageRange range,
            bool enabled = true)
        {
            if (range.Min > 0 || range.Max > 0)
            {
                InitializePropertyFilter(filters,
                    type,
                    label,
                    range,
                    enabled: enabled,
                    min: range.Min.ToDecimal(),
                    max: range.Max.ToDecimal());
            }
        }

        private void InitializeNumericFilter(List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            double? value,
            bool enabled = false,
            double? delta = null)
        {
            if (value > 0)
            {
                InitializePropertyFilter(filters,
                    type,
                    label,
                    value.Value,
                    enabled: enabled,
                    delta: delta);
            }
        }

        private void InitializeBooleanFilter(List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            bool value)
        {
            if (value)
            {
                InitializePropertyFilter(filters,
                    type,
                    label,
                    value,
                    enabled: value);
            }
        }

        private DamageRange ConvertFromCommonDamageRange(Sidekick.Common.Game.Items.DamageRange range)
        {
            if (range == null) return new DamageRange();
            return new DamageRange 
            { 
                Min = range.Min, 
                Max = range.Max 
            };
        }
    }
}
