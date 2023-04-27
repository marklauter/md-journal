namespace MD.Journal.IO
{
    public interface IDocumentWriter
    {
        DocumentUri Uri { get; }

        Task AppendLineAsync(string value);
        Task AppendTextAsync(string value);
        Task OverwriteAllLinesAsync(IEnumerable<string> lines);
        Task OverwriteAllTextAsync(string value);
    }
}

