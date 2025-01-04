using Sidekick.Common.Game.Items;

namespace Sidekick.Apis.Poe.Trade.Models
{
    public class PseudoModifierFilter : ITradeFilter
    {
        public required PseudoModifier PseudoModifier { get; set; }

        public bool? @Checked { get; set; } = false;

        public double? Min { get; set; }

        public double? Max { get; set; }

        public double NormalizeValue { get; set; }

        /// <summary>
        /// Normalize the Min value with NormalizeValue.
        /// </summary>
        public void NormalizeMinValue()
        {
            if (PseudoModifier.Value > 0)
            {
                Min = (int)Math.Max((1 - NormalizeValue) * PseudoModifier.Value, 0);
            }
            else
            {
                Min = (int)Math.Min((1 + NormalizeValue) * PseudoModifier.Value, 0);
            }
        }

        /// <summary>
        /// Normalize the Max value, +1 and/or NormalizeValue.
        /// </summary>
        public void NormalizeMaxValue()
        {
            if (PseudoModifier.Value > 0)
            {
                Max = (int)Math.Max(Math.Max(PseudoModifier.Value + 1, (1 + NormalizeValue) * PseudoModifier.Value), 0);
            }
            else
            {
                Max = (int)Math.Min(Math.Max(PseudoModifier.Value + 1, (1 - NormalizeValue) * PseudoModifier.Value), 0);
            }
        }

        /// <summary>
        /// Sets the filter to be the exact value.
        /// </summary>
        public void SetExactValue()
        {
            Min = (int)PseudoModifier.Value;
            Max = (int)PseudoModifier.Value;
        }
    }
}
