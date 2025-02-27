using NuGet.Versioning;
using Velopack;
using Velopack.Locators;

namespace Sidekick.Common.Updater;

public class DebugLocator : IVelopackLocator
{
    public List<VelopackAsset> GetLocalPackages() => [];

    public VelopackAsset? GetLatestLocalFullPackage() => null;

    public Guid? GetOrCreateStagedUserId() => null;

    public string AppId => "Sidekick";

    public string? RootAppDir => null;

    public string? PackagesDir => null;

    public string? AppContentDir => null;

    public string? AppTempDir => null;

    public string? UpdateExePath => null;

    public SemanticVersion CurrentlyInstalledVersion => new(1, 0, 0);

    public string? ThisExeRelativePath => null;

    public string? Channel => "windows-stable";

    public bool IsPortable => false;
}
