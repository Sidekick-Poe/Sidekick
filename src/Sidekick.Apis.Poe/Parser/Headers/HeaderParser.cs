using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Parser.Headers.Models;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Headers;

public class HeaderParser
(
    IGameLanguageProvider gameLanguageProvider,
    IFuzzyService fuzzyService,
    IFilterProvider filterProvider
) : IHeaderParser
{
    public int Priority => 100;

    private List<ItemCategory> ItemCategories { get; set; } = [];

    private Dictionary<Rarity, Regex> RarityPatterns { get; set; } = [];

    public Task Initialize()
    {
        InitializeItemCategories();
        InitializeRarityPatterns();
        return Task.CompletedTask;
    }

    private void InitializeItemCategories()
    {
        ItemCategories = filterProvider.TypeCategoryOptions.ConvertAll(x => new ItemCategory()
        {
            Id = x.Id,
            Text = x.Text,
            Pattern = GetItemCategoryPattern(x.Id),
            FuzzyText = fuzzyService.CleanFuzzyText(x.Text),
        });
    }

    private Regex BuildRegex(params string[] labels) => new($"^{Regex.Escape(gameLanguageProvider.Language.Classes.Prefix)}:* *(?:{string.Join("|", labels.Select(Regex.Escape))})$");

    private Regex? GetItemCategoryPattern(string? id) => id switch
    {
        "jewel.abyss" => BuildRegex(gameLanguageProvider.Language.Classes.AbyssJewel),
        "gem.activegem" => BuildRegex(gameLanguageProvider.Language.Classes.ActiveSkillGems),
        "accessory.amulet" => BuildRegex(gameLanguageProvider.Language.Classes.Amulet),
        "accessory.belt" => BuildRegex(gameLanguageProvider.Language.Classes.Belt),
        "heistmission.blueprint" => BuildRegex(gameLanguageProvider.Language.Classes.Blueprint),
        "armour.chest" => BuildRegex(gameLanguageProvider.Language.Classes.BodyArmours),
        "armour.boots" => BuildRegex(gameLanguageProvider.Language.Classes.Boots),
        "weapon.bow" => BuildRegex(gameLanguageProvider.Language.Classes.Bows),
        "weapon.claw" => BuildRegex(gameLanguageProvider.Language.Classes.Claws),
        "heistmission.contract" => BuildRegex(gameLanguageProvider.Language.Classes.Contract),
        "weapon.dagger" => BuildRegex(gameLanguageProvider.Language.Classes.Daggers),
        "currency.resonator" => BuildRegex(gameLanguageProvider.Language.Classes.DelveStackableSocketableCurrency),
        "card" => BuildRegex(gameLanguageProvider.Language.Classes.DivinationCard),
        "armour.gloves" => BuildRegex(gameLanguageProvider.Language.Classes.Gloves),
        "heistequipment.heistreward" => BuildRegex(gameLanguageProvider.Language.Classes.HeistBrooch),
        "heistequipment.heistutility" => BuildRegex(gameLanguageProvider.Language.Classes.HeistCloak),
        "heistequipment.heistweapon" => BuildRegex(gameLanguageProvider.Language.Classes.HeistGear),
        "currency.heistobjective" => BuildRegex(gameLanguageProvider.Language.Classes.HeistTarget),
        "heistequipment.heisttool" => BuildRegex(gameLanguageProvider.Language.Classes.HeistTool),
        "armour.helmet" => BuildRegex(gameLanguageProvider.Language.Classes.Helmets),
        "jewel.base" => BuildRegex(gameLanguageProvider.Language.Classes.Jewel),
        "logbook" => BuildRegex(gameLanguageProvider.Language.Classes.Logbooks),
        "map.fragment" => BuildRegex(gameLanguageProvider.Language.Classes.MapFragments),
        "map" => BuildRegex(gameLanguageProvider.Language.Classes.Maps),
        "monster.sample" => BuildRegex(gameLanguageProvider.Language.Classes.MetamorphSample),
        "weapon.oneaxe" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandAxes),
        "weapon.onemace" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandMaces),
        "weapon.onesword" => BuildRegex(gameLanguageProvider.Language.Classes.OneHandSwords),
        "armour.quiver" => BuildRegex(gameLanguageProvider.Language.Classes.Quivers),
        "accessory.ring" => BuildRegex(gameLanguageProvider.Language.Classes.Ring),
        "weapon.runedagger" => BuildRegex(gameLanguageProvider.Language.Classes.RuneDaggers),
        "weapon.sceptre" => BuildRegex(gameLanguageProvider.Language.Classes.Sceptres),
        "armour.shield" => BuildRegex(gameLanguageProvider.Language.Classes.Shields),
        "weapon.staff" => BuildRegex(gameLanguageProvider.Language.Classes.Staves),
        "gem.supportgem" => BuildRegex(gameLanguageProvider.Language.Classes.SupportSkillGems),
        "accessory.trinket" => BuildRegex(gameLanguageProvider.Language.Classes.Trinkets),
        "weapon.twoaxe" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandAxes),
        "weapon.twomace" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandMaces),
        "weapon.twosword" => BuildRegex(gameLanguageProvider.Language.Classes.TwoHandSwords),
        "flask" => BuildRegex(gameLanguageProvider.Language.Classes.HybridFlasks, gameLanguageProvider.Language.Classes.LifeFlasks, gameLanguageProvider.Language.Classes.ManaFlasks, gameLanguageProvider.Language.Classes.UtilityFlasks),
        "weapon.wand" => BuildRegex(gameLanguageProvider.Language.Classes.Wands),
        "weapon.warstaff" => BuildRegex(gameLanguageProvider.Language.Classes.Warstaves),
        "memoryline" => BuildRegex(gameLanguageProvider.Language.Classes.MemoryLine),
        "azmeri.tincture" => BuildRegex(gameLanguageProvider.Language.Classes.Tinctures),
        "azmeri.corpse" => BuildRegex(gameLanguageProvider.Language.Classes.Corpses),
        _ => null,
    };

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
