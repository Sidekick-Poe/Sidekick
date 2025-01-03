using System.Text.RegularExpressions;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public class ParserPatterns(IGameLanguageProvider gameLanguageProvider) : IParserPatterns
    {
        /// <inheritdoc/>
        public int Priority => 100;

        /// <inheritdoc/>
        public Task Initialize()
        {
            InitInfluences();

            Requirements = gameLanguageProvider.Language.DescriptionRequirements.ToRegexLine();

            return Task.CompletedTask;
        }

        #region Header (Rarity, Name, Type)

        public Regex Requirements { get; private set; } = null!;

        #endregion Header (Rarity, Name, Type)

        #region Influences

        private void InitInfluences()
        {
            Crusader = gameLanguageProvider.Language.InfluenceCrusader.ToRegexLine();
            Elder = gameLanguageProvider.Language.InfluenceElder.ToRegexLine();
            Hunter = gameLanguageProvider.Language.InfluenceHunter.ToRegexLine();
            Redeemer = gameLanguageProvider.Language.InfluenceRedeemer.ToRegexLine();
            Shaper = gameLanguageProvider.Language.InfluenceShaper.ToRegexLine();
            Warlord = gameLanguageProvider.Language.InfluenceWarlord.ToRegexLine();
        }

        public Regex Crusader { get; private set; } = null!;

        public Regex Elder { get; private set; } = null!;

        public Regex Hunter { get; private set; } = null!;

        public Regex Redeemer { get; private set; } = null!;

        public Regex Shaper { get; private set; } = null!;

        public Regex Warlord { get; private set; } = null!;

        #endregion Influences
    }
}
