namespace Sidekick.Common.Exceptions
{
    public class InvalidItemException : SidekickException
    {
        public InvalidItemException() :
            base("This item is invalid for this feature.")
        {
        }

        public InvalidItemException(string? additionalInformation) :
            base("This item is invalid for this feature.", additionalInformation)
        {
        }
    }
}
