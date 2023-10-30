namespace Sidekick.Common.Game.Items.AdditionalInformation
{
    public class ClusterJewelInformation
    {
        public List<string> GrantTexts { get; } = new();

        public int SmallPassiveCount { get; set; }

        public int ItemLevel { get; set; }

        public int NormalizedItemLevel
        {
            get
            {
                if (ItemLevel >= 84) return 84;
                if (ItemLevel >= 75) return 75;
                if (ItemLevel >= 68) return 68;
                if (ItemLevel >= 50) return 50;
                return 1;
            }
        }
    }
}
