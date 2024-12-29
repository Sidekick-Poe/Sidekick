using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Parser.Headers.Models;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Common.Extensions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Parser.Headers;

public class HeaderParser
(
    IGameLanguageProvider gameLanguageProvider,
    IFuzzyService fuzzyService,
    IFilterProvider filterProvider,
    ISettingsService settingsService
) : IHeaderParser
{
    public int Priority => 100;

    private List<ItemCategory> ItemCategories { get; set; } = [];

    private Dictionary<Rarity, Regex> RarityPatterns { get; set; } = [];

    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        InitializeItemCategories(game);
        InitializeRarityPatterns();
    }

    private void InitializeItemCategories(GameType game)
    {
        ItemCategories = filterProvider.TypeCategoryOptions.ConvertAll(x => new ItemCategory()
        {
            Id = x.Id,
            Text = x.Text,
            Pattern = GetItemCategoryPattern(x.Id, game),
            FuzzyText = fuzzyService.CleanFuzzyText(x.Text),
        });
    }

    private Regex BuildRegex(params string[] labels) => new($"^{Regex.Escape(gameLanguageProvider.Language.Classes.Prefix)}:* *(?:{string.Join("|", labels.Select(Regex.Escape))})$");

    private Regex? GetItemCategoryPattern(string? id, GameType game)
    {
        if (game == GameType.PathOfExile2)
        {
            switch (id)
            {
                case "flask.life": return BuildRegex(gameLanguageProvider.Language.Classes.LifeFlasks);
                case "flask.mana": return BuildRegex(gameLanguageProvider.Language.Classes.ManaFlasks);

                // case "gem.metagem": return BuildRegex(gameLanguageProvider.Language.Classes.MetaGems);
                // case "currency.rune": return BuildRegex(gameLanguageProvider.Language.Classes.Rune);
                // case "currency.soulcore": return BuildRegex(gameLanguageProvider.Language.Classes.Soulcore);
            }
        }

        if (game == GameType.PathOfExile)
        {
            switch (id)
            {
                case "flask": return BuildRegex(gameLanguageProvider.Language.Classes.HybridFlasks, gameLanguageProvider.Language.Classes.LifeFlasks, gameLanguageProvider.Language.Classes.ManaFlasks, gameLanguageProvider.Language.Classes.UtilityFlasks);
            }
        }

        return id switch
        {
            "accessory.amulet" => BuildRegex(gameLanguageProvider.Language.Classes.Amulet),
            "accessory.belt" => BuildRegex(gameLanguageProvider.Language.Classes.Belt),
            "accessory.ring" => BuildRegex(gameLanguageProvider.Language.Classes.Ring),
            "accessory.trinket" => BuildRegex(gameLanguageProvider.Language.Classes.Trinkets),

            "armour.chest" => BuildRegex(gameLanguageProvider.Language.Classes.BodyArmours),
            "armour.boots" => BuildRegex(gameLanguageProvider.Language.Classes.Boots),
            "armour.gloves" => BuildRegex(gameLanguageProvider.Language.Classes.Gloves),
            "armour.helmet" => BuildRegex(gameLanguageProvider.Language.Classes.Helmets),
            "armour.quiver" => BuildRegex(gameLanguageProvider.Language.Classes.Quivers),
            "armour.shield" => BuildRegex(gameLanguageProvider.Language.Classes.Shields),
            "armour.focus" => BuildRegex(gameLanguageProvider.Language.Classes.Focus),
            // "armour.buckler" => BuildRegex(gameLanguageProvider.Language.Classes.Bucklers),

            "card" => BuildRegex(gameLanguageProvider.Language.Classes.DivinationCard),

            "currency.resonator" => BuildRegex(gameLanguageProvider.Language.Classes.DelveStackableSocketableCurrency),
            // "currency.piece" => BuildRegex(gameLanguageProvider.Language.Classes.UniqueFragment),
            // "currency.fossil" => BuildRegex(gameLanguageProvider.Language.Classes.Fossil),
            // "currency.incubator" => BuildRegex(gameLanguageProvider.Language.Classes.Incubator),
            "currency.heistobjective" => BuildRegex(gameLanguageProvider.Language.Classes.HeistTarget),
            "currency.omen" => BuildRegex(gameLanguageProvider.Language.Classes.Omen),
            // "currency.tattoo" => BuildRegex(gameLanguageProvider.Language.Classes.Tattoo),
            "currency.socketable" => BuildRegex(gameLanguageProvider.Language.Classes.Socketable),

            "gem.activegem" => BuildRegex(gameLanguageProvider.Language.Classes.ActiveSkillGems),
            "gem.supportgem" => BuildRegex(gameLanguageProvider.Language.Classes.SupportSkillGems),
            // "gem.supportgemplus" => BuildRegex(gameLanguageProvider.Language.Classes.AwakenedSupportSkillGems),

            "heistmission.blueprint" => BuildRegex(gameLanguageProvider.Language.Classes.Blueprint),
            "heistmission.contract" => BuildRegex(gameLanguageProvider.Language.Classes.Contract),
            "heistequipment.heistreward" => BuildRegex(gameLanguageProvider.Language.Classes.HeistBrooch),
            "heistequipment.heistutility" => BuildRegex(gameLanguageProvider.Language.Classes.HeistCloak),
            "heistequipment.heistweapon" => BuildRegex(gameLanguageProvider.Language.Classes.HeistGear),
            "heistequipment.heisttool" => BuildRegex(gameLanguageProvider.Language.Classes.HeistTool),

            "jewel" => BuildRegex(gameLanguageProvider.Language.Classes.Jewel),
            "jewel.abyss" => BuildRegex(gameLanguageProvider.Language.Classes.AbyssJewel),
            // "jewel.cluster" => BuildRegex(gameLanguageProvider.Language.Classes.ClusterJewel),

            "logbook" => BuildRegex(gameLanguageProvider.Language.Classes.Logbooks),

            "map.waystone" => BuildRegex(gameLanguageProvider.Language.Classes.Waystone),
            "map.breachstone" => BuildRegex(gameLanguageProvider.Language.Classes.Breachstone),
            "map.barya" => BuildRegex(gameLanguageProvider.Language.Classes.Barya),
            "map.bosskey" => BuildRegex(gameLanguageProvider.Language.Classes.BossKey),
            "map.ultimatum" => BuildRegex(gameLanguageProvider.Language.Classes.Ultimatum),
            "map.tablet" => BuildRegex(gameLanguageProvider.Language.Classes.Tablet),
            "map.fragment" => BuildRegex(gameLanguageProvider.Language.Classes.MapFragments),
            "map" => BuildRegex(gameLanguageProvider.Language.Classes.Maps),

            "memoryline" => BuildRegex(gameLanguageProvider.Language.Classes.MemoryLine),
            "monster.sample" => BuildRegex(gameLanguageProvider.Language.Classes.MetamorphSample),

            "weapon.bow" => BuildRegex(gameLanguageProvider.Language.Classes.Bows),
            "weapon.crossbow" => BuildRegex(gameLanguageProvider.Language.Classes.Crossbows),
            "weapon.claw" => BuildRegex(gameLanguageProvider.Language.Classes.Claws),
            "weapon.dagger" => BuildRegex(gameLanguageProvider.Language.Classes.Daggers),
            "weapon.runedagger" => BuildRegex(gameLanguageProvider.Language.Classes.RuneDaggers),
            "weapon.oneaxe" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandAxes),
            "weapon.onemace" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandMaces),
            "weapon.onesword" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandSwords),
            "weapon.sceptre" => BuildRegex(gameLanguageProvider.Language.Classes.Sceptres),
            "weapon.staff" => BuildRegex(gameLanguageProvider.Language.Classes.Staves),
            // "weapon.spear" => BuildRegex(gameLanguageProvider.Language.Classes.Spears),
            // "weapon.flail" => BuildRegex(gameLanguageProvider.Language.Classes.Flails),
            // "weapon.rapier" => BuildRegex(gameLanguageProvider.Language.Classes.Rapiers),
            "weapon.rod" => BuildRegex(gameLanguageProvider.Language.Classes.FishingRods),
            "weapon.twoaxe" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandAxes),
            "weapon.twomace" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandMaces),
            "weapon.twosword" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandSwords),
            "weapon.wand" => BuildRegex(gameLanguageProvider.Language.Classes.Wands),
            "weapon.warstaff" => BuildRegex(gameLanguageProvider.Language.Classes.Warstaves),

            "tincture" => BuildRegex(gameLanguageProvider.Language.Classes.Tinctures),
            "corpse" => BuildRegex(gameLanguageProvider.Language.Classes.Corpses),

            "sanctum.relic" => BuildRegex(gameLanguageProvider.Language.Classes.SanctumRelics),
            "sanctum.research" => BuildRegex(gameLanguageProvider.Language.Classes.SanctumResearch),

            _ => null,
        };
    }

    private void InitializeRarityPatterns()
    {
        RarityPatterns = new Dictionary<Rarity, Regex>
        {
            { Rarity.Normal, gameLanguageProvider.Language.RarityNormal.ToRegexEndOfLine() },
            { Rarity.Magic, gameLanguageProvider.Language.RarityMagic.ToRegexEndOfLine() },
            { Rarity.Rare, gameLanguageProvider.Language.RarityRare.ToRegexEndOfLine() },
            { Rarity.Unique, gameLanguageProvider.Language.RarityUnique.ToRegexEndOfLine() },
            { Rarity.Currency, gameLanguageProvider.Language.RarityCurrency.ToRegexEndOfLine() },
            { Rarity.Gem, gameLanguageProvider.Language.RarityGem.ToRegexEndOfLine() },
            { Rarity.DivinationCard, gameLanguageProvider.Language.RarityDivinationCard.ToRegexEndOfLine() }
        };
    }

    public Header Parse(ParsingItem parsingItem)
    {
        var apiItemCategoryId = ParseItemCategory(parsingItem);

        string? type = null;
        if (parsingItem.Blocks[0].Lines.Count >= 2)
        {
            type = parsingItem.Blocks[0].Lines[^1].Text;
        }

        string? name = null;
        if (parsingItem.Blocks[0].Lines.Count >= 3 && !parsingItem.Blocks[0].Lines[^2].Parsed)
        {
            name = parsingItem.Blocks[0].Lines[^2].Text;
        }

        return new Header()
        {
            Name = name,
            Type = type,
            ItemCategory = apiItemCategoryId
        };
    }

    private string? ParseItemCategory(ParsingItem parsingItem)
    {
        var firstLine = parsingItem.Blocks[0].Lines[0].Text;
        string? apiItemCategoryId = null;

        foreach (var itemCategory in ItemCategories)
        {
            if (!(itemCategory.Pattern?.IsMatch(firstLine) ?? false))
            {
                continue;
            }

            apiItemCategoryId = itemCategory.Id;
            break;
        }

        if (!string.IsNullOrEmpty(apiItemCategoryId) && !firstLine.StartsWith(gameLanguageProvider.Language.Classes.Prefix))
        {
            return apiItemCategoryId;
        }

        var classLine = firstLine.Replace(gameLanguageProvider.Language.Classes.Prefix, "").Trim(' ', ':');

        // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
        if (classLine == gameLanguageProvider.Language.Classes.MiscMapItems)
        {
            classLine = gameLanguageProvider.Language.Classes.MapFragments;
        }

        var categoryToMatch = new ItemCategory()
        {
            Text = classLine,
            FuzzyText = fuzzyService.CleanFuzzyText(classLine)
        };
        apiItemCategoryId = Process.ExtractOne(categoryToMatch, ItemCategories, x => x.FuzzyText, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;

        return apiItemCategoryId;
    }

    public Rarity ParseRarity(ParsingItem parsingItem)
    {
        foreach (var pattern in RarityPatterns)
        {
            if (!pattern.Value.IsMatch(parsingItem.Blocks[0].Lines[1].Text))
            {
                continue;
            }

            parsingItem.Blocks[0].Lines[1].Parsed = true;
            return pattern.Key;
        }

        return Rarity.Unknown;
    }
}
