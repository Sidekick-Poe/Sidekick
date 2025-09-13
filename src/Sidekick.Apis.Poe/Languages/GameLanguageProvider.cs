using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;
using Sidekick.Common.Settings;
namespace Sidekick.Apis.Poe.Languages;

public class GameLanguageProvider(ISettingsService settingsService) : IGameLanguageProvider
{
    private const string EnglishLanguageCode = "en";
    private IGameLanguage? language;
    private IGameLanguage? invariantLanguage;

    public IGameLanguage Language => language ?? throw new SidekickException("The game language could not be found.");

    public IGameLanguage InvariantLanguage => invariantLanguage ?? throw new SidekickException("The English language could not be found.");

    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public async Task Initialize()
    {
        var languageCode = await settingsService.GetString(SettingKeys.LanguageParser) ?? EnglishLanguageCode;
        language = GetLanguage(languageCode);
        invariantLanguage = GetInvariantLanguage();

        // If the language is Chinese, we are forcing the use invariant trade results flag.
        var useInvariantTradeResults = await settingsService.GetBool(SettingKeys.UseInvariantTradeResults);
        if (languageCode == "zh" && !useInvariantTradeResults)
        {
            await settingsService.Set(SettingKeys.UseInvariantTradeResults, true);
        }
    }

    public List<GameLanguageAttribute> GetList()
    {
        var result = new List<GameLanguageAttribute>();

        foreach (var type in typeof(GameLanguageAttribute).GetTypesImplementingAttribute())
        {
            var attribute = type.GetAttribute<GameLanguageAttribute>();
            if (attribute == null)
            {
                continue;
            }

            attribute.ImplementationType = type;
            result.Add(attribute);
        }

        return result;
    }

    private IGameLanguage GetLanguage(string? languageCode)
    {
        languageCode ??= EnglishLanguageCode;

        var languages = GetList();
        var implementationType = languages.FirstOrDefault(x => x.LanguageCode == languageCode)?.ImplementationType;
        implementationType ??= GetInvariantLanguage().GetType();

        return (IGameLanguage?)Activator.CreateInstance(implementationType) ?? throw new SidekickException("The game language could not be found.");
    }

    private IGameLanguage GetInvariantLanguage()
    {
        var languages = GetList();
        var implementationType = languages.FirstOrDefault(x => x.LanguageCode == EnglishLanguageCode)?.ImplementationType ?? throw new SidekickException("The English language could not be found.");

        return (IGameLanguage?)Activator.CreateInstance(implementationType) ?? throw new SidekickException("The English language could not be found.");
    }

    public bool IsEnglish()
    {
        return Language.GetType() == InvariantLanguage.GetType();
    }

    public bool IsChinese()
    {
        var languages = GetList();
        var chineseLanguage = languages.FirstOrDefault(x => x.LanguageCode == "zh");
        return chineseLanguage?.ImplementationType == Language.GetType();
    }
}
