using Sidekick.Common.Initialization;

namespace Sidekick.Common.Keybinds;

public interface IKeybindHandler : IInitializableService
{
    List<string?> Keybinds { get; }
    bool IsValid(string keybind);
    Task Execute(string keybind);
}
