using Sidekick.Common.Initialization;
using Sidekick.Game.Languages;
namespace Sidekick.Common.Settings.Languages;

public interface ICurrentGameLanguage : IInitializableService
{
    IGameLanguage Language { get; }

    IGameLanguage InvariantLanguage { get; }

    bool IsEnglish();

    bool IsChinese();
}
