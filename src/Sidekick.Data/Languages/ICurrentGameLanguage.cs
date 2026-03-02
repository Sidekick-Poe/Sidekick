using Sidekick.Common.Initialization;
namespace Sidekick.Data.Languages;

public interface ICurrentGameLanguage : IInitializableService
{
    IGameLanguage Language { get; }

    IGameLanguage InvariantLanguage { get; }

    bool IsEnglish();

    bool IsChinese();
}
