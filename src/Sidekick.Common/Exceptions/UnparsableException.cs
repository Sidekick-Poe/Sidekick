namespace Sidekick.Common.Exceptions;

public class UnparsableException : SidekickException
{
    public UnparsableException(string? itemText = null)
        : base("Sidekick was unable to parse this item.", "Make sure you have set your game language and league correctly in the settings.", "Sidekick is not perfect and can make mistakes, let us know on Discord or GitHub.")
    {
        ItemText = itemText;
    }

    public string? ItemText { get; set; }

    public override bool IsCritical => false;
}
