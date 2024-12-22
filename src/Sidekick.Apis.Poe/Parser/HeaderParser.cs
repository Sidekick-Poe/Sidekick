using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Filters;
using Sidekick.Apis.Poe.Metadata;
using Sidekick.Apis.Poe.Metadata.Models;
using Sidekick.Apis.Poe.Parser.AdditionalInformation;
using Sidekick.Apis.Poe.Parser.Patterns;
using Sidekick.Apis.Poe.Pseudo;
using Sidekick.Common.Exceptions;
using Sidekick.Common.Game;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser;

public class HeaderParser
(
    ILogger<HeaderParser> logger,
    IItemMetadataParser itemMetadataProvider,
    IModifierParser modifierParser,
    IPseudoModifierProvider pseudoModifierProvider,
    IParserPatterns patterns,
    ClusterJewelParser clusterJewelParser,
    IInvariantMetadataProvider invariantMetadataProvider,
    SocketParser socketParser,
    PropertyParser propertyParser,
    IGameLanguageProvider gameLanguageProvider,
    IFilterProvider filterProvider
)
{
    
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

            var categoryToMatch = new ApiFilterOption { Text = classLine };
            apiItemCategoryId = Process.ExtractOne(categoryToMatch, filterProvider.ApiItemCategories, x => x.Text, ScorerCache.Get<DefaultRatioScorer>())?.Value?.Id ?? null;
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

