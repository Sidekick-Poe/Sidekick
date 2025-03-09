using System.Globalization;

namespace Sidekick.Common.Localization;

/// <summary>
///     Implementation of the ui language provider.
/// </summary>
public class UiLanguageProvider : IUiLanguageProvider
{
    private static readonly string[] supportedLanguages =
    [
        "en",
        "fr",
        "ko",
    ];

    private string? currentLanguage;


    /// <inheritdoc />
    public List<CultureInfo> GetList()
    {
        var languages = supportedLanguages
                        .Select(CultureInfo.GetCultureInfo)
                        .ToList();
        return languages;
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
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(language);
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(language);
    }
}
