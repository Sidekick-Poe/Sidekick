namespace Sidekick.Apis.Poe.Trade.Trade.Requests;

public class Status
{
    public const string Securable = "securable";
    public const string Available = "available";
    public const string Online = "online";
    public const string Any = "any";

    public const string OnlineLeague = "onlineleague";

    public string Option { get; set; } = Online;
}
