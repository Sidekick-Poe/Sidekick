using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class ModifierPattern
    {
        public ModifierPattern(
            string text,
            string fuzzyText,
            Regex pattern)
        {
            Text = text;
            FuzzyText = fuzzyText;
            Pattern = pattern;
        }

        public string Text { get; }

        public string FuzzyText { get; set; }

        public string? OptionText { get; set; }

        public int LineCount => OptionText != null ? (OptionText?.Split('\n').Length ?? 1) : (Text?.Split('\n').Length ?? 1);

        public Regex Pattern { get; set; }

        public int? Value { get; set; }
    }
}
