using Microsoft.Extensions.Localization;

namespace Sidekick.Common.Blazor.Initialization;

public class InitializationResources(IStringLocalizer<InitializationResources> localizer)
{
    public string Close => localizer["Close"];
    public string Error => localizer["Error"];
    public string Exit => localizer["Exit"];
    public string Notification => localizer["Notification"];
    public string Ready => localizer["Ready"];

    public string Title(int completed, int count) => localizer["Title", completed, count];

}
