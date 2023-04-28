namespace MD.Journal.IO.Writers
{
    public interface IResourceWriter
    {
        Task AppendLineAsync(ResourceUri uri, string line);
        Task AppendTextAsync(ResourceUri uri, string text);
        Task OverwriteAllLinesAsync(ResourceUri uri, IEnumerable<string> lines);
        Task OverwriteAllTextAsync(ResourceUri uri, string text);
    }
}
