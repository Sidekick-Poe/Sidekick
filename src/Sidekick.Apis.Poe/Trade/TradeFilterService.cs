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
            if (item.Metadata.Category == Category.DivinationCard || item.Metadata.Category == Category.Gem || item.Metadata.Category == Category.ItemisedMonster || item.Metadata.Category == Category.Leaguestone || item.Metadata.Category == Category.Unknown)
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
            if (item.Metadata.Category == Category.DivinationCard || item.Metadata.Category == Category.Gem || item.Metadata.Category == Category.ItemisedMonster || item.Metadata.Category == Category.Leaguestone || item.Metadata.Category == Category.Unknown || item.Metadata.Category == Category.Currency)
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
            if (item.Metadata.Category == Category.DivinationCard || item.Metadata.Category == Category.Currency || item.Metadata.Category == Category.ItemisedMonster || item.Metadata.Category == Category.Leaguestone || item.Metadata.Category == Category.Unknown)
            {
                return new();
            }

            var result = new PropertyFilters();

            // Armour properties
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Armour, gameLanguageProvider.Language.DescriptionArmour, item.Properties.Armor);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Evasion, gameLanguageProvider.Language.DescriptionEvasion, item.Properties.Evasion);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_EnergyShield, gameLanguageProvider.Language.DescriptionEnergyShield, item.Properties.EnergyShield);
            InitializeNumericFilter(result.Armour, PropertyFilterType.Armour_Block, gameLanguageProvider.Language.DescriptionChanceToBlock, item.Properties.ChanceToBlock);

            // Weapon properties
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_Damage, resources.Filters_Damage, item.Properties.TotalDamage, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_PhysicalDps, resources.Filters_PDps, item.Properties.PhysicalDps, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ElementalDps, resources.Filters_EDps, item.Properties.ElementalDps, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_ChaosDps, resources.Filters_ChaosDps, item.Properties.ChaosDps, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_Dps, resources.Filters_Dps, item.Properties.TotalDps, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_AttacksPerSecond, gameLanguageProvider.Language.DescriptionAttacksPerSecond, item.Properties.AttacksPerSecond, @checked: false);
            InitializeNumericFilter(result.Weapon, PropertyFilterType.Weapon_CriticalStrikeChance, gameLanguageProvider.Language.DescriptionCriticalStrikeChance, item.Properties.CriticalStrikeChance, @checked: false);

            // Map properties
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_ItemQuantity, gameLanguageProvider.Language.DescriptionItemQuantity, item.Properties.ItemQuantity);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_ItemRarity, gameLanguageProvider.Language.DescriptionItemRarity, item.Properties.ItemRarity);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_MonsterPackSize, gameLanguageProvider.Language.DescriptionMonsterPackSize, item.Properties.MonsterPackSize);
            InitializeBooleanFilter(result.Map, PropertyFilterType.Map_Blighted, gameLanguageProvider.Language.AffixBlighted, item.Properties.Blighted);
            InitializeBooleanFilter(result.Map, PropertyFilterType.Map_BlightRavaged, gameLanguageProvider.Language.AffixBlightRavaged, item.Properties.BlightRavaged);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_Tier, gameLanguageProvider.Language.DescriptionMapTier, item.Properties.MapTier, @checked: true);
            InitializeNumericFilter(result.Map, PropertyFilterType.Map_AreaLevel, gameLanguageProvider.Language.DescriptionAreaLevel, item.Properties.AreaLevel, @checked: true);

            // Misc properties
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_Quality, gameLanguageProvider.Language.DescriptionQuality, item.Properties.Quality, @checked: item.Metadata.Rarity == Rarity.Gem);
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_GemLevel, gameLanguageProvider.Language.DescriptionLevel, item.Properties.GemLevel, @checked: true);
            InitializeNumericFilter(result.Misc, PropertyFilterType.Misc_ItemLevel, gameLanguageProvider.Language.DescriptionItemLevel, item.Properties.ItemLevel, @checked: item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Metadata.Rarity != Rarity.Unique);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Corrupted, gameLanguageProvider.Language.DescriptionCorrupted, item.Properties.Corrupted, true);

            // Influence properties
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Crusader, gameLanguageProvider.Language.InfluenceCrusader, item.Influences.Crusader);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Elder, gameLanguageProvider.Language.InfluenceElder, item.Influences.Elder);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Hunter, gameLanguageProvider.Language.InfluenceHunter, item.Influences.Hunter);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Redeemer, gameLanguageProvider.Language.InfluenceRedeemer, item.Influences.Redeemer);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Shaper, gameLanguageProvider.Language.InfluenceShaper, item.Influences.Shaper);
            InitializeBooleanFilter(result.Misc, PropertyFilterType.Misc_Influence_Warlord, gameLanguageProvider.Language.InfluenceWarlord, item.Influences.Warlord);

            return result;
        }

        private void InitializePropertyFilter(
            List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            object value,
            bool? @checked = null,
            decimal? min = null,
            decimal? max = null)
        {
            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            filters.Add(new PropertyFilter(@checked, type, label, value, min, max));
        }

        private void InitializeNumericFilter(List<PropertyFilter> filters, PropertyFilterType type, string? label, double? value, bool @checked = false)
        {
            if (value > 0)
            {
                InitializePropertyFilter(filters, type, label, value.Value, @checked: @checked);
            }
        }

        private void InitializeBooleanFilter(List<PropertyFilter> filters, PropertyFilterType type, string? label, bool @checked, bool force = false)
        {
            if (force || @checked)
            {
                InitializePropertyFilter(filters, type, label, @checked, @checked: @checked);
            }
        }
    }
}
