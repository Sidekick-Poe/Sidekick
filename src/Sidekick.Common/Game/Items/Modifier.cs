namespace Sidekick.Common.Game.Items
{
    public class Modifier
    {
        public Modifier(string text)
        {
            Text = text;
        }

        public string? Id { get; set; }

        public string? Tier { get; set; }

        public string? TierName { get; set; }

        public ModifierCategory Category { get; set; }

        public string Text { get; set; }
    }
}
