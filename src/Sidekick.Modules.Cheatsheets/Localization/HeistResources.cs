using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Cheatsheets.Localization
{
    public class HeistResources
    {
        private readonly IStringLocalizer<HeistResources> localizer;

        public HeistResources(IStringLocalizer<HeistResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Ally_Adiyah => localizer["Ally_Adiyah"];
        public string Ally_Gianna => localizer["Ally_Gianna"];
        public string Ally_Huck => localizer["Ally_Huck"];
        public string Ally_Isla => localizer["Ally_Isla"];
        public string Ally_Karst => localizer["Ally_Karst"];
        public string Ally_Kurai => localizer["Ally_Kurai"];
        public string Ally_Nenet => localizer["Ally_Nenet"];
        public string Ally_Niles => localizer["Ally_Niles"];
        public string Ally_Tibbs => localizer["Ally_Tibbs"];
        public string Ally_Tullina => localizer["Ally_Tullina"];
        public string Ally_Vinderi => localizer["Ally_Vinderi"];
        public string Job_Agility => localizer["Job_Agility"];
        public string Job_BruteForce => localizer["Job_BruteForce"];
        public string Job_CounterThaumaturgy => localizer["Job_CounterThaumaturgy"];
        public string Job_Deception => localizer["Job_Deception"];
        public string Job_Demolition => localizer["Job_Demolition"];
        public string Job_Engineering => localizer["Job_Engineering"];
        public string Job_Lockpicking => localizer["Job_Lockpicking"];
        public string Job_Perception => localizer["Job_Perception"];
        public string Job_TrapDisarmament => localizer["Job_TrapDisarmament"];
        public string MaxLevel => localizer["MaxLevel"];
        public string Reward_Abyss => localizer["Reward_Abyss"];
        public string Reward_Accessories => localizer["Reward_Accessories"];
        public string Reward_Armour => localizer["Reward_Armour"];
        public string Reward_Breach => localizer["Reward_Breach"];
        public string Reward_Currency => localizer["Reward_Currency"];
        public string Reward_DivinationCards => localizer["Reward_DivinationCards"];
        public string Reward_Essences => localizer["Reward_Essences"];
        public string Reward_Fossils => localizer["Reward_Fossils"];
        public string Reward_Fragments => localizer["Reward_Fragments"];
        public string Reward_Harbinger => localizer["Reward_Harbinger"];
        public string Reward_Legion => localizer["Reward_Legion"];
        public string Reward_Maps => localizer["Reward_Maps"];
        public string Reward_Talismans => localizer["Reward_Talismans"];
        public string Reward_Uniques => localizer["Reward_Uniques"];
        public string Reward_Weapon => localizer["Reward_Weapon"];
        public string Reward_Generic => localizer["Reward_Generic"];
        public string Reward_Blight => localizer["Reward_Blight"];
        public string Reward_Metamorph => localizer["Reward_Metamorph"];
        public string Reward_Delirium => localizer["Reward_Delirium"];
        public string Reward_Gems => localizer["Reward_Gems"];
    }
}
