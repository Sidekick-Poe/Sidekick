using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Trade.Parser;

public static class RegexExtensions
{
    public static Regex ToRegexIntProperty(this string input) => new($@"^{Regex.Escape(input)}:[^\d]*([\d,\.]+)");

    public static Regex ToRegexDoubleProperty(this string input) => new($@"^{Regex.Escape(input)}:[^\d]*([\d,\.]+)");

    public static Regex ToRegexStringProperty(this string input) => new($"^{Regex.Escape(input)}: *(.+)$");

    public static Regex ToRegexIsAugmented(this string input) => new($"^{Regex.Escape(input)}.*\\)$");

    public static Regex ToRegexAffix(this string input, string superior)
    {
        if (input.StartsWith('/'))
        {
            input = input.Trim('/');
            return new($"^(?:{superior} )?{input}.*$|^.*{input}$");
        }

        input = Regex.Escape(input);
        return new($"^(?:{superior} )?{input}.*$|^.*{input}$");
    }

    public static Regex ToRegexEndOfLine(this string input) => new($"^.*{Regex.Escape(input)}$");

    public static Regex ToRegexLine(this string input) => new($"^{Regex.Escape(input)}$");

    public static Regex ToRegexHeistLevelCapture(this string input)
    {
        input = Regex.Escape(input);
        input = input.Replace(@"\#", @"([\d,\.]+)(?: \(unmet\))?");
        return new($@"^{input}$");
    }

}
