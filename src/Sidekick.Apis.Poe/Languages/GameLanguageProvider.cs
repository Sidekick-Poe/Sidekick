using Sidekick.Common.Exceptions;
using Sidekick.Common.Extensions;

namespace Sidekick.Apis.Poe.Languages;

public class GameLanguageProvider : IGameLanguageProvider
{
    private const string EnglishLanguageCode = "en";
    private IGameLanguage? invariantLanguage;

    private List<IGameLanguage>? List { get; set; }

    public IGameLanguage InvariantLanguage
    {
        get
        {
            invariantLanguage ??= GetList().FirstOrDefault(x => x.Code == EnglishLanguageCode)
                                  ?? throw new SidekickException("The invariant language could not be found.");;
            return invariantLanguage;
        }
    }

    public List<IGameLanguage> GetList()
    {
        if (List != null) return List;

        var result = new List<IGameLanguage>();
        foreach (var type in typeof(IGameLanguage).GetTypesImplementingInterface())
        {
            var language = (IGameLanguage?)Activator.CreateInstance(type) ?? throw new SidekickException("The game language could not be found.");;
            result.Add(language);
        }

        List = result;
        return result;
    }
}
