namespace Sidekick.Common.Exceptions
{
    public class AlreadyRunningException : SidekickException
    {
        public AlreadyRunningException() :
            base("The application is already running. Only one instance can run at once.")
        {
        }

        public AlreadyRunningException(string? additionalInformation) :
            base("The application is already running. Only one instance can run at once.", additionalInformation)
        {
        }
    }
}
