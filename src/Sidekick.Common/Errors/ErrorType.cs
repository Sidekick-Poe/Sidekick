namespace Sidekick.Common.Errors
{
    /// <summary>
    /// The type of the error.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Translation is unavailable.
        /// </summary>
        UnavailableTranslation,

        /// <summary>
        /// An item is invalid.
        /// </summary>
        InvalidItem,

        /// <summary>
        /// An item could not be parsed.
        /// </summary>
        Unparsable,

        /// <summary>
        /// The API returned an error.
        /// </summary>
        ApiError,

        /// <summary>
        /// Unknown error occured, look in the logs for more information.
        /// </summary>
        Unknown,

        /// <summary>
        /// Indicates the application is already running.
        /// </summary>
        AlreadyRunning,
    }
}
