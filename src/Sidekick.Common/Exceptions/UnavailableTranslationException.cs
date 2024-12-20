namespace Sidekick.Common.Exceptions;

public class UnavailableTranslationException : SidekickException
{
    public UnavailableTranslationException()
        : base("This feature is only available when the game is running in english.")
    {
    }

    public UnavailableTranslationException(string? additionalInformation)
        : base("This feature is only available when the game is running in english.", additionalInformation ?? string.Empty)
    {
    }
}
