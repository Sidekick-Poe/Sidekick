#pragma warning disable CS0067

using Sidekick.Common.Platform;

namespace Sidekick.Mock;

public class MockProcessProvider : IProcessProvider
{
    public string ClientLogPath => string.Empty;

    public bool IsPathOfExileInFocus => true;
    public bool IsSidekickInFocus => false;
    public int Priority => 0;

    public Task Initialize()
    {
        return Task.CompletedTask;
    }
}
