namespace Sidekick.Game.Languages;

public interface IGameLanguageProvider
{
    IGameLanguage InvariantLanguage { get; }

    List<IGameLanguage> GetList();
}
