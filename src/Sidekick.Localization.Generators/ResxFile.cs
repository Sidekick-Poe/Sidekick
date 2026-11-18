using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Sidekick.Localization.Generators;

/// <summary>
/// The string entries of a base (untranslated) .resx file.
/// </summary>
internal sealed record ResxFile(string Path, string ClassName, EquatableArray<ResxEntry> Entries, string? Error)
{
    private static readonly Regex CultureSuffix = new("^[a-z]{2,3}(-[A-Za-z0-9]+)*$");

    /// <summary>
    /// A base file is the one named after its resources class, like UiResources.resx. Translations have a culture suffix, like UiResources.fr.resx.
    /// </summary>
    public static bool IsBase(string path)
    {
        if (!path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var name = System.IO.Path.GetFileNameWithoutExtension(path);
        var lastDot = name.LastIndexOf('.');
        if (lastDot < 0)
        {
            return true;
        }

        return !CultureSuffix.IsMatch(name.Substring(lastDot + 1));
    }

    public static ResxFile Parse(string path, string? text)
    {
        var className = System.IO.Path.GetFileNameWithoutExtension(path);
        if (text == null)
        {
            return new ResxFile(path, className, EquatableArray<ResxEntry>.Empty, "the file could not be read");
        }

        XDocument document;
        try
        {
            document = XDocument.Parse(text, LoadOptions.SetLineInfo);
        }
        catch (XmlException e)
        {
            return new ResxFile(path, className, EquatableArray<ResxEntry>.Empty, e.Message);
        }

        var entries = new List<ResxEntry>();
        foreach (var data in document.Root?.Elements("data") ?? [])
        {
            // Entries with a type or mimetype are not strings (images, icons, serialized objects).
            if (data.Attribute("type") != null || data.Attribute("mimetype") != null)
            {
                continue;
            }

            var key = data.Attribute("name")?.Value;
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            var lineInfo = (IXmlLineInfo)data;
            entries.Add(new ResxEntry(key!, data.Element("value")?.Value ?? string.Empty, lineInfo.LineNumber, lineInfo.LinePosition));
        }

        return new ResxFile(path, className, new EquatableArray<ResxEntry>(entries.ToArray()), null);
    }
}

internal sealed record ResxEntry(string Key, string Value, int Line, int Column)
{
    /// <summary>
    /// The number of arguments the value expects, from its highest placeholder like {1}. Escaped braces ({{) are not placeholders.
    /// </summary>
    public int ArgumentCount
    {
        get
        {
            var highest = -1;
            for (var i = 0; i < Value.Length; i++)
            {
                if (Value[i] != '{')
                {
                    continue;
                }

                if (i + 1 < Value.Length && Value[i + 1] == '{')
                {
                    i++;
                    continue;
                }

                var end = i + 1;
                var index = 0;
                while (end < Value.Length && char.IsDigit(Value[end]))
                {
                    index = index * 10 + (Value[end] - '0');
                    end++;
                }

                if (end > i + 1 && end < Value.Length && Value[end] is '}' or ',' or ':')
                {
                    highest = Math.Max(highest, index);
                }
            }

            return highest + 1;
        }
    }
}
