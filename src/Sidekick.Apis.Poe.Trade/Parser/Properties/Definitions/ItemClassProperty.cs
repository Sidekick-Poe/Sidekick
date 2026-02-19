using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Fuzzy;

namespace Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions;

public class ItemClassProperty : PropertyDefinition
{
    private readonly GameType game;
    private readonly ICurrentGameLanguage currentGameLanguage;
    private readonly IStringLocalizer<PoeResources> resources;
    private readonly ITradeFilterProvider tradeFilterProvider;
    private readonly IFuzzyService fuzzyService;

    public ItemClassProperty(
        GameType game,
        ICurrentGameLanguage currentGameLanguage,
        IServiceProvider serviceProvider,
        IStringLocalizer<PoeResources> resources)
    {
        this.game = game;
        this.currentGameLanguage = currentGameLanguage;
        this.resources = resources;
        tradeFilterProvider = serviceProvider.GetRequiredService<ITradeFilterProvider>();
        fuzzyService = serviceProvider.GetRequiredService<IFuzzyService>();

        ItemClassDefinitions = new(GetItemClassDefinitions);
        ApiItemClassDefinitions = new(GetApiItemClassDefinitions);
    }

    public override string Label => resources["Item_Class"];

    private Lazy<List<ItemClassDefinition>> ItemClassDefinitions { get; }

    private Lazy<List<ItemClassDefinition>> ApiItemClassDefinitions { get; }

    private List<ItemClassDefinition> GetApiItemClassDefinitions()
    {
        if (tradeFilterProvider.TypeCategory == null) return [];

        return tradeFilterProvider.TypeCategory.Option.Options.ConvertAll(x => new ItemClassDefinition()
        {
            Id = x.Id,
            Text = x.Text,
            FuzzyText = x.Text is null ? null : fuzzyService.CleanFuzzyText(currentGameLanguage.Language, x.Text),
        });
    }

    private List<ItemClassDefinition> GetItemClassDefinitions()
    {
        List<ItemClassDefinition> definitions =
        [
            CreateItemClassDefinition(ItemClass.Amulet, currentGameLanguage.Language.Classes.Amulet),
            CreateItemClassDefinition(ItemClass.Belt, currentGameLanguage.Language.Classes.Belt),
            CreateItemClassDefinition(ItemClass.Ring, currentGameLanguage.Language.Classes.Ring),
            CreateItemClassDefinition(ItemClass.Trinket, currentGameLanguage.Language.Classes.Trinkets),

            CreateItemClassDefinition(ItemClass.BodyArmour, currentGameLanguage.Language.Classes.BodyArmours),
            CreateItemClassDefinition(ItemClass.Boots, currentGameLanguage.Language.Classes.Boots),
            CreateItemClassDefinition(ItemClass.Gloves, currentGameLanguage.Language.Classes.Gloves),
            CreateItemClassDefinition(ItemClass.Helmet, currentGameLanguage.Language.Classes.Helmets),
            CreateItemClassDefinition(ItemClass.Quiver, currentGameLanguage.Language.Classes.Quivers),
            CreateItemClassDefinition(ItemClass.Shield, currentGameLanguage.Language.Classes.Shields),
            CreateItemClassDefinition(ItemClass.Focus, currentGameLanguage.Language.Classes.Focus),
            CreateItemClassDefinition(ItemClass.Buckler, currentGameLanguage.Language.Classes.Bucklers),

            CreateItemClassDefinition(ItemClass.DivinationCard, currentGameLanguage.Language.Classes.DivinationCard),

            CreateItemClassDefinition(ItemClass.Resonator, currentGameLanguage.Language.Classes.DelveStackableSocketableCurrency),
            CreateItemClassDefinition(ItemClass.HeistObjective, currentGameLanguage.Language.Classes.HeistTarget),
            CreateItemClassDefinition(ItemClass.Omen, currentGameLanguage.Language.Classes.Omen),
            CreateItemClassDefinition(ItemClass.Socketable, currentGameLanguage.Language.Classes.Socketable),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.UniqueFragment),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Fossil),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Incubator),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Tattoo),

            CreateItemClassDefinition(ItemClass.ActiveGem, currentGameLanguage.Language.Classes.ActiveSkillGems),
            CreateItemClassDefinition(ItemClass.SupportGem, currentGameLanguage.Language.Classes.SupportSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSkillGem, currentGameLanguage.Language.Classes.UncutSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSupportGem, currentGameLanguage.Language.Classes.UncutSupportGems),
            CreateItemClassDefinition(ItemClass.UncutSpiritGem, currentGameLanguage.Language.Classes.UncutSpiritGems),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.AwakenedSupportSkillGems),

            CreateItemClassDefinition(ItemClass.Blueprint, currentGameLanguage.Language.Classes.Blueprint),
            CreateItemClassDefinition(ItemClass.Contract, currentGameLanguage.Language.Classes.Contract),
            CreateItemClassDefinition(ItemClass.HeistReward, currentGameLanguage.Language.Classes.HeistBrooch),
            CreateItemClassDefinition(ItemClass.HeistUtility, currentGameLanguage.Language.Classes.HeistCloak),
            CreateItemClassDefinition(ItemClass.HeistWeapon, currentGameLanguage.Language.Classes.HeistGear),
            CreateItemClassDefinition(ItemClass.HeistTool, currentGameLanguage.Language.Classes.HeistTool),

            CreateItemClassDefinition(ItemClass.Jewel, currentGameLanguage.Language.Classes.Jewel),
            CreateItemClassDefinition(ItemClass.AbyssJewel, currentGameLanguage.Language.Classes.AbyssJewel),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.ClusterJewel),

            CreateItemClassDefinition(ItemClass.Logbook, currentGameLanguage.Language.Classes.Logbooks),

            CreateItemClassDefinition(ItemClass.Waystone, currentGameLanguage.Language.Classes.Waystone),
            CreateItemClassDefinition(ItemClass.Breachstone, currentGameLanguage.Language.Classes.Breachstone),
            CreateItemClassDefinition(ItemClass.Barya, currentGameLanguage.Language.Classes.Barya),
            CreateItemClassDefinition(ItemClass.BossKey, currentGameLanguage.Language.Classes.BossKey),
            CreateItemClassDefinition(ItemClass.Ultimatum, currentGameLanguage.Language.Classes.Ultimatum),
            CreateItemClassDefinition(ItemClass.Tablet, currentGameLanguage.Language.Classes.Tablet),
            CreateItemClassDefinition(ItemClass.MapFragment, currentGameLanguage.Language.Classes.MapFragments),

            CreateItemClassDefinition(ItemClass.MemoryLine, currentGameLanguage.Language.Classes.MemoryLine),

            CreateItemClassDefinition(ItemClass.Bow, currentGameLanguage.Language.Classes.Bows),
            CreateItemClassDefinition(ItemClass.Crossbow, currentGameLanguage.Language.Classes.Crossbows),
            CreateItemClassDefinition(ItemClass.Claw, currentGameLanguage.Language.Classes.Claws),
            CreateItemClassDefinition(ItemClass.Dagger, currentGameLanguage.Language.Classes.Daggers),
            CreateItemClassDefinition(ItemClass.RuneDagger, currentGameLanguage.Language.Classes.RuneDaggers),
            CreateItemClassDefinition(ItemClass.OneHandAxe, currentGameLanguage.Language.Classes.OneHandAxes),
            CreateItemClassDefinition(ItemClass.OneHandMace, currentGameLanguage.Language.Classes.OneHandMaces),
            CreateItemClassDefinition(ItemClass.OneHandSword, currentGameLanguage.Language.Classes.OneHandSwords),
            CreateItemClassDefinition(ItemClass.Sceptre, currentGameLanguage.Language.Classes.Sceptres),
            CreateItemClassDefinition(ItemClass.Staff, currentGameLanguage.Language.Classes.Staves),
            CreateItemClassDefinition(ItemClass.Spear, currentGameLanguage.Language.Classes.Spears),
            CreateItemClassDefinition(ItemClass.Talisman, currentGameLanguage.Language.Classes.Talismans),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Flails),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.Classes.Rapiers),
            CreateItemClassDefinition(ItemClass.FishingRod, currentGameLanguage.Language.Classes.FishingRods),
            CreateItemClassDefinition(ItemClass.TwoHandAxe, currentGameLanguage.Language.Classes.TwoHandAxes),
            CreateItemClassDefinition(ItemClass.TwoHandMace, currentGameLanguage.Language.Classes.TwoHandMaces),
            CreateItemClassDefinition(ItemClass.TwoHandSword, currentGameLanguage.Language.Classes.TwoHandSwords),
            CreateItemClassDefinition(ItemClass.Wand, currentGameLanguage.Language.Classes.Wands),
            CreateItemClassDefinition(ItemClass.Warstaff, currentGameLanguage.Language.Classes.Warstaves, currentGameLanguage.Language.Classes.Quarterstaves),

            CreateItemClassDefinition(ItemClass.Tincture, currentGameLanguage.Language.Classes.Tinctures),
            CreateItemClassDefinition(ItemClass.Corpse, currentGameLanguage.Language.Classes.Corpses),
            CreateItemClassDefinition(ItemClass.Charms, currentGameLanguage.Language.Classes.Charms),

            CreateItemClassDefinition(ItemClass.SanctumRelic, currentGameLanguage.Language.Classes.SanctumRelics),
            CreateItemClassDefinition(ItemClass.SanctumResearch, currentGameLanguage.Language.Classes.SanctumResearch),
        ];

        if (game == GameType.PathOfExile2)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.LifeFlask, currentGameLanguage.Language.Classes.LifeFlasks),
                CreateItemClassDefinition(ItemClass.ManaFlask, currentGameLanguage.Language.Classes.ManaFlasks),
            ]);
        }

        if (game == GameType.PathOfExile1)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.Flask, currentGameLanguage.Language.Classes.HybridFlasks),
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
            FuzzyText = text is null ? null : fuzzyService.CleanFuzzyText(currentGameLanguage.Language, text),
        };
    }

    private Regex BuildRegex(params string[] labels) => new($"^{Regex.Escape(currentGameLanguage.Language.Classes.Prefix)}:* *(?:{string.Join("|", labels.Select(Regex.Escape))})$");

    public override void Parse(Item item)
    {
        if (item.Properties.ItemClass != ItemClass.Unknown) return;

        var line = item.Text.Blocks[0].Lines[0].Text;

        foreach (var definition in ItemClassDefinitions.Value)
        {
            if (!(definition.Pattern?.IsMatch(line) ?? false)) continue;

            item.Text.Blocks[0].Lines[0].Parsed = true;
            item.Properties.ItemClass = definition.ItemClass;
            return;
        }

        var classLine = line.Replace(currentGameLanguage.Language.Classes.Prefix, "").Trim(' ', ':');

        // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
        if (classLine == currentGameLanguage.Language.Classes.MiscMapItems)
        {
            classLine = currentGameLanguage.Language.Classes.MapFragments;
        }

        var categoryToMatch = new ItemClassDefinition()
        {
            Text = classLine,
            FuzzyText = fuzzyService.CleanFuzzyText(currentGameLanguage.Language, classLine)
        };
        var apiItemCategoryId = Process.ExtractOne(categoryToMatch, ApiItemClassDefinitions.Value, x => x.FuzzyText, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;

        item.Properties.ItemClass = apiItemCategoryId?.GetEnumFromValue<ItemClass>() ?? ItemClass.Unknown;
    }

    public override Task<TradeFilter?> GetFilter(Item item)
    {
        if (item.Properties.Rarity is not (Rarity.Rare or Rarity.Magic or Rarity.Normal)) return Task.FromResult<TradeFilter?>(null);
        if (item.Properties.ItemClass == ItemClass.Unknown) return Task.FromResult<TradeFilter?>(null);

        var classLabel = tradeFilterProvider.TypeCategory?.Option.Options.FirstOrDefault(x => x.Id == item.Properties.ItemClass.GetValueAttribute())?.Text;
        if (classLabel == null || item.ApiInformation.Type == null) return Task.FromResult<TradeFilter?>(null);

        var filter = new ItemClassFilter
        {
            Text = resources["Item_Class"],
            ItemClass = classLabel,
            BaseTypeText = resources["Base_Type"],
            BaseType = item.ApiInformation.Type,
            AutoSelectSettingKey = $"Trade_Filter_{nameof(ItemClassProperty)}_{game.GetValueAttribute()}",
        };
        return Task.FromResult<TradeFilter?>(filter);
    }
}

public class ItemClassFilter : TradeFilter
{
    public ItemClassFilter()
    {
        DefaultAutoSelect = AutoSelectPreferences.Create(false);
    }

    public required string ItemClass { get; init; }
    public required string BaseType { get; init; }
    public required string BaseTypeText { get; init; }

    public override void PrepareTradeRequest(Query query, Item item)
    {
        if (!Checked) return;

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
