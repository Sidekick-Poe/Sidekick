using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class ModifierFilter : ITradeFilter
    {
        public ModifierFilter(ModifierLine line)
        {
            Line = line;
            Enabled = line.Modifiers.FirstOrDefault()?.Category == ModifierCategory.Fractured;
            NormalizeMinValue();
        }

        public ModifierLine Line { get; init; }

        public bool? Enabled { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        /// <summary>
        /// Normalizes the Min value between a -1 delta or 90%.
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
                Min = (int)Math.Max(Math.Min(value - 1, value * 0.9), 0);
            }
            else
            {
                Min = (int)Math.Min(Math.Min(value - 1, value * 1.1), 0);
            }
        }

        /// <summary>
        /// Normalizes the Max value between a +1 delta or 90%.
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
                Max = (int)Math.Max(Math.Max(value + 1, value * 1.1), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(value + 1, value * 0.9), 0);
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
