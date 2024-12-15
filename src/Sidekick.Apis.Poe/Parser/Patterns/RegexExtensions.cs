using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Parser.Patterns;

public static class RegexExtensions
{
    public static Regex ToRegexIntCapture(this string input) => new($"^{Regex.Escape(input)}[^\\d]*(\\d+)");

    public static Regex ToRegexDecimalCapture(this string input) => new($"^{Regex.Escape(input)}[^\\d]*([\\d,\\.]+)");

    public static Regex ToRegexAffix(this string input)
    {
        if (input.StartsWith('/'))
        {
            input = input.Trim('/');
            return new($"^{input}.*$|^.*{input}$");
        }

        input = Regex.Escape(input);
        return new($"^{input}.*$|^.*{input}$");
    }

    public static Regex ToRegexStartOfLine(this string input) => new($"^{Regex.Escape(input)}.*$");

    public static Regex ToRegexEndOfLine(this string input) => new($"^.*{Regex.Escape(input)}$");

    public static Regex ToRegexLine(this string input) => new($"^{Regex.Escape(input)}$");

}
