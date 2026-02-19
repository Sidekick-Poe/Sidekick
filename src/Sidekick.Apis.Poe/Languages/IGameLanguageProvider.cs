namespace Sidekick.Apis.Poe.Languages;

public interface IGameLanguageProvider
{
    IGameLanguage InvariantLanguage { get; }

    List<GameLanguageAttribute> GetList();

    IGameLanguage GetLanguage(string? languageCode);
}
