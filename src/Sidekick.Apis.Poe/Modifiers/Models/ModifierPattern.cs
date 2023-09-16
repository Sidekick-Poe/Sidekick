using System.Text.RegularExpressions;
using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Modifiers.Models
{
    public class ModifierPattern
    {
        public ModifierPattern(
            ModifierCategory category,
            string id,
            bool isOption,
            string text,
            string fuzzyText,
            Regex pattern)
        {
            Category = category;
            Id = id;
            IsOption = isOption;
            Text = text;
            FuzzyText = fuzzyText;
            Pattern = pattern;
        }

        public string Id { get; }

        public ModifierCategory Category { get; }

        public bool IsOption { get; }

        public string Text { get; }

        public string FuzzyText { get; set; }

        public string? OptionText { get; set; }

        public int LineCount => OptionText != null ? (OptionText?.Split('\n').Length ?? 1) : (Text?.Split('\n').Length ?? 1);

        public Regex Pattern { get; set; }

        public int? Value { get; set; }
    }
}
