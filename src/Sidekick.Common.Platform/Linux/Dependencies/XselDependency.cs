using Microsoft.Extensions.Localization;
using Sidekick.Common.Platform.Localization;

namespace Sidekick.Common.Platform.Linux.Dependencies;

/// <summary>
/// Reads and writes the clipboard, through the TextCopy library. Sidekick copies an item by pressing Ctrl+C in the
/// game, then reads the item text from the clipboard. Required in every session: wl-clipboard, used instead on KDE
/// Wayland when installed, is optional, so a missing wl-clipboard never blocks Sidekick from starting.
/// </summary>
internal sealed class XselDependency : IDependency
{
	public Func<IStringLocalizer<PlatformResources>, string> Description => resources => resources.Linux_Dependencies_Xsel;
	public bool IsAvailable() => LinuxCommand.IsAvailable("xsel");
	public string ArchPackage => "xsel";
	public string DebianPackage => "xsel";
	public string FedoraPackage => "xsel";
}
