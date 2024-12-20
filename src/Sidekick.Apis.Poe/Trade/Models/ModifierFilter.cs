using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class ModifierFilter : ITradeFilter
    {
        public ModifierFilter(ModifierLine line)
        {
            Line = line;
            Checked = line.Modifiers.FirstOrDefault()?.Category == ModifierCategory.Fractured;

            if (HasMoreThanOneCategory && FirstCategory == ModifierCategory.Fractured)
            {
                ForceFirstCategory = true;
            }
        }

        public ModifierLine Line { get; }

        public bool? @Checked { get; set; }

        public bool ForceFirstCategory { get; set; }

        public ModifierCategory FirstCategory => Line.Modifiers.FirstOrDefault()?.Category ?? ModifierCategory.Undefined;

        public bool HasMoreThanOneCategory => Line.Modifiers.GroupBy(x => x.Category).Count() > 1;

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public double NormalizeValue { get; set; }

        /// <summary>
        /// Normalize the Min value with NormalizeValue.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (Line.OptionValue != null || !Line.Values.Any())
            {
                return;
            }

            var value = Line.Values.OrderBy(x => x).FirstOrDefault();
            if (value > 0)
            {
                Min = (int)Math.Max((1 - NormalizeValue) * value, 0);
            }
            else
            {
                Min = (int)Math.Min((1 + NormalizeValue) * value, 0);
            }
        }

        /// <summary>
        /// Normalize the Max value, +1 and/or NormalizeValue.
        /// </summary>
        public void NormalizeMaxValue()
        {
            if (Line.OptionValue != null || !Line.Values.Any())
            {
                return;
            }

            var value = Line.Values.OrderBy(x => x).FirstOrDefault();
            if (value > 0)
            {
                Max = (int)Math.Max(Math.Max(value + 1, (1 + NormalizeValue) * value), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(value + 1, (1 - NormalizeValue) * value), 0);
            }
        }

        /// <summary>
        /// Sets the filter to be the exact value.
        /// </summary>
        public void SetExactValue()
        {
            if (Line.OptionValue != null || !Line.Values.Any())
            {
                return;
            }

            var value = Line.Values.OrderBy(x => x).FirstOrDefault();
            Min = (int)value;
            Max = (int)value;
        }
    }
}
