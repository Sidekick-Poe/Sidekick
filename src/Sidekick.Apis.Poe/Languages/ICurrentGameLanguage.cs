using Sidekick.Common.Initialization;
namespace Sidekick.Apis.Poe.Languages;

public interface ICurrentGameLanguage : IInitializableService
{
    IGameLanguage Language { get; }

    IGameLanguage InvariantLanguage { get; }

    bool IsEnglish();

    bool IsChinese();
}
