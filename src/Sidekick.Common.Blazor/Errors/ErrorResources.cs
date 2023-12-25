using Microsoft.Extensions.Localization;
using Sidekick.Common.Errors;

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
        public string AlreadyRunning => localizer["AlreadyRunning"];
        public string Title => localizer["Title"];
        public string Close => localizer["Close"];
        public string Error => localizer["Error"];

        public string GetText(string? type) => GetText(Enum.Parse<ErrorType>(type ?? ErrorType.Unknown.ToString()));

        public string GetText(ErrorType type) => type switch
        {
            ErrorType.InvalidItem => InvalidItemError,
            ErrorType.UnavailableTranslation => AvailableInEnglishError,
            ErrorType.Unparsable => ParserError,
            ErrorType.ApiError => ApiError,
            ErrorType.AlreadyRunning => AlreadyRunning,
            _ => Error,
        };
    }
}
