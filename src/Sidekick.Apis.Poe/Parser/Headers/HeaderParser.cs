using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Fuzzy;
using Sidekick.Apis.Poe.Parser.Headers.Models;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Headers;

public class HeaderParser
(
    IGameLanguageProvider gameLanguageProvider,
    IFuzzyService fuzzyService,
    IFilterProvider filterProvider,
    ILogger<HeaderParser> logger
) : IHeaderParser
{
    public int Priority => 100;

    private List<ItemCategory> ItemCategories { get; set; } = [];

    public Task Initialize()
    {
        ItemCategories = filterProvider.TypeCategoryOptions.ConvertAll(x => new ItemCategory()
        {
            Id = x.Id,
            Text = x.Text,
            FuzzyText = fuzzyService.CleanFuzzyText(x.Text),
        });

        return Task.CompletedTask;
    }

    public Header Parse(string itemText)
    {
        if (string.IsNullOrEmpty(itemText))
        {
            throw new UnparsableException();
        }

        try
        {
            return Parse(new ParsingItem(itemText));
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Could not parse item.");
            throw new UnparsableException();
        }
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
            apiItemCategoryId = Process.ExtractOne(categoryToMatch, ItemCategories, x => x.Text, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;
        }
        else
        {
            apiItemCategoryId = null;
        }

        return new Header()
        {
            Name = parsingItem.Blocks[0].Lines.ElementAtOrDefault(2)?.Text,
            Type = parsingItem.Blocks[0].Lines.ElementAtOrDefault(3)?.Text,
            ItemCategory = apiItemCategoryId
        };
    }

}

