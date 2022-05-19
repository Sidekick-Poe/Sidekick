using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;
using Sidekick.Common.Game.Languages;

namespace Sidekick.Apis.Poe.Parser.Patterns
{
    public class ParserPatterns : IParserPatterns
    {
        private readonly IGameLanguageProvider gameLanguageProvider;

        public ParserPatterns(IGameLanguageProvider gameLanguageProvider)
        {
            this.gameLanguageProvider = gameLanguageProvider;
        }

        public void Initialize()
        {
            InitHeader();
            InitProperties();
            InitSockets();
            InitInfluences();
            InitClasses();
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

            ItemLevel = gameLanguageProvider.Language.DescriptionItemLevel.ToRegexIntCapture();
            Unidentified = gameLanguageProvider.Language.DescriptionUnidentified.ToRegexLine();
            IsRelic = gameLanguageProvider.Language.DescriptionIsRelic.ToRegexLine();
            Corrupted = gameLanguageProvider.Language.DescriptionCorrupted.ToRegexLine();
            Scourged = gameLanguageProvider.Language.DescriptionScourged.ToRegexLine();
        }
        public Dictionary<Rarity, Regex> Rarity { get; private set; }
        public Regex ItemLevel { get; private set; }
        public Regex Unidentified { get; private set; }
        public Regex Corrupted { get; private set; }
        public Regex Scourged { get; private set; }
        public Regex IsRelic { get; private set; }
        #endregion Header (Rarity, Name, Type)

        #region Properties (Armour, Evasion, Energy Shield, Quality, Level)
        private void InitProperties()
        {
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
            ItemQuantity = gameLanguageProvider.Language.DescriptionItemQuantity.ToRegexIntCapture();
            ItemRarity = gameLanguageProvider.Language.DescriptionItemRarity.ToRegexIntCapture();
            MonsterPackSize = gameLanguageProvider.Language.DescriptionMonsterPackSize.ToRegexIntCapture();
            Blighted = gameLanguageProvider.Language.PrefixBlighted.ToRegexStartOfLine();
        }

        public Regex Armor { get; private set; }
        public Regex EnergyShield { get; private set; }
        public Regex Evasion { get; private set; }
        public Regex ChanceToBlock { get; private set; }
        public Regex Quality { get; private set; }
        public Regex AlternateQuality { get; private set; }
        public Regex Level { get; private set; }
        public Regex MapTier { get; private set; }
        public Regex ItemQuantity { get; private set; }
        public Regex ItemRarity { get; private set; }
        public Regex MonsterPackSize { get; private set; }
        public Regex AttacksPerSecond { get; private set; }
        public Regex CriticalStrikeChance { get; private set; }
        public Regex ElementalDamage { get; private set; }
        public Regex PhysicalDamage { get; private set; }
        public Regex Blighted { get; private set; }
        #endregion Properties (Armour, Evasion, Energy Shield, Quality, Level)

        #region Sockets
        private void InitSockets()
        {
            // We need 6 capturing groups as it is possible for a 6 socket unlinked item to exist
            Socket = new Regex($"{Regex.Escape(gameLanguageProvider.Language.DescriptionSockets)}.*?([-RGBWA]+)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)\\ ?([-RGBWA]*)");
        }

        public Regex Socket { get; private set; }
        #endregion Sockets

        #region Influences
        private void InitInfluences()
        {
            Crusader = gameLanguageProvider.Language.InfluenceCrusader.ToRegexStartOfLine();
            Elder = gameLanguageProvider.Language.InfluenceElder.ToRegexStartOfLine();
            Hunter = gameLanguageProvider.Language.InfluenceHunter.ToRegexStartOfLine();
            Redeemer = gameLanguageProvider.Language.InfluenceRedeemer.ToRegexStartOfLine();
            Shaper = gameLanguageProvider.Language.InfluenceShaper.ToRegexStartOfLine();
            Warlord = gameLanguageProvider.Language.InfluenceWarlord.ToRegexStartOfLine();
        }

        public Regex Crusader { get; private set; }
        public Regex Elder { get; private set; }
        public Regex Hunter { get; private set; }
        public Regex Redeemer { get; private set; }
        public Regex Shaper { get; private set; }
        public Regex Warlord { get; private set; }
        #endregion Influences

        #region Classes
        public Dictionary<Class, Regex> Classes { get; } = new Dictionary<Class, Regex>();

        private void InitClasses()
        {
            Classes.Clear();

            if (gameLanguageProvider.Language.Classes == null) return;

            var type = gameLanguageProvider.Language.Classes.GetType();
            var properties = type.GetProperties().Where(x => x.Name != nameof(ClassLanguage.Prefix));
            var prefix = gameLanguageProvider.Language.Classes.Prefix;

            foreach (var property in properties)
            {
                var label = property.GetValue(gameLanguageProvider.Language.Classes).ToString();
                if (string.IsNullOrEmpty(label)) continue;

                Classes.Add(Enum.Parse<Class>(property.Name), $"{prefix}{label}".ToRegexLine());
            }
        }
        #endregion
    }
}
