using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Languages;

public interface IGameLanguageProvider : IInitializableService
{
    IGameLanguage Language { get; }

    IGameLanguage InvariantLanguage { get; }

    List<GameLanguageAttribute> GetList();

    IGameLanguage GetLanguage(string? languageCode);

    bool IsEnglish();

    bool IsChinese();
}
