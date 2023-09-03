using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public class ParserPatterns : IParserPatterns
    {
        private readonly IGameLanguageProvider gameLanguageProvider;

        public ParserPatterns(IGameLanguageProvider gameLanguageProvider)
        {
            this.gameLanguageProvider = gameLanguageProvider;
        }

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Medium;

        /// <inheritdoc/>
        public Task Initialize()
        {
            InitHeader();
            InitProperties();
            InitSockets();
            InitInfluences();
            InitClasses();

            return Task.CompletedTask;
        }

        #region Header (Rarity, Name, Type)

        private void InitHeader()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Parser Patterns] Could not find a valid language.");
            }

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

            ItemLevel = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();
            Unidentified = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();
            IsRelic = gameLanguageProvider.Language.DescriptionIsRelic.ToRegexLine();
            Corrupted = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();
            Scourged = gameLanguageProvider.Language.DescriptionScourged.ToRegexLine();
        }

        public Dictionary<Rarity, Regex> Rarity { get; private set; } = null!;
        public Regex ItemLevel { get; private set; } = null!;
        public Regex Unidentified { get; private set; } = null!;
        public Regex Corrupted { get; private set; } = null!;
        public Regex Scourged { get; private set; } = null!;
        public Regex IsRelic { get; private set; } = null!;

        #endregion Header (Rarity, Name, Type)

        #region Properties (Armour, Evasion, Energy Shield, Quality, Level)

        private void InitProperties()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Parser Patterns] Could not find a valid language.");
            }

            Armor = gameLanguageProvider.Language.DescriptionArmour.ToRegexIntCapture();
            EnergyShield = gameLanguageProvider.Language.DescriptionEnergyShield.ToRegexIntCapture();
            Evasion = gameLanguageProvider.Language.DescriptionEvasion.ToRegexIntCapture();
            ChanceToBlock = gameLanguageProvider.Language.DescriptionChanceToBlock.ToRegexIntCapture();
            Level = gameLanguageProvider.Language.DescriptionLevel.ToRegexIntCapture();
            AttacksPerSecond = gameLanguageProvider.Language.DescriptionAttacksPerSecond.ToRegexDecimalCapture();
            CriticalStrikeChance = gameLanguageProvider.Language.DescriptionCriticalStrikeChance.ToRegexDecimalCapture();
            ElementalDamage = gameLanguageProvider.Language.DescriptionElementalDamage.ToRegexStartOfLine();
            PhysicalDamage = gameLanguageProvider.Language.DescriptionPhysicalDamage.ToRegexStartOfLine();

            Quality = gameLanguageProvider.Language.DescriptionQuality.ToRegexIntCapture();
            AlternateQuality = gameLanguageProvider.Language.DescriptionAlternateQuality.ToRegexLine();

            MapTier = gameLanguageProvider.Language.DescriptionMapTier.ToRegexIntCapture();
            AreaLevel = gameLanguageProvider.Language.DescriptionAreaLevel.ToRegexIntCapture();
            ItemQuantity = gameLanguageProvider.Language.DescriptionItemQuantity.ToRegexIntCapture();
            ItemRarity = gameLanguageProvider.Language.DescriptionItemRarity.ToRegexIntCapture();
            MonsterPackSize = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();
            Blighted = gameLanguageProvider.Language.PrefixBlighted.ToRegexStartOfLine();
            BlightRavaged = gameLanguageProvider.Language.PrefixBlightRavaged.ToRegexStartOfLine();

            Requirements = gameLanguageProvider.Language.DescriptionRequirements.ToRegexLine();
        }

        public Regex Armor { get; private set; } = null!;
        public Regex EnergyShield { get; private set; } = null!;
        public Regex Evasion { get; private set; } = null!;
        public Regex ChanceToBlock { get; private set; } = null!;
        public Regex Quality { get; private set; } = null!;
        public Regex AlternateQuality { get; private set; } = null!;
        public Regex Level { get; private set; } = null!;
        public Regex MapTier { get; private set; } = null!;
        public Regex ItemQuantity { get; private set; } = null!;
        public Regex ItemRarity { get; private set; } = null!;
        public Regex MonsterPackSize { get; private set; } = null!;
        public Regex AttacksPerSecond { get; private set; } = null!;
        public Regex CriticalStrikeChance { get; private set; } = null!;
        public Regex ElementalDamage { get; private set; } = null!;
        public Regex PhysicalDamage { get; private set; } = null!;
        public Regex Blighted { get; private set; } = null!;
        public Regex BlightRavaged { get; private set; } = null!;
        public Regex Requirements { get; private set; } = null!;
        public Regex AreaLevel { get; private set; } = null!;

        #endregion Properties (Armour, Evasion, Energy Shield, Quality, Level)

        #region Sockets

        private void InitSockets()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Parser Patterns] Could not find a valid language.");
            }

            // We need 6 capturing groups as it is possible for a 6 socket unlinked item to exist
            Socket = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWA]+)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)");
        }

        public Regex Socket { get; private set; } = null!;

        #endregion Sockets

        #region Influences

        private void InitInfluences()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Parser Patterns] Could not find a valid language.");
            }

            Crusader = gameLanguageProvider.Language.InfluenceCrusader.ToRegexStartOfLine();
            Elder = gameLanguageProvider.Language.InfluenceElder.ToRegexStartOfLine();
            Hunter = gameLanguageProvider.Language.InfluenceHunter.ToRegexStartOfLine();
            Redeemer = gameLanguageProvider.Language.InfluenceRedeemer.ToRegexStartOfLine();
            Shaper = gameLanguageProvider.Language.InfluenceShaper.ToRegexStartOfLine();
            Warlord = gameLanguageProvider.Language.InfluenceWarlord.ToRegexStartOfLine();
        }

        public Regex Crusader { get; private set; } = null!;
        public Regex Elder { get; private set; } = null!;
        public Regex Hunter { get; private set; } = null!;
        public Regex Redeemer { get; private set; } = null!;
        public Regex Shaper { get; private set; } = null!;
        public Regex Warlord { get; private set; } = null!;

        #endregion Influences

        #region Classes

        public Dictionary<Class, Regex> Classes { get; } = new Dictionary<Class, Regex>();

        private void InitClasses()
        {
            if (gameLanguageProvider.Language == null)
            {
                throw new Exception("[Parser Patterns] Could not find a valid language.");
            }

            Classes.Clear();

            if (gameLanguageProvider.Language.Classes == null) return;

            var type = gameLanguageProvider.Language.Classes.GetType();
            var properties = type.GetProperties().Where(x => x.Name != nameof(ClassLanguage.Prefix));
            var prefix = gameLanguageProvider.Language.Classes.Prefix;

            foreach (var property in properties)
            {
                var label = property.GetValue(gameLanguageProvider.Language.Classes)?.ToString();
                if (string.IsNullOrEmpty(label)) continue;

                Classes.Add(Enum.Parse<Class>(property.Name), $"{prefix}{label}".ToRegexLine());
            }
        }

        #endregion Classes
    }
}
