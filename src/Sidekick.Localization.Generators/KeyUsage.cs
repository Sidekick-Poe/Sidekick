using System.Collections.Concurrent;

namespace Sidekick.Localization.Generators;

/// <summary>
/// The keys a compilation reads, per resources class, so the unused ones can be reported once the whole project is analyzed.
/// </summary>
internal sealed class KeyUsage
{
    private readonly ConcurrentDictionary<(string ClassName, string Key), byte> used = new();

    private readonly ConcurrentDictionary<string, byte> unverifiable = new();

    public void Use(string className, string key) => used.TryAdd((className, key), 0);

    /// <summary>
    /// A class read with a key computed at runtime can't have its unused keys reported, any of them might be the one computed.
    /// </summary>
    public void MarkUnverifiable(string className) => unverifiable.TryAdd(className, 0);

    public bool IsUsed(string className, string key) => used.ContainsKey((className, key));

    public bool IsUnverifiable(string className) => unverifiable.ContainsKey(className);
}
