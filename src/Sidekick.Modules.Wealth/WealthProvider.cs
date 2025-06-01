using Microsoft.Extensions.Logging;

namespace Sidekick.Modules.Wealth;

internal class WealthProvider(
    ILogger<WealthProvider> logger)
{
    public event Action? OnStashUpdated;

    public void StashUpdated()
    {
        OnStashUpdated?.Invoke();
    }
}
