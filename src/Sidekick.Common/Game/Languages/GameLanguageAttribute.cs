namespace Sidekick.Common.Game.Languages;

public class GameLanguageAttribute(
    string name,
    string languageCode) : Attribute
{
    public string Name { get; private set; } = name;

    public string LanguageCode { get; private set; } = languageCode;

    public Type? ImplementationType { get; set; }
}
