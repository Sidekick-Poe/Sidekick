using Microsoft.Extensions.Localization;

namespace Sidekick.Modules.Trade.Localization
{
    public class TradeResources
    {
        private readonly IStringLocalizer<TradeResources> localizer;

        public TradeResources(IStringLocalizer<TradeResources> localizer)
        {
            this.localizer = localizer;
        }

        public string Age_Day => localizer["Age_Day"];
        public string Age_Days => localizer["Age_Days"];
        public string Age_Hour => localizer["Age_Hour"];
        public string Age_Hours => localizer["Age_Hours"];
        public string Age_Minute => localizer["Age_Minute"];
        public string Age_Minutes => localizer["Age_Minutes"];
        public string Age_Now => localizer["Age_Now"];
        public string Age_Seconds => localizer["Age_Seconds"];
        public string Class => localizer["Class"];
        public string ClassHint => localizer["Class_Hint"];
        public string Corrupted => localizer["Corrupted"];
        public string ForceCategory => localizer["ForceCategory"];

        public string CountString(int count, int total) => localizer["CountString", count, total];

        public string Filters_Max => localizer["Filters_Max"];
        public string Filters_Min => localizer["Filters_Min"];
        public string Filters_Equals => localizer["Filters_Equals"];
        public string Filters_Submit => localizer["Filters_Submit"];
        public string ItemLevel => localizer["ItemLevel"];
        public string Layout => localizer["Layout"];
        public string Layout_Cards_Maximized => localizer["Layout_Cards_Maximized"];
        public string Layout_Cards_Minimized => localizer["Layout_Cards_Minimized"];
        public string LoadMoreData => localizer["LoadMoreData"];
        public string ModifierHint => localizer["ModifierHint"];
        public string OpenQueryInWebsite => localizer["OpenQueryInWebsite"];
        public string Parsing => localizer["Parsing"];
        public string Prediction => localizer["Prediction"];

        public string PredictionConfidence(double confidence) => localizer["PredictionConfidence", confidence.ToString("0.##")];

        public string Requires => localizer["Requires"];
        public string Scourged => localizer["Scourged"];
        public string Trade => localizer["Trade"];
        public string Unidentified => localizer["Unidentified"];
        public string BasePercentile => localizer["Base_Percentile"];
        public string BuyoutPrice => localizer["BuyoutPrice"];
    }
}
