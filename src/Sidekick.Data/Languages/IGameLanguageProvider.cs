namespace Sidekick.Data.Languages;

public interface IGameLanguageProvider
{
    IGameLanguage InvariantLanguage { get; }

    List<IGameLanguage> GetList();
}
