using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
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
            InitHeader();
            InitInfluences();

            Requirements = gameLanguageProvider.Language.DescriptionRequirements.ToRegexLine();

            return Task.CompletedTask;
        }

        #region Header (Rarity, Name, Type)

        private void InitHeader()
        {
            Rarity = new Dictionary<Rarity, Regex>
            {
                { Common.Game.Items.Rarity.Normal, gameLanguageProvider.Language.RarityNormal.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.Magic, gameLanguageProvider.Language.RarityMagic.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.Rare, gameLanguageProvider.Language.RarityRare.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.Unique, gameLanguageProvider.Language.RarityUnique.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.Currency, gameLanguageProvider.Language.RarityCurrency.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.Gem, gameLanguageProvider.Language.RarityGem.ToRegexEndOfLine() },
                { Common.Game.Items.Rarity.DivinationCard, gameLanguageProvider.Language.RarityDivinationCard.ToRegexEndOfLine() }
            };

        }

        public Dictionary<Rarity, Regex> Rarity { get; private set; } = null!;

        public Regex Scourged { get; private set; } = null!;

        public Regex IsRelic { get; private set; } = null!;

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
