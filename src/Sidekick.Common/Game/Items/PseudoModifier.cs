namespace Sidekick.Common.Game.Items
{
    public class PseudoModifier
    {
        public PseudoModifier(string text)
        {
            Text = text;
        }

        public string? Id { get; set; }

        public string Text { get; set; }

        public double Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether this modifier has value.
        /// </summary>
        public bool HasValue => Value != 0;
    }
}
