using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PseudoModifierFilter : PseudoModifier, ITradeFilter
    {
        public bool? @Checked { get; set; } = false;

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public double NormalizeValue { get; set; }

        /// <summary>
        /// Normalize the Min value with NormalizeValue.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (Value > 0)
            {
                Min = (int)Math.Max((1 - NormalizeValue) * Value, 0);
            }
            else
            {
                Min = (int)Math.Min((1 + NormalizeValue) * Value, 0);
            }
        }

        /// <summary>
        /// Normalize the Max value, +1 and/or NormalizeValue.
        /// </summary>
        public void NormalizeMaxValue()
        {
            if (Value > 0)
            {
                Max = (int)Math.Max(Math.Max(Value + 1, (1 + NormalizeValue) * Value), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(Value + 1, (1 - NormalizeValue) * Value), 0);
            }
        }

        /// <summary>
        /// Sets the filter to be the exact value.
        /// </summary>
        public void SetExactValue()
        {
            Min = (int)Value;
            Max = (int)Value;
        }
    }
}
