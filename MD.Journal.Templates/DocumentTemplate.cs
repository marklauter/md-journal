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

    private readonly Dictionary<string, string> properties;

    public DocumentTemplate(string content)
    {
        if (String.IsNullOrEmpty(content))
        {
            throw new ArgumentException($"'{nameof(content)}' cannot be null or empty.", nameof(content));
        }

        this.Content = content;
        this.properties = GetFieldKeys(content)
            .ToDictionary(key => key);
    }

    public DocumentTemplate(DocumentRecord record)
        : this(record?.Content ?? throw new ArgumentNullException(nameof(record)))
    {
        foreach (var kvp in record.Properties)
        {
            if (this.properties.ContainsKey(kvp.Key))
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

    public IReadOnlyDictionary<string, string> Properties => this.properties;

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

        if (!this.properties.ContainsKey(key))
        {
            throw new KeyNotFoundException(key);
        }

        this.properties[key] = value;
    }

    public string Read(string key)
    {
        return String.IsNullOrEmpty(key)
            ? throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key))
            : !this.properties.TryGetValue(key, out var value)
                ? throw new KeyNotFoundException(key)
                : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsField(string line)
    {
        return line.StartsWith("{{", StringComparison.OrdinalIgnoreCase)
            && line.EndsWith("}}", StringComparison.OrdinalIgnoreCase);
    }

    public string Render()
    {
        var lines = SplitOnFieldPattern().Split(this.Content);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (IsField(line))
            {
                lines[i] = this.properties[line[2..^2]];
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
