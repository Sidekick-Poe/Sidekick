using System.Globalization;
using Avalonia;
using Sidekick.Common.Localization;
using Sidekick.Common.Ui.Views;

namespace Sidekick.Avalonia.Services;

public class AvaloniaCultureHandler : IDisposable
{
    private readonly ILogger<AvaloniaCultureHandler> logger;
    private readonly IUiLanguageProvider uiLanguageProvider;

    public SidekickViewType LastOpenedType { get; private set; }

    public AvaloniaCultureHandler(ILogger<AvaloniaCultureHandler> logger, IUiLanguageProvider uiLanguageProvider)
    {
        this.logger = logger;
        this.uiLanguageProvider = uiLanguageProvider;
        this.uiLanguageProvider.OnLanguageChanged += SetCultureInfo;

        SetCultureInfo();
    }

    private Application GetApplication() => Application.Current ?? throw new Exception("Application is not initialized.");

    private async void SetCultureInfo(CultureInfo? cultureInfo = null)
    {
        try
        {
            cultureInfo ??= await uiLanguageProvider.Get();

            await GetApplication().Dispatcher.InvokeAsync(() =>
            {
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            });
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Error while trying to set culture info.");
        }
    }

    public void Dispose()
    {
        uiLanguageProvider.OnLanguageChanged -= SetCultureInfo;
    }
}
