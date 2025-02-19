using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Items.Models;
using Sidekick.Apis.Poe.Parser.Headers.Models;
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
    ISettingsService settingsService,
    IApiItemProvider apiItemProvider
) : IHeaderParser
{
    public int Priority => 200;

    private Regex Affixes { get; set; } = null!;

    private Regex SuperiorAffix { get; set; } = null!;

    private List<ItemCategory> ItemCategories { get; set; } = [];

    private Dictionary<Rarity, Regex> RarityPatterns { get; set; } = [];

    private string GetLineWithoutAffixes(string line) => Affixes.Replace(line, string.Empty).Trim(' ', ',');

    private string GetLineWithoutSuperiorAffix(string line) => SuperiorAffix.Replace(line, string.Empty).Trim(' ', ',');

    public async Task Initialize()
    {
        var leagueId = await settingsService.GetString(SettingKeys.LeagueId);
        var game = leagueId.GetGameFromLeagueId();

        InitializeItemCategories(game);
        InitializeRarityPatterns();

        Regex GetRegexLine(string input)
        {
            if (input.StartsWith('/'))
            {
                input = input.Trim('/');
                return new Regex($"^{input} | {input}$");
            }

            input = Regex.Escape(input);
            return new Regex($"^{input} | {input}$");
        }

        Affixes = new Regex("(?:" + GetRegexLine(gameLanguageProvider.Language.AffixSuperior) + "|" + GetRegexLine(gameLanguageProvider.Language.AffixBlighted) + "|" + GetRegexLine(gameLanguageProvider.Language.AffixBlightRavaged) + ")");
        SuperiorAffix = new Regex("(?:" + GetRegexLine(gameLanguageProvider.Language.AffixSuperior) + ")");
    }

    private void InitializeItemCategories(GameType game)
    {
        ItemCategories = filterProvider.TypeCategoryOptions.ConvertAll(x => new ItemCategory()
        {
            Id = x.Id,
            Text = x.Text,
            Pattern = GetItemCategoryPattern(x.Id, game),
            FuzzyText = x.Text is null ? null : fuzzyService.CleanFuzzyText(x.Text),
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

    public ItemHeader Parse(ParsingItem parsingItem)
    {
        var rarity = ParseRarity(parsingItem);

        string? type = null;
        if (parsingItem.Blocks[0].Lines.Count >= 2)
        {
            type = parsingItem.Blocks[0].Lines[^1].Text;
            parsingItem.Blocks[0].Lines[^1].Parsed = true;
        }

        string? name = null;
        if (parsingItem.Blocks[0].Lines.Count >= 3 && !parsingItem.Blocks[0].Lines[^2].Parsed)
        {
            name = parsingItem.Blocks[0].Lines[^2].Text;
            parsingItem.Blocks[0].Lines[^2].Parsed = true;
        }

        var vaalGem = ParseVaalGem(parsingItem, rarity);
        if (vaalGem != null)
        {
            name = vaalGem.Name;
            type = vaalGem.Type;
        }

        var apiItem = ParseApiItem(rarity, name, type);
        var header = apiItem?.ToHeader() ?? new ItemHeader();
        header.Name = name;
        header.Type = type;
        header.ItemCategory = ParseItemCategory(parsingItem);
        if (header.Rarity == Rarity.Unknown) header.Rarity = rarity;

        parsingItem.Blocks[0].Parsed = true;

        return header;
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

    private ApiItem? ParseApiItem(Rarity rarity, string? name, string? type)
    {
        // Rares may have conflicting names, so we don't want to search any unique items that may have that name. Like "Ancient Orb" which can be used by abyss jewels.
        name = rarity is Rarity.Rare or Rarity.Magic ? null : name;
        name = name != null ? GetLineWithoutSuperiorAffix(name) : null;

        type = type != null ? GetLineWithoutSuperiorAffix(type) : null;

        // We can find multiple matches while parsing. This will store all of them. We will figure out which result is correct at the end of this method.
        var results = new List<ApiItem>();

        // There are some items which have prefixes which we don't want to remove, like the "Blighted Delirium Orb".
        if (!string.IsNullOrEmpty(name) && apiItemProvider.NameAndTypeDictionary.TryGetValue(name, out var itemData))
        {
            results.AddRange(itemData);
        }

        // Here we check without any prefixes
        else if (!string.IsNullOrEmpty(name) && apiItemProvider.NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(name), out itemData))
        {
            results.AddRange(itemData);
        }

        // Now we check the type
        else if (!string.IsNullOrEmpty(type) && apiItemProvider.NameAndTypeDictionary.TryGetValue(type, out itemData))
        {
            results.AddRange(itemData);
        }

        else if (!string.IsNullOrEmpty(type) && apiItemProvider.NameAndTypeDictionary.TryGetValue(GetLineWithoutAffixes(type), out itemData))
        {
            results.AddRange(itemData);
        }

        // Finally. if we don't have any matches, we will look into our regex dictionary
        else
        {
            if (!string.IsNullOrEmpty(name))
            {
                results.AddRange(apiItemProvider.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(name)).Select(x => x.Item));
            }

            if (!string.IsNullOrEmpty(type))
            {
                results.AddRange(apiItemProvider.NameAndTypeRegex.Where(pattern => pattern.Regex.IsMatch(type)).Select(x => x.Item));
            }
        }

        var orderedResults = results.OrderByDescending(x => x.Type?.Length ?? x.Name?.Length ?? 0).ToList();

        if (orderedResults.Any(x => x.Type == type))
        {
            return orderedResults.FirstOrDefault(x => x.Type == type);
        }

        if (orderedResults.Any(x => x.Type == type))
        {
            return orderedResults.FirstOrDefault(x => x.Type == type);
        }

        if (orderedResults.Any(x => x.IsUnique))
        {
            return orderedResults.FirstOrDefault(x => x.IsUnique);
        }

        return orderedResults.FirstOrDefault();
    }

    private ApiItem? ParseVaalGem(ParsingItem parsingItem, Rarity rarity)
    {
        var canBeVaalGem = rarity == Rarity.Gem && parsingItem.Blocks.Count > 7;
        if (!canBeVaalGem || parsingItem.Blocks[5].Lines.Count <= 0)
        {
            return null;
        }

        apiItemProvider.NameAndTypeDictionary.TryGetValue(parsingItem.Blocks[5].Lines[0].Text, out var vaalGem);
        return vaalGem?.First();
    }

    private Rarity ParseRarity(ParsingItem parsingItem)
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

