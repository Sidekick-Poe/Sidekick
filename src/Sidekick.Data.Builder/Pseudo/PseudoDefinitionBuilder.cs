using System.Text.RegularExpressions;

namespace Sidekick.Data.Builder.Pseudo;

public abstract class PseudoDefinitionBuilder
{
    public abstract string StatId { get; }

    public abstract List<PseudoPattern> Patterns { get; }

    /// <summary>
    /// Represents a regular expression pattern used to exclude certain stat texts
    /// during the processing of pseudo-stat definitions in the Path of Exile API.
    /// </summary>
    /// <remarks>
    /// This property defines a regular expression that matches stat texts
    /// which should be excluded from further processing. Each derived class provides
    /// a specific implementation of this property. It is utilized within the
    /// initialization and parsing processes to filter out unwanted stat entries.
    /// </remarks>
    public abstract Regex Exception { get; }
}
