using System.Globalization;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Localization;

/// <summary>
///     Implementation of the ui language provider.
/// </summary>
public class UiLanguageProvider(ISettingsService settingsService) : IUiLanguageProvider
{
    private static readonly string[] supportedLanguages =
    [
        "en",
        "fr",
        "ko",
    ];

    private string? currentLanguage;

    /// <inheritdoc />
    public event Action<CultureInfo>? OnLanguageChanged;

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task Initialize()
    {
        var language = await settingsService.GetString(SettingKeys.LanguageUi);
        Set(language ?? "en");
    }

    /// <inheritdoc />
    public List<CultureInfo> GetList()
    {
        var languages = supportedLanguages
                        .Select(CultureInfo.GetCultureInfo)
                        .ToList();
        return languages;
    }

    /// <inheritdoc />
    public async Task<CultureInfo> Get()
    {
        var language = await settingsService.GetString(SettingKeys.LanguageUi);
        var cultureInfo = CultureInfo.GetCultureInfo(language ?? "en");

        return cultureInfo;
    }

    /// <inheritdoc />
    public void Set(string? name)
    {
        name ??= "en";
        if (currentLanguage == name)
        {
            return;
        }

        var languages = GetList();
        var language = name;
        if (languages.All(x => x.Name != language))
        {
            language = languages.FirstOrDefault()
                                ?.Name;
        }

        if (string.IsNullOrEmpty(language))
        {
            return;
        }

        currentLanguage = name;
        var cultureInfo = CultureInfo.GetCultureInfo(language);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        OnLanguageChanged?.Invoke(cultureInfo);
    }
}
