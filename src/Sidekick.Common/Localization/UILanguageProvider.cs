using System.Globalization;
using Sidekick.Common.Initialization;
using Sidekick.Common.Settings;

namespace Sidekick.Common.Localization;

/// <summary>
///     Implementation of the ui language provider.
/// </summary>
public class UILanguageProvider(ISettings settings) : IUILanguageProvider
{
    private static readonly string[] supportedLanguages =
    {
        "en",
        "fr",
        "de",
        "zh-tw",
    };

    /// <inheritdoc />
    public InitializationPriority Priority => InitializationPriority.Critical;

    /// <inheritdoc />
    public Task Initialize()
    {
        Set(settings.Language_UI);
        return Task.CompletedTask;
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
    public void Set(string name)
    {
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

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo(language);
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(language);
    }
}
