using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Languages;

namespace Sidekick.Apis.Poe.Trade.Parser.Headers;

public class RarityParser(IGameLanguageProvider gameLanguageProvider) : IRarityParser
{
    public int Priority => 200;

    private Dictionary<Rarity, Regex> RarityPatterns { get; set; } = [];

    public Task Initialize()
    {
        InitializeRarityPatterns();
        return Task.CompletedTask;
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

    public Rarity Parse(ParsingItem parsingItem)
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
