using Sidekick.Common.Exceptions;

namespace Sidekick.Common.Game.Languages;

public class GameLanguageAttribute
(
    string name,
    string languageCode
) : Attribute
{
    public string Name { get; private set; } = name;

    public string LanguageCode { get; private set; } = languageCode;

    public Type? ImplementationType { get; set; }

    public IGameLanguage Build()
    {
        if (ImplementationType == null) throw new SidekickException("The language implementation type could not be found.");

        return (IGameLanguage?)Activator.CreateInstance(ImplementationType) ?? throw new SidekickException("The language could not be instantiated.");
    }
}
