using System.Security.Cryptography;
using System.Text;

namespace Sidekick.Apis.Poe.Clients.Authentication;

/// <summary>
/// Proof Key for Code Exchange helper methods
/// </summary>
public class PkceHelper
{
    public record PkcePair(string Verifier, string Challenge);

    public static PkcePair GeneratePkcePair()
    {
        var verifier = GenerateCodeVerifier();
        var challenge = GenerateCodeChallenge(verifier);
        return new PkcePair(verifier, challenge);
    }

    private static string GenerateCodeVerifier()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);

        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
        return Convert.ToBase64String(challengeBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

}
