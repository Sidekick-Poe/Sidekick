namespace Sidekick.Common.Initialization
{
    /// <summary>
    /// Determines the order of execution for the initialization services.
    /// </summary>
    public enum InitializationPriority
    {
        /// <summary>
        /// Represents a high priority, will run before medium and low priority services.
        /// </summary>
        High = 0,

        /// <summary>
        /// Represents a medium priority, will run after high priority services.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// Represents a low priority, will run after high and medium priority services.
        /// </summary>
        Low = 2,
    }
}
