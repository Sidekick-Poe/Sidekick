namespace Sidekick.Apis.Poe.Trade.Requests;

public class Status
{
    public const string Online = "online";
    public const string Any = "any";

    public string Option { get; set; } = Online;
}
