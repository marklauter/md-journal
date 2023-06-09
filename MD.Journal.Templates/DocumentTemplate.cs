using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MD.Journal.Templates;

public sealed partial class DocumentTemplate
{
    // http://regexstorm.net/tester

    [GeneratedRegex(@"[^\{{2}]+(?=\}{2})", RegexOptions.Singleline)]
    private static partial Regex FieldPattern();

    [GeneratedRegex(@"(?<key>\{{2}\w*\}{2})", RegexOptions.Singleline)]
    private static partial Regex SplitPattern();

    private readonly string[] fields;
    private readonly Dictionary<string, string> values;

    public DocumentTemplate(string content)
    {
        if (String.IsNullOrEmpty(content))
        {
            throw new ArgumentException($"'{nameof(content)}' cannot be null or empty.", nameof(content));
        }

        this.Content = content;
        this.fields = GetFields(content).ToArray();
        this.values = this.fields.ToDictionary(key => key);
    }

    private static IEnumerable<string> GetFields(string content)
    {
        var match = FieldPattern().Match(content);
        while (match.Captures.Count == 1)
        {
            yield return match.Value;
            match = match.NextMatch();
        }
    }

    public string Content { get; }

    public IEnumerable<string> Fields => this.fields;

    public int FieldCount => this.fields.Length;

    public IReadOnlyDictionary<string, string> Values => this.values;

    public void Write(string field, string value)
    {
        if (String.IsNullOrEmpty(field))
        {
            throw new ArgumentException($"'{nameof(field)}' cannot be null or empty.", nameof(field));
        }

        if (String.IsNullOrEmpty(value))
        {
            throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
        }

        if (!this.values.ContainsKey(field))
        {
            throw new KeyNotFoundException(field);
        }

        this.values[field] = value;
    }

    public string Read(string field)
    {
        return String.IsNullOrEmpty(field)
            ? throw new ArgumentException($"'{nameof(field)}' cannot be null or empty.", nameof(field))
            : !this.values.TryGetValue(field, out var value)
                ? throw new KeyNotFoundException(field)
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
        var lines = SplitPattern().Split(this.Content);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (IsField(line))
            {
                lines[i] = this.values[line[2..^2]];
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
