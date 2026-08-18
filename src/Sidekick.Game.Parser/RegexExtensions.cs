using System.Text.RegularExpressions;
namespace Sidekick.Game.Parser;

public static class RegexExtensions
{
    public static Regex ToRegexIntProperty(this string input) => new($@"^{Regex.Escape(input)}:\s*\+?([\d,\.]+)");

    public static Regex ToRegexDoubleProperty(this string input) => new($@"^{Regex.Escape(input)}:\s*\+?([\d,\.]+)");

    public static Regex ToRegexStringProperty(this string input) => new($@"^{Regex.Escape(input)}:\s*(.+)$");

    public static Regex ToRegexIsAugmented(this string input) => new($@"^{Regex.Escape(input)}.*\)$");

    public static Regex ToRegexAffix(this string input)
    {
        input = Regex.Escape(input);
        input = input.Replace(@"\#", @"([a-zA-Z\s]+)?");
        return new(input);
    }

    public static Regex ToRegexEndOfLine(this string input) => new($"^.*{Regex.Escape(input)}$");

    public static Regex ToRegexLine(this string input) => new($"^{Regex.Escape(input)}$");

    public static Regex ToRegexHeistLevelCapture(this string input)
    {
        input = Regex.Escape(input);
        input = input.Replace(@"\#", @"([\d,\.]+)(?: \(unmet\))?");
        return new($@"^{input}$");
    }

    public static string CleanWildcard(this string input)
    {
        return input.Replace("#", string.Empty).Trim();
    }

}
