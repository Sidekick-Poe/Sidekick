using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Sidekick.Apis.Poe.Trade.Filters;
using Sidekick.Apis.Poe.Trade.Fuzzy;
using Sidekick.Apis.Poe.Trade.Parser.Headers.Models;
using Sidekick.Common.Enums;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public class ItemClassParser
(
    IGameLanguageProvider gameLanguageProvider,
    IFuzzyService fuzzyService,
    IFilterProvider filterProvider,
    ISettingsService settingsService
) : IItemClassParser
{
    public int Priority => 300;

    private List<ItemClassDefinition> ItemClassDefinitions { get; set; } = [];

    private List<ApiItemClassDefinition> ApiItemClassDefinitions { get; set; } = [];

    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        ItemClassDefinitions = GetItemClassDefinitions(game);
        ApiItemClassDefinitions = GetApiItemClassDefinitions();
    }

    private List<ApiItemClassDefinition> GetApiItemClassDefinitions()
    {
        if (filterProvider.TypeCategory == null) return [];

        return filterProvider.TypeCategory.Option.Options.ConvertAll(x => new ApiItemClassDefinition()
        {
            Id = x.Id,
            Text = x.Text,
            FuzzyText = x.Text is null ? null : fuzzyService.CleanFuzzyText(x.Text),
        });
    }

    private List<ItemClassDefinition> GetItemClassDefinitions(GameType game)
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

            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.UniqueFragment),
            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.Fossil),
            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.Incubator),
            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.Tattoo),

            CreateItemClassDefinition(ItemClass.ActiveGem, gameLanguageProvider.Language.Classes.ActiveSkillGems),
            CreateItemClassDefinition(ItemClass.SupportGem, gameLanguageProvider.Language.Classes.SupportSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSkillGem, gameLanguageProvider.Language.Classes.UncutSkillGems),
            CreateItemClassDefinition(ItemClass.UncutSupportGem, gameLanguageProvider.Language.Classes.UncutSupportGems),
            CreateItemClassDefinition(ItemClass.UncutSpiritGem, gameLanguageProvider.Language.Classes.UncutSpiritGems),

            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.AwakenedSupportSkillGems),

            CreateItemClassDefinition(ItemClass.Blueprint, gameLanguageProvider.Language.Classes.Blueprint),
            CreateItemClassDefinition(ItemClass.Contract, gameLanguageProvider.Language.Classes.Contract),
            CreateItemClassDefinition(ItemClass.HeistReward, gameLanguageProvider.Language.Classes.HeistBrooch),
            CreateItemClassDefinition(ItemClass.HeistUtility, gameLanguageProvider.Language.Classes.HeistCloak),
            CreateItemClassDefinition(ItemClass.HeistWeapon, gameLanguageProvider.Language.Classes.HeistGear),
            CreateItemClassDefinition(ItemClass.HeistTool, gameLanguageProvider.Language.Classes.HeistTool),

            CreateItemClassDefinition(ItemClass.Jewel, gameLanguageProvider.Language.Classes.Jewel),
            CreateItemClassDefinition(ItemClass.AbyssJewel, gameLanguageProvider.Language.Classes.AbyssJewel),

            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.ClusterJewel),

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

            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.Flails),
            // GetItemClass(ItemClass.Unknown, gameLanguageProvider.Language.Classes.Rapiers),
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

    public ItemClass Parse(ParsingItem parsingItem)
    {
        var line = parsingItem.Blocks[0].Lines[0].Text;

        foreach (var definition in ItemClassDefinitions)
        {
            if (definition.Pattern?.IsMatch(line) ?? false) return definition.ItemClass;
        }

        var classLine = line.Replace(gameLanguageProvider.Language.Classes.Prefix, "").Trim(' ', ':');

        // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
        if (classLine == gameLanguageProvider.Language.Classes.MiscMapItems)
        {
            classLine = gameLanguageProvider.Language.Classes.MapFragments;
        }

        var categoryToMatch = new ApiItemClassDefinition()
        {
            Text = classLine,
            FuzzyText = fuzzyService.CleanFuzzyText(classLine)
        };
        var apiItemCategoryId = Process.ExtractOne(categoryToMatch, ApiItemClassDefinitions, x => x.FuzzyText, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;

        return apiItemCategoryId?.GetEnumFromValue<ItemClass>() ?? ItemClass.Unknown;
    }
}
