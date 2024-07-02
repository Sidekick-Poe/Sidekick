using Sidekick.Apis.Poe.Trade.Models;

namespace Sidekick.Modules.Trade.Localization
{
    public class TradeCurrencyOption
    {
        public TradeCurrency Value { get; init; }

        public required string Label { get; init; }

        public string? Image { get; init; }
    }
}
