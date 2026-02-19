using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;

namespace Sidekick.Apis.Poe.Languages;

public class GameLanguageProvider : IGameLanguageProvider
{
    private const string EnglishLanguageCode = "en";
    private IGameLanguage? invariantLanguage;

    public IGameLanguage InvariantLanguage
    {
        get
        {
            invariantLanguage ??= GetInvariantLanguage();
            return invariantLanguage;
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

    public IGameLanguage GetLanguage(string? languageCode)
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
}
