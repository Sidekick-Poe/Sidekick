using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Errors
{
    public class ErrorResources
    {
        private readonly IStringLocalizer<ErrorResources> localizer;

        public ErrorResources(IStringLocalizer<ErrorResources> localizer)
        {
            this.localizer = localizer;
        }

        public string AdditionalInformation => localizer["AdditionalInformation"];
        public string ApiError => localizer["ApiError"];
        public string AvailableInEnglishError => localizer["AvailableInEnglishError"];
        public string InvalidItemError => localizer["InvalidItemError"];
        public string ParserError => localizer["ParserError"];
        public string UnknownError => localizer["UnknownError"];
        public string Title => localizer["Title"];
        public string Close => localizer["Close"];
        public string Error => localizer["Error"];
    }
}
