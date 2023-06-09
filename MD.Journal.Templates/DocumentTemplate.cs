using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MD.Journal.Templates;

public sealed partial class DocumentTemplate
{
    // http://regexstorm.net/tester

    [GeneratedRegex(@"[^\{{2}]+(?=\}{2})", RegexOptions.Singleline)]
    private static partial Regex FieldKeyPattern();

    [GeneratedRegex(@"(?<key>\{{2}\w*\}{2})", RegexOptions.Singleline)]
    private static partial Regex SplitOnFieldPattern();

    private readonly Dictionary<string, string> contentProperties;

    private readonly HashSet<string> systemProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "ShortLocalDate",
        "LongLocalDate",
        "ShortLocalTime",
        "LongLocalTime",
        "ShortLocalDateTime",
        "LongLocalDateTime",

        "ShortUtcDate",
        "LongUtcDate",
        "ShortUtcTime",
        "LongUtcime",
        "ShortUtcDateTime",
        "LongUtcDateTime",

        "Upn",
    };

    public DocumentTemplate(string content)
    {
        if (String.IsNullOrEmpty(content))
        {
            throw new ArgumentException($"'{nameof(content)}' cannot be null or empty.", nameof(content));
        }

        this.Content = content;
        this.contentProperties = GetFieldKeys(content)
            .ToDictionary(key => key);
    }

    public DocumentTemplate(IContentSource contentSource)
        : this(contentSource?.Content ?? throw new ArgumentNullException(nameof(contentSource)))
    {
    }

    public DocumentTemplate(DocumentRecord record)
        : this(record?.Content ?? throw new ArgumentNullException(nameof(record)))
    {
        foreach (var kvp in record.Properties)
        {
            if (this.contentProperties.ContainsKey(kvp.Key))
            {
                this.Write(kvp.Key, kvp.Value);
            }
        }
    }

    private static IEnumerable<string> GetFieldKeys(string content)
    {
        var match = FieldKeyPattern().Match(content);
        while (match.Captures.Count == 1)
        {
            yield return match.Value;
            match = match.NextMatch();
        }
    }

    public string Content { get; }

    public IReadOnlyDictionary<string, string> Properties => this.contentProperties;

    public void Write(string key, string value)
    {
        if (String.IsNullOrEmpty(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
        }

        if (String.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
        }

        if (!this.contentProperties.ContainsKey(key))
        {
            throw new KeyNotFoundException(key);
        }

        this.contentProperties[key] = value;
    }

    public string Read(string key)
    {
        return String.IsNullOrEmpty(key)
            ? throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key))
            : !this.contentProperties.TryGetValue(key, out var value)
                ? throw new KeyNotFoundException(key)
                : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsField(string line)
    {
        return line.StartsWith("{{", StringComparison.OrdinalIgnoreCase)
            && line.EndsWith("}}", StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsSystemProperty(string key)
    {
        return this.systemProperties.Contains(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ReadSystemProperty(string key)
    {
        return key switch
        {
            "ShortLocalDate" => DateTime.Now.ToShortDateString(),
            "LongLocalDate" => DateTime.Now.ToLongDateString(),
            "ShortLocalTime" => DateTime.Now.ToShortTimeString(),
            "LongLocalTime" => DateTime.Now.ToLongTimeString(),
            "ShortLocalDateTime" => DateTime.Now.ToString("g", CultureInfo.CurrentCulture),
            "LongLocalDateTime" => DateTime.Now.ToString("F", CultureInfo.CurrentCulture),
            "ShortUtcDate" => DateTime.UtcNow.ToShortDateString(),
            "LongUtcDate" => DateTime.UtcNow.ToLongDateString(),
            "ShortUtcTime" => DateTime.UtcNow.ToShortTimeString(),
            "LongUtcime" => DateTime.UtcNow.ToLongTimeString(),
            "ShortUtcDateTime" => DateTime.UtcNow.ToString("g", CultureInfo.CurrentCulture),
            "LongUtcDateTime" => DateTime.UtcNow.ToString("F", CultureInfo.CurrentCulture),
            "Upn" => OperatingSystem.IsWindows()
                ? System.Security.Principal.WindowsIdentity.GetCurrent().Name
                : "unknown",
            _ => String.Empty,
        };
    }

    public string Render()
    {
        var lines = SplitOnFieldPattern().Split(this.Content);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (IsField(line))
            {
                var key = line[2..^2];
                lines[i] = this.IsSystemProperty(key)
                    ? ReadSystemProperty(key)
                    : this.contentProperties[key];
            }
        }

        return String.Join(String.Empty, lines);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
    {
        return this.Render();
    }
}
