namespace Sidekick.Apis.Poe.Languages;

public interface IGameLanguageProvider
{
    IGameLanguage InvariantLanguage { get; }

    List<IGameLanguage> GetList();
}
