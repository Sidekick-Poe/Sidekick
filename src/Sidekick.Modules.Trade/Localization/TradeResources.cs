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
        public string Corrupted => localizer["Corrupted"];
        public string Scourged => localizer["Scourged"];
        public string CountString(int count, int total) => localizer["CountString", count, total];
        public string Error_PoeApi => localizer["Error_PoeApi"];
        public string Filters_Expand => localizer["Filters_Expand"];
        public string Filters_Collapse => localizer["Filters_Collapse"];
        public string Filters_Dps => localizer["Filters_Dps"];
        public string Filters_EDps => localizer["Filters_EDps"];
        public string Filters_Max => localizer["Filters_Max"];
        public string Filters_Min => localizer["Filters_Min"];
        public string Filters_PDps => localizer["Filters_PDps"];
        public string Filters_Armour => localizer["Filters_Armour"];
        public string Filters_Map => localizer["Filters_Map"];
        public string Filters_Misc => localizer["Filters_Misc"];
        public string Filters_Weapon => localizer["Filters_Weapon"];
        public string Filters_Modifiers => localizer["Filters_Modifiers"];
        public string Filters_Pseudo => localizer["Filters_Pseudo"];
        public string Filters_Submit => localizer["Filters_Submit"];
        public string ItemLevel => localizer["ItemLevel"];
        public string Layout => localizer["Layout"];
        public string Layout_Cards_Maximized => localizer["Layout_Cards_Maximized"];
        public string Layout_Cards_Minimized => localizer["Layout_Cards_Minimized"];
        public string LoadMoreData => localizer["LoadMoreData"];
        public string MaxQualityArmour => localizer["MaxQualityArmour"];
        public string MaxQualityDps => localizer["MaxQualityDps"];
        public string MaxQualityEDps => localizer["MaxQualityEDps"];
        public string MaxQualityEnergyShield => localizer["MaxQualityEnergyShield"];
        public string MaxQualityEvasion => localizer["MaxQualityEvasion"];
        public string MaxQualityPDps => localizer["MaxQualityPDps"];
        public string OpenQueryInWebsite => localizer["OpenQueryInWebsite"];
        public string OverlayAccountName => localizer["OverlayAccountName"];
        public string OverlayAge => localizer["OverlayAge"];
        public string OverlayCharacter => localizer["OverlayCharacter"];
        public string OverlayItemLevel => localizer["OverlayItemLevel"];
        public string OverlayPrice => localizer["OverlayPrice"];
        public string PoeNinja => localizer["PoeNinja"];
        public string PoeNinjaLastUpdated => localizer["PoeNinjaLastUpdated"];
        public string Prediction => localizer["Prediction"];
        public string PredictionConfidence(double confidence) => localizer["PredictionConfidence", confidence.ToString("0.##")];
        public string Requires => localizer["Requires"];
        public string Settings => localizer["Settings"];
        public string Trade => localizer["Trade"];
        public string Unidentified => localizer["Unidentified"];
        public string UpdateNow => localizer["UpdateNow"];
        public string UpdateSeconds => localizer["UpdateSeconds"];
        public string UpdateShortly => localizer["UpdateShortly"];
        public string View_More => localizer["View_More"];
        public string View_Less => localizer["View_Less"];

    }
}
