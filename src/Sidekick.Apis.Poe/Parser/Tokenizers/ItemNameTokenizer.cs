using System.Text;
using System.Text.RegularExpressions;

namespace Sidekick.Apis.Poe.Parser.Tokenizers
{
    public class ItemNameTokenizer
    {
        private readonly Dictionary<ItemNameTokenType, Regex> _tokenDefs;

        public ItemNameTokenizer()
        {
            _tokenDefs = new Dictionary<ItemNameTokenType, Regex>
            {
                { ItemNameTokenType.Set, new Regex("<<set:(?<LANG>\\w{1,2})>>") },
                { ItemNameTokenType.Name, new Regex("^((?!<).)+") },
                { ItemNameTokenType.If, new Regex("<(?:el)?if:(?<LANG>\\w{1,2})>{(?<NAME>\\s?((?!<).)+)}") },
                { ItemNameTokenType.MiscTags, new Regex("^<.*>") }
            };
        }

        private IEnumerable<ItemNameToken> GetTokens(string input)
        {
            var tokens = new List<ItemNameToken>();

            while (!string.IsNullOrWhiteSpace(input))
            {
                var match = FindMatch(ref input);
                if (match != null)
                {
                    tokens.Add(new ItemNameToken(match.TokenType, match));
                }
                else
                {
                    if (Regex.IsMatch(input, "^\\s+"))
                    {
                        input = input.TrimStart();
                    }
                    else
                    {
                        throw new KeyNotFoundException("Failed to parse item name");
                    }
                }
            }

            tokens.Add(new ItemNameToken(ItemNameTokenType.EndOfItem, null));

            return tokens;
        }

        private ItemNameTokenMatch? FindMatch(ref string input)
        {
            foreach (var def in _tokenDefs)
            {
                var match = def.Value.Match(input);
                if (match.Success)
                {
                    if (match.Length != input.Length)
                    {
                        input = input[match.Length..];
                    }
                    else
                    {
                        input = string.Empty;
                    }

                    return new ItemNameTokenMatch(
                        tokenType: def.Key,
                        match: match);
                }
            }

            return null;
        }

        public string CleanString(string input)
        {
            var langs = new List<string>();
            var tokens = GetTokens(input);
            var output = new StringBuilder();

            foreach (var token in tokens)
            {
                if (token.Match == null)
                {
                    continue;
                }

                if (token.TokenType == ItemNameTokenType.Set)
                {
                    langs.Add(token.Match.Match.Groups["LANG"].Value);
                }
                else if (token.TokenType == ItemNameTokenType.Name)
                {
                    output.Append($"{token.Match.Match.Value}\n");
                }
                else if (token.TokenType == ItemNameTokenType.If)
                {
                    var lang = token.Match.Match.Groups["LANG"].Value;
                    if (langs.Contains(lang))
                    {
                        output.Append(token.Match.Match.Groups["NAME"].Value);
                    }
                }
            }

            return output.ToString();
        }
    }
}
