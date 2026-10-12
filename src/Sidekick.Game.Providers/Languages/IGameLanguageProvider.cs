namespace Sidekick.Game.Providers.Languages;

public interface IGameLanguageProvider
{
    IGameLanguage InvariantLanguage { get; }

    List<IGameLanguage> GetList();
}
