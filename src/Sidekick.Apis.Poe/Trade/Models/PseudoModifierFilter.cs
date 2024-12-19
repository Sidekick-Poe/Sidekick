using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PseudoModifierFilter : ITradeFilter
    {
        public PseudoModifierFilter(PseudoModifier modifier)
        {
            Modifier = modifier;
            Checked = false;
        }

        public PseudoModifier Modifier { get; }

        public bool? @Checked { get; set; }

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public double NormalizeValue { get; set; }

        /// <summary>
        /// Normalize the Min value with NormalizeValue.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (Modifier.Value > 0)
            {
                Min = (int)Math.Max((1 - NormalizeValue) * Modifier.Value, 0);
            }
            else
            {
                Min = (int)Math.Min((1 + NormalizeValue) * Modifier.Value, 0);
            }
        }

        /// <summary>
        /// Normalize the Max value, +1 and/or NormalizeValue.
        /// </summary>
        public void NormalizeMaxValue()
        {
            if (Modifier.Value > 0)
            {
                Max = (int)Math.Max(Math.Max(Modifier.Value + 1, (1 + NormalizeValue) * Modifier.Value), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(Modifier.Value + 1, (1 - NormalizeValue) * Modifier.Value), 0);
            }
        }

        /// <summary>
        /// Sets the filter to be the exact value.
        /// </summary>
        public void SetExactValue()
        {
            Min = (int)Modifier.Value;
            Max = (int)Modifier.Value;
        }
    }
}
