using Sidekick.Common.Initialization;

namespace Sidekick.Common.Game.Languages;

public interface IGameLanguageProvider : IInitializableService
{
    IGameLanguage? Language { get; }

    void SetLanguage(string? languageCode);

    List<GameLanguageAttribute> GetList();

    IGameLanguage? Get(string code);

    bool IsEnglish();
}
