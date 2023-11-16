namespace Sidekick.Apis.Poe.Authentication.Models
{
    internal class Oauth2TokenResponse
    {
        public string? access_token { get; set; }
        public int expires_in { get; set; }
        public string? token_type { get; set; }
        public string? scope { get; set; }
        public string? username { get; set; }
        public string? sub { get; set; }
        public string? refresh_token { get; set; }
    }
}
