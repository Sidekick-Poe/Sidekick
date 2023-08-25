namespace Sidekick.Common.Platform
{
    /// <summary>
    /// Interface containing platform specific methods.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Shutdown the application.
        /// </summary>
        void Shutdown();
    }
}
