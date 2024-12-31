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
            FuzzyText = fuzzyService.CleanFuzzyText(x.Text),
        });
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
        var firstLine = parsingItem.Blocks[0].Lines[0].Text;
        string? apiItemCategoryId;

        if (firstLine.StartsWith(gameLanguageProvider.Language.Classes.Prefix))
        {
            var classLine = firstLine.Replace(gameLanguageProvider.Language.Classes.Prefix, "").Trim(' ', ':');

            // There is a weird thing where the API says Map Fragment and the game says Misc Map Items. I thought we could hardcode it here.
            if (classLine == gameLanguageProvider.Language.Classes.MiscMapItems)
            {
                classLine = gameLanguageProvider.Language.Classes.MapFragments;
            }

            var categoryToMatch = new ItemCategory() { Text = classLine, FuzzyText = fuzzyService.CleanFuzzyText(classLine) };
            apiItemCategoryId = Process.ExtractOne(categoryToMatch, ItemCategories, x => x.FuzzyText, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;
        }
        else
        {
            apiItemCategoryId = null;
        }

        string? type = null;
        if (parsingItem.Blocks[0].Lines.Count >= 2)
        {
            type = parsingItem.Blocks[0].Lines[^1].Text;
        }

        string? name = null;
        if (parsingItem.Blocks[0].Lines.Count >= 3)
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

    public Rarity ParseRarity(ParsingItem parsingItem)
    {
        foreach (var pattern in RarityPatterns)
        {
            if (pattern.Value.IsMatch(parsingItem.Blocks[0].Lines[1].Text))
            {
                return pattern.Key;
            }
        }

        return Rarity.Unknown;
    }
}

