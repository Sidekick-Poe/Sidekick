using Sidekick.Common.Exceptions;
using Sidekick.Common.Settings;
namespace Sidekick.Data.Languages;

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
        language = gameLanguageProvider.GetList()
            .FirstOrDefault(x => x.Code == languageCode);
    }

    public bool IsEnglish() => Language.Code == "en";

    public bool IsChinese() => Language.Code == "zh";
}
