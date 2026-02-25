using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade.Localization;
using Sidekick.Apis.Poe.Trade.Parser.Properties.Definitions.Models;
using Sidekick.Apis.Poe.Trade.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests;
using Sidekick.Apis.Poe.Trade.Trade.Items.Requests.Filters;
using Sidekick.Common.Enums;
using Sidekick.Data.Fuzzy;
using Sidekick.Data.Items.Models;
using Sidekick.Data.Languages;

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
            CreateItemClassDefinition(ItemClass.Amulet, currentGameLanguage.Language.ClassAmulet),
            CreateItemClassDefinition(ItemClass.Belt, currentGameLanguage.Language.ClassBelt),
            CreateItemClassDefinition(ItemClass.Ring, currentGameLanguage.Language.ClassRing),
            CreateItemClassDefinition(ItemClass.Trinket, currentGameLanguage.Language.ClassTrinkets),

            CreateItemClassDefinition(ItemClass.BodyArmour, currentGameLanguage.Language.ClassBodyArmours),
            CreateItemClassDefinition(ItemClass.Boots, currentGameLanguage.Language.ClassBoots),
            CreateItemClassDefinition(ItemClass.Gloves, currentGameLanguage.Language.ClassGloves),
            CreateItemClassDefinition(ItemClass.Helmet, currentGameLanguage.Language.ClassHelmets),
            CreateItemClassDefinition(ItemClass.Quiver, currentGameLanguage.Language.ClassQuivers),
            CreateItemClassDefinition(ItemClass.Shield, currentGameLanguage.Language.ClassShields),
            CreateItemClassDefinition(ItemClass.Focus, currentGameLanguage.Language.ClassFocus),
            CreateItemClassDefinition(ItemClass.Buckler, currentGameLanguage.Language.ClassBucklers),

            CreateItemClassDefinition(ItemClass.DivinationCard, currentGameLanguage.Language.ClassDivinationCard),

            CreateItemClassDefinition(ItemClass.Resonator, currentGameLanguage.Language.ClassDelveStackableSocketableCurrency),
            CreateItemClassDefinition(ItemClass.HeistObjective, currentGameLanguage.Language.ClassHeistTarget),
            CreateItemClassDefinition(ItemClass.Omen, currentGameLanguage.Language.ClassOmen),
            CreateItemClassDefinition(ItemClass.Socketable, currentGameLanguage.Language.ClassSocketable),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassUniqueFragment),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassFossil),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassIncubator),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassTattoo),

            CreateItemClassDefinition(ItemClass.ActiveGem, currentGameLanguage.Language.ClassActiveSkillGems),
            CreateItemClassDefinition(ItemClass.SupportGem, currentGameLanguage.Language.ClassSupportSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSkillGem, currentGameLanguage.Language.ClassUncutSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSupportGem, currentGameLanguage.Language.ClassUncutSupportGems),
            CreateItemClassDefinition(ItemClass.UncutSpiritGem, currentGameLanguage.Language.ClassUncutSpiritGems),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassAwakenedSupportSkillGems),

            CreateItemClassDefinition(ItemClass.Blueprint, currentGameLanguage.Language.ClassBlueprint),
            CreateItemClassDefinition(ItemClass.Contract, currentGameLanguage.Language.ClassContract),
            CreateItemClassDefinition(ItemClass.HeistReward, currentGameLanguage.Language.ClassHeistBrooch),
            CreateItemClassDefinition(ItemClass.HeistUtility, currentGameLanguage.Language.ClassHeistCloak),
            CreateItemClassDefinition(ItemClass.HeistWeapon, currentGameLanguage.Language.ClassHeistGear),
            CreateItemClassDefinition(ItemClass.HeistTool, currentGameLanguage.Language.ClassHeistTool),

            CreateItemClassDefinition(ItemClass.Jewel, currentGameLanguage.Language.ClassJewel),
            CreateItemClassDefinition(ItemClass.AbyssJewel, currentGameLanguage.Language.ClassAbyssJewel),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassClusterJewel),

            CreateItemClassDefinition(ItemClass.Logbook, currentGameLanguage.Language.ClassLogbooks),

            CreateItemClassDefinition(ItemClass.Waystone, currentGameLanguage.Language.ClassWaystone),
            CreateItemClassDefinition(ItemClass.Breachstone, currentGameLanguage.Language.ClassBreachstone),
            CreateItemClassDefinition(ItemClass.Barya, currentGameLanguage.Language.ClassBarya),
            CreateItemClassDefinition(ItemClass.BossKey, currentGameLanguage.Language.ClassBossKey),
            CreateItemClassDefinition(ItemClass.Ultimatum, currentGameLanguage.Language.ClassUltimatum),
            CreateItemClassDefinition(ItemClass.Tablet, currentGameLanguage.Language.ClassTablet),
            CreateItemClassDefinition(ItemClass.MapFragment, currentGameLanguage.Language.ClassMapFragments),

            CreateItemClassDefinition(ItemClass.MemoryLine, currentGameLanguage.Language.ClassMemoryLine),

            CreateItemClassDefinition(ItemClass.Bow, currentGameLanguage.Language.ClassBows),
            CreateItemClassDefinition(ItemClass.Crossbow, currentGameLanguage.Language.ClassCrossbows),
            CreateItemClassDefinition(ItemClass.Claw, currentGameLanguage.Language.ClassClaws),
            CreateItemClassDefinition(ItemClass.Dagger, currentGameLanguage.Language.ClassDaggers),
            CreateItemClassDefinition(ItemClass.RuneDagger, currentGameLanguage.Language.ClassRuneDaggers),
            CreateItemClassDefinition(ItemClass.OneHandAxe, currentGameLanguage.Language.ClassOneHandAxes),
            CreateItemClassDefinition(ItemClass.OneHandMace, currentGameLanguage.Language.ClassOneHandMaces),
            CreateItemClassDefinition(ItemClass.OneHandSword, currentGameLanguage.Language.ClassOneHandSwords),
            CreateItemClassDefinition(ItemClass.Sceptre, currentGameLanguage.Language.ClassSceptres),
            CreateItemClassDefinition(ItemClass.Staff, currentGameLanguage.Language.ClassStaves),
            CreateItemClassDefinition(ItemClass.Spear, currentGameLanguage.Language.ClassSpears),
            CreateItemClassDefinition(ItemClass.Talisman, currentGameLanguage.Language.ClassTalismans),

            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassFlails),
            // GetItemClass(ItemClass.Unknown, GameLanguageProvider.Language.ClassRapiers),
            CreateItemClassDefinition(ItemClass.FishingRod, currentGameLanguage.Language.ClassFishingRods),
            CreateItemClassDefinition(ItemClass.TwoHandAxe, currentGameLanguage.Language.ClassTwoHandAxes),
            CreateItemClassDefinition(ItemClass.TwoHandMace, currentGameLanguage.Language.ClassTwoHandMaces),
            CreateItemClassDefinition(ItemClass.TwoHandSword, currentGameLanguage.Language.ClassTwoHandSwords),
            CreateItemClassDefinition(ItemClass.Wand, currentGameLanguage.Language.ClassWands),
            CreateItemClassDefinition(ItemClass.Warstaff, currentGameLanguage.Language.ClassWarstaves, currentGameLanguage.Language.ClassQuarterstaves),

            CreateItemClassDefinition(ItemClass.Tincture, currentGameLanguage.Language.ClassTinctures),
            CreateItemClassDefinition(ItemClass.Corpse, currentGameLanguage.Language.ClassCorpses),
            CreateItemClassDefinition(ItemClass.Charms, currentGameLanguage.Language.ClassCharms),

            CreateItemClassDefinition(ItemClass.SanctumRelic, currentGameLanguage.Language.ClassSanctumRelics),
            CreateItemClassDefinition(ItemClass.SanctumResearch, currentGameLanguage.Language.ClassSanctumResearch),
        ];

        if (game == GameType.PathOfExile2)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.LifeFlask, currentGameLanguage.Language.ClassLifeFlasks),
                CreateItemClassDefinition(ItemClass.ManaFlask, currentGameLanguage.Language.ClassManaFlasks),
            ]);
        }

        if (game == GameType.PathOfExile1)
        {
            definitions.AddRange([
                CreateItemClassDefinition(ItemClass.Flask, currentGameLanguage.Language.ClassHybridFlasks),
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

    private Regex BuildRegex(params string[] labels) => new($"^{Regex.Escape(currentGameLanguage.Language.ClassPrefix)}:* *(?:{string.Join("|", labels.Select(Regex.Escape))})$");

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

        var classLine = line.Replace(currentGameLanguage.Language.ClassPrefix, "").Trim(' ', ':');

        // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
        if (classLine == currentGameLanguage.Language.ClassMiscMapItems)
        {
            classLine = currentGameLanguage.Language.ClassMapFragments;
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
