namespace MD.Journal.IO
{
    public interface IResourceWriter
    {
        ResourceUri Uri { get; }

        Task AppendLineAsync(string value);
        Task AppendTextAsync(string value);
        Task OverwriteAllLinesAsync(IEnumerable<string> lines);
        Task OverwriteAllTextAsync(string value);
    }
}

