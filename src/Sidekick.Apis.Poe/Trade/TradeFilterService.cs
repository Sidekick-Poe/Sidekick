using Sidekick.Apis.Poe.Localization;
using Sidekick.Apis.Poe.Trade.Models;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Trade
{
    public class TradeFilterService : ITradeFilterService
    {
        private readonly IGameLanguageProvider gameLanguageProvider;
        private readonly FilterResources resources;

        public TradeFilterService(
            IGameLanguageProvider gameLanguageProvider,
            FilterResources resources)
        {
            this.gameLanguageProvider = gameLanguageProvider;
            this.resources = resources;
        }

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

            // Armour
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Armour,
                gameLanguageProvider.Language?.DescriptionArmour,
                item.Properties.Armor);
            // Evasion
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Evasion,
                gameLanguageProvider.Language?.DescriptionEvasion,
                item.Properties.Evasion);
            // Energy shield
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_EnergyShield,
                gameLanguageProvider.Language?.DescriptionEnergyShield,
                item.Properties.EnergyShield);
            // Block
            InitializePropertyFilter(result.Armour,
                PropertyFilterType.Armour_Block,
                gameLanguageProvider.Language?.DescriptionChanceToBlock,
                item.Properties.ChanceToBlock);

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
                gameLanguageProvider.Language?.DescriptionAttacksPerSecond,
                item.Properties.AttacksPerSecond,
                delta: 0.1);
            // Critical strike chance
            InitializePropertyFilter(result.Weapon,
                PropertyFilterType.Weapon_CriticalStrikeChance,
                gameLanguageProvider.Language?.DescriptionCriticalStrikeChance,
                item.Properties.CriticalStrikeChance);

            // Item quantity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemQuantity,
                gameLanguageProvider.Language?.DescriptionItemQuantity,
                item.Properties.ItemQuantity);
            // Item rarity
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_ItemRarity,
                gameLanguageProvider.Language?.DescriptionItemRarity,
                item.Properties.ItemRarity);
            // Monster pack size
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_MonsterPackSize,
                gameLanguageProvider.Language?.DescriptionMonsterPackSize,
                item.Properties.MonsterPackSize);
            // Blighted
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Blighted,
                gameLanguageProvider.Language?.AffixBlighted,
                item.Properties.Blighted,
                enabled: item.Properties.Blighted);
            // Blight-ravaged
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_BlightRavaged,
                gameLanguageProvider.Language?.AffixBlightRavaged,
                item.Properties.BlightRavaged,
                enabled: item.Properties.BlightRavaged);
            // Map tier
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_Tier,
                gameLanguageProvider.Language?.DescriptionMapTier,
                item.Properties.MapTier,
                enabled: true,
                min: item.Properties.MapTier);
            // Area level
            InitializePropertyFilter(result.Map,
                PropertyFilterType.Map_AreaLevel,
                gameLanguageProvider.Language?.DescriptionAreaLevel,
                item.Properties.AreaLevel,
                enabled: true,
                min: item.Properties.AreaLevel);

            // Quality
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Quality,
                gameLanguageProvider.Language?.DescriptionQuality,
                item.Properties.Quality,
                enabled: item.Metadata.Rarity == Rarity.Gem,
                min: item.Metadata.Rarity == Rarity.Gem && item.Properties.Quality >= 20 ? item.Properties.Quality : null);
            // Gem level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_GemLevel,
                gameLanguageProvider.Language?.DescriptionLevel,
                item.Properties.GemLevel,
                enabled: true,
                min: item.Properties.GemLevel);
            // Item level
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_ItemLevel,
                gameLanguageProvider.Language?.DescriptionItemLevel,
                item.Properties.ItemLevel,
                enabled: item.Properties.ItemLevel >= 80 && item.Properties.MapTier == 0 && item.Metadata.Rarity != Rarity.Unique,
                min: item.Properties.ItemLevel >= 80 ? item.Properties.ItemLevel : null);
            // Corrupted
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Corrupted,
                gameLanguageProvider.Language?.DescriptionCorrupted,
                true,
                enabled: item.Properties.Corrupted ? true : null);
            // Crusader
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Crusader,
                gameLanguageProvider.Language?.InfluenceCrusader,
                item.Influences.Crusader,
                enabled: item.Influences.Crusader);
            // Elder
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Elder,
                gameLanguageProvider.Language?.InfluenceElder,
                item.Influences.Elder,
                enabled: item.Influences.Elder);
            // Hunter
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Hunter,
                gameLanguageProvider.Language?.InfluenceHunter,
                item.Influences.Hunter,
                enabled: item.Influences.Hunter);
            // Redeemer
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Redeemer,
                gameLanguageProvider.Language?.InfluenceRedeemer,
                item.Influences.Redeemer,
                enabled: item.Influences.Redeemer);
            // Shaper
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Shaper,
                gameLanguageProvider.Language?.InfluenceShaper,
                item.Influences.Shaper,
                enabled: item.Influences.Shaper);
            // Warlord
            InitializePropertyFilter(result.Misc,
                PropertyFilterType.Misc_Influence_Warlord,
                gameLanguageProvider.Language?.InfluenceWarlord,
                item.Influences.Warlord,
                enabled: item.Influences.Warlord);

            return result;
        }

        private void InitializePropertyFilter(List<PropertyFilter> filters,
            PropertyFilterType type,
            string? label,
            object value,
            double? delta = null,
            bool? enabled = false,
            double? min = null)
        {
            if (label == null)
            {
                return;
            }

            if (double.TryParse(value.ToString(), out var doubleValue) && doubleValue != 0)
            {
                filters.Add(new PropertyFilter(
                    enabled: enabled,
                    type: type,
                    value: value,
                    text: label,
                    min: min,
                    delta: delta ?? 1));
            }
            else if (value is bool boolValue && boolValue)
            {
                filters.Add(new PropertyFilter(
                    enabled: enabled,
                    type: type,
                    value: value,
                    text: label,
                    min: min,
                    delta: delta ?? 1));
            }
        }
    }
}
