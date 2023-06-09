namespace MD.Journal.Templates;

public sealed record DocumentRecord(string Content, IReadOnlyDictionary<string, string> Properties)
{
    public DocumentRecord(DocumentTemplate template)
        : this(template?.Content ?? throw new ArgumentNullException(nameof(template)),
              template?.Properties ?? throw new ArgumentNullException(nameof(template)))
    {
    }
}
