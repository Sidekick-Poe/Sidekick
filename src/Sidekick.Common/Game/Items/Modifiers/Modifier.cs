namespace Sidekick.Common.Game.Items.Modifiers
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

        public List<double> Values { get; set; } = new List<double>();

        public int? OptionValue { get; set; }

        public bool HasValue => OptionValue == null && Values.Count > 0;
    }
}
