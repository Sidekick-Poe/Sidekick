using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;

namespace Sidekick.Apis.Poe.Languages;

public class CurrentGameLanguage(
    ISettingsService settingsService,
    IGameLanguageProvider gameLanguageProvider) : ICurrentGameLanguage
{
    private const string EnglishLanguageCode = "en";
    private IGameLanguage? language;

    public IGameLanguage Language => language ?? throw new SidekickException("The game language could not be found.");
    public IGameLanguage InvariantLanguage => gameLanguageProvider.InvariantLanguage;

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task Initialize()
    {
        var languageCode = await settingsService.GetString(SettingKeys.LanguageParser) ?? EnglishLanguageCode;
        language = gameLanguageProvider.GetLanguage(languageCode);
    }

    public bool IsEnglish()
    {
        return Language.GetType() == gameLanguageProvider.InvariantLanguage.GetType();
    }

    public bool IsChinese()
    {
        var languages = gameLanguageProvider.GetList();
        var chineseLanguage = languages.FirstOrDefault(x => x.LanguageCode == "zh");
        return chineseLanguage?.ImplementationType == Language.GetType();
    }
}
