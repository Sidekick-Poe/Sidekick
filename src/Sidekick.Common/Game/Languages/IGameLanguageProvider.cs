using Sidekick.Common.Initialization;

namespace Sidekick.Common.Game.Languages;

public interface IGameLanguageProvider : IInitializableService
{
    IGameLanguage Language { get; }

    IGameLanguage InvariantLanguage { get; }

    List<GameLanguageAttribute> GetList();

    bool IsEnglish();
}
