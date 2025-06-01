namespace Sidekick.Apis.Poe.Account.Authentication;

public static class AuthenticationConfig
{
    public const string AuthorizationUrl = "https://www.pathofexile.com/oauth/authorize";
    public const string RedirectUrl = "https://sidekick-poe.github.io/oauth/poe";
    public const string ClientId = "sidekick";
    public const string Scopes = "account:stashes";
    public const string TokenUrl = "https://www.pathofexile.com/oauth/token";
}
