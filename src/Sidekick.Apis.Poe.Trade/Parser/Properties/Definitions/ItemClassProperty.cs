using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions.Models;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemClassProperty : PropertyDefinition
{
    private readonly GameType game;
    private readonly IGameLanguageProvider gameLanguageProvider;
    private readonly IFilterProvider filterProvider;
    private readonly ISettingsService settingsService;
    private readonly IFuzzyService fuzzyService;

    public ItemClassProperty(
        GameType game,
        IServiceProvider serviceProvider)
    {
        this.game = game;
        gameLanguageProvider = serviceProvider.GetRequiredService<IGameLanguageProvider>();
        filterProvider = serviceProvider.GetRequiredService<IFilterProvider>();
        settingsService = serviceProvider.GetRequiredService<ISettingsService>();
        fuzzyService = serviceProvider.GetRequiredService<IFuzzyService>();

        ItemClassDefinitions = new(GetItemClassDefinitions);
        ApiItemClassDefinitions = new(GetApiItemClassDefinitions);
    }

    public override List<Category> ValidItemClasses { get; } = [];

    private Lazy<List<ItemClassDefinition>> ItemClassDefinitions { get; }

    private Lazy<List<ItemClassDefinition>> ApiItemClassDefinitions { get; }

    private List<ItemClassDefinition> GetApiItemClassDefinitions()
    {
        if (filterProvider.TypeCategory == null) return [];

        return filterProvider.TypeCategory.Option.Options.ConvertAll(x => new ItemClassDefinition()
        {
            Id = x.Id,
            Text = x.Text,
            FuzzyText = x.Text is null ? null : fuzzyService.CleanFuzzyText(x.Text),
        });
    }

    private List<ItemClassDefinition> GetItemClassDefinitions()
    {
        List<ItemClassDefinition> definitions =
        [
            CreateItemClassDefinition(ItemClass.Amulet, gameLanguageProvider.Language.Classes.Amulet),
            CreateItemClassDefinition(ItemClass.Belt, gameLanguageProvider.Language.Classes.Belt),
            CreateItemClassDefinition(ItemClass.Ring, gameLanguageProvider.Language.Classes.Ring),
            CreateItemClassDefinition(ItemClass.Trinket, gameLanguageProvider.Language.Classes.Trinkets),

            CreateItemClassDefinition(ItemClass.BodyArmour, gameLanguageProvider.Language.Classes.BodyArmours),
            CreateItemClassDefinition(ItemClass.Boots, gameLanguageProvider.Language.Classes.Boots),
            CreateItemClassDefinition(ItemClass.Gloves, gameLanguageProvider.Language.Classes.Gloves),
            CreateItemClassDefinition(ItemClass.Helmet, gameLanguageProvider.Language.Classes.Helmets),
            CreateItemClassDefinition(ItemClass.Quiver, gameLanguageProvider.Language.Classes.Quivers),
            CreateItemClassDefinition(ItemClass.Shield, gameLanguageProvider.Language.Classes.Shields),
            CreateItemClassDefinition(ItemClass.Focus, gameLanguageProvider.Language.Classes.Focus),
            CreateItemClassDefinition(ItemClass.Buckler, gameLanguageProvider.Language.Classes.Bucklers),

            CreateItemClassDefinition(ItemClass.DivinationCard, gameLanguageProvider.Language.Classes.DivinationCard),

            CreateItemClassDefinition(ItemClass.Resonator, gameLanguageProvider.Language.Classes.DelveStackableSocketableCurrency),
            CreateItemClassDefinition(ItemClass.HeistObjective, gameLanguageProvider.Language.Classes.HeistTarget),
            CreateItemClassDefinition(ItemClass.Omen, gameLanguageProvider.Language.Classes.Omen),
            CreateItemClassDefinition(ItemClass.Socketable, gameLanguageProvider.Language.Classes.Socketable),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.UniqueFragment),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Fossil),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Incubator),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Tattoo),

            CreateItemClassDefinition(ItemClass.ActiveGem, gameLanguageProvider.Language.Classes.ActiveSkillGems),
            CreateItemClassDefinition(ItemClass.SupportGem, gameLanguageProvider.Language.Classes.SupportSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSkillGem, gameLanguageProvider.Language.Classes.UncutSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSupportGem, gameLanguageProvider.Language.Classes.UncutSupportGems),
            CreateItemClassDefinition(ItemClass.UncutSpiritGem, gameLanguageProvider.Language.Classes.UncutSpiritGems),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.AwakenedSupportSkillGems),

            CreateItemClassDefinition(ItemClass.Blueprint, gameLanguageProvider.Language.Classes.Blueprint),
            CreateItemClassDefinition(ItemClass.Contract, gameLanguageProvider.Language.Classes.Contract),
            CreateItemClassDefinition(ItemClass.HeistReward, gameLanguageProvider.Language.Classes.HeistBrooch),
            CreateItemClassDefinition(ItemClass.HeistUtility, gameLanguageProvider.Language.Classes.HeistCloak),
            CreateItemClassDefinition(ItemClass.HeistWeapon, gameLanguageProvider.Language.Classes.HeistGear),
            CreateItemClassDefinition(ItemClass.HeistTool, gameLanguageProvider.Language.Classes.HeistTool),

            CreateItemClassDefinition(ItemClass.Jewel, gameLanguageProvider.Language.Classes.Jewel),
            CreateItemClassDefinition(ItemClass.AbyssJewel, gameLanguageProvider.Language.Classes.AbyssJewel),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.ClusterJewel),

            CreateItemClassDefinition(ItemClass.Logbook, gameLanguageProvider.Language.Classes.Logbooks),

            CreateItemClassDefinition(ItemClass.Waystone, gameLanguageProvider.Language.Classes.Waystone),
            CreateItemClassDefinition(ItemClass.Breachstone, gameLanguageProvider.Language.Classes.Breachstone),
            CreateItemClassDefinition(ItemClass.Barya, gameLanguageProvider.Language.Classes.Barya),
            CreateItemClassDefinition(ItemClass.BossKey, gameLanguageProvider.Language.Classes.BossKey),
            CreateItemClassDefinition(ItemClass.Ultimatum, gameLanguageProvider.Language.Classes.Ultimatum),
            CreateItemClassDefinition(ItemClass.Tablet, gameLanguageProvider.Language.Classes.Tablet),
            CreateItemClassDefinition(ItemClass.MapFragment, gameLanguageProvider.Language.Classes.MapFragments),

            CreateItemClassDefinition(ItemClass.MemoryLine, gameLanguageProvider.Language.Classes.MemoryLine),

            CreateItemClassDefinition(ItemClass.Bow, gameLanguageProvider.Language.Classes.Bows),
            CreateItemClassDefinition(ItemClass.Crossbow, gameLanguageProvider.Language.Classes.Crossbows),
            CreateItemClassDefinition(ItemClass.Claw, gameLanguageProvider.Language.Classes.Claws),
            CreateItemClassDefinition(ItemClass.Dagger, gameLanguageProvider.Language.Classes.Daggers),
            CreateItemClassDefinition(ItemClass.RuneDagger, gameLanguageProvider.Language.Classes.RuneDaggers),
            CreateItemClassDefinition(ItemClass.OneHandAxe, gameLanguageProvider.Language.Classes.OneHandAxes),
            CreateItemClassDefinition(ItemClass.OneHandMace, gameLanguageProvider.Language.Classes.OneHandMaces),
            CreateItemClassDefinition(ItemClass.OneHandSword, gameLanguageProvider.Language.Classes.OneHandSwords),
            CreateItemClassDefinition(ItemClass.Sceptre, gameLanguageProvider.Language.Classes.Sceptres),
            CreateItemClassDefinition(ItemClass.Staff, gameLanguageProvider.Language.Classes.Staves),
            CreateItemClassDefinition(ItemClass.Spear, gameLanguageProvider.Language.Classes.Spears),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Flails),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Rapiers),
            CreateItemClassDefinition(ItemClass.FishingRod, gameLanguageProvider.Language.Classes.FishingRods),
            CreateItemClassDefinition(ItemClass.TwoHandAxe, gameLanguageProvider.Language.Classes.TwoHandAxes),
            CreateItemClassDefinition(ItemClass.TwoHandMace, gameLanguageProvider.Language.Classes.TwoHandMaces),
            CreateItemClassDefinition(ItemClass.TwoHandSword, gameLanguageProvider.Language.Classes.TwoHandSwords),
            CreateItemClassDefinition(ItemClass.Wand, gameLanguageProvider.Language.Classes.Wands),
            CreateItemClassDefinition(ItemClass.Warstaff, gameLanguageProvider.Language.Classes.Warstaves, gameLanguageProvider.Language.Classes.Quarterstaves),

            CreateItemClassDefinition(ItemClass.Tincture, gameLanguageProvider.Language.Classes.Tinctures),
            CreateItemClassDefinition(ItemClass.Corpse, gameLanguageProvider.Language.Classes.Corpses),

            CreateItemClassDefinition(ItemClass.SanctumRelic, gameLanguageProvider.Language.Classes.SanctumRelics),
            CreateItemClassDefinition(ItemClass.SanctumResearch, gameLanguageProvider.Language.Classes.SanctumResearch),
        ];

        if (game == GameType.PathOfExile2)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.LifeFlask, gameLanguageProvider.Language.Classes.LifeFlasks),
                CreateItemClassDefinition(ItemClass.ManaFlask, gameLanguageProvider.Language.Classes.ManaFlasks),
            ]);
        }

        if (game == GameType.PathOfExile)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.Flask, gameLanguageProvider.Language.Classes.HybridFlasks),
            ]);
        }

        return definitions;
    }

    private ItemClassDefinition CreateItemClassDefinition(ItemClass itemClass, params string[] labels)
    {
        var text = labels.FirstOrDefault();

        return new ItemClassDefinition()
        {
            ItemClass = itemClass,
            Id = itemClass.GetValueAttribute(),
            Text = text,
            Pattern = BuildRegex(labels),
            FuzzyText = text is null ? null : fuzzyService.CleanFuzzyText(text),
        };
    }

    private Regex BuildRegex(params string[] labels) => new($"^{Regex.Escape(gameLanguageProvider.Language.Classes.Prefix)}:* *(?:{string.Join("|", labels.Select(Regex.Escape))})$");

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass != ItemClass.Unknown) return;

        var line = item.Text.Blocks[0].Lines[0].Text;
        item.Text.Blocks[0].Lines[0].Parsed = true;

        foreach (var definition in ItemClassDefinitions.Value)
        {
            if (definition.Pattern?.IsMatch(line) ?? false)
            {
                item.Properties.ItemClass = definition.ItemClass;
                return;
            }
        }

        var classLine = line.Replace(gameLanguageProvider.Language.Classes.Prefix, "").Trim(' ', ':');

        // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
        if (classLine == gameLanguageProvider.Language.Classes.MiscMapItems)
        {
            classLine = gameLanguageProvider.Language.Classes.MapFragments;
        }

        var categoryToMatch = new ItemClassDefinition()
        {
            Text = classLine,
            FuzzyText = fuzzyService.CleanFuzzyText(classLine)
        };
        var apiItemCategoryId = Process.ExtractOne(categoryToMatch, ApiItemClassDefinitions.Value, x => x.FuzzyText, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;

        item.Properties.ItemClass = apiItemCategoryId?.GetEnumFromValue<ItemClass>() ?? ItemClass.Unknown;
    }

    public override async Task<PropertyFilter?> GetFilter(Item item, double normalizeValue, FilterType filterType)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return null;
        if (item.Properties.ItemClass == ItemClass.Unknown) return null;

        var classLabel = filterProvider.TypeCategory?.Option.Options.FirstOrDefault(x => x.Id == item.Properties.ItemClass.GetValueAttribute())?.Text;
        if (classLabel == null || item.ApiInformation.Type == null) return null;

        var preferItemClass = await settingsService.GetEnum<DefaultItemClassFilter>(SettingKeys.PriceCheckItemClassFilter) ?? DefaultItemClassFilter.BaseType;

        var filter = new ItemClassPropertyFilter(this)
        {
            Text = gameLanguageProvider.Language.DescriptionRarity,
            ItemClass = classLabel,
            BaseType = item.ApiInformation.Type,
            Checked = preferItemClass == DefaultItemClassFilter.ItemClass,
        };
        return filter;
    }

    public override void PrepareTradeRequest(Query query, Item item, PropertyFilter filter)
    {
        if (!filter.Checked || filter is not ItemClassPropertyFilter) return;

        query.Type = null;
        query.Filters.GetOrCreateTypeFilters().Filters.Category = GetCategoryFilter(item.Properties.ItemClass);
    }

    private static SearchFilterOption? GetCategoryFilter(ItemClass itemClass)
    {
        var enumValue = itemClass.GetValueAttribute();
        if (string.IsNullOrEmpty(enumValue)) return null;

        return new SearchFilterOption(enumValue);
    }
}
