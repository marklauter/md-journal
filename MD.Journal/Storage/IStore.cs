namespace MD.Journal.Storage
{
    public interface IStore
    {
        string Path { get; }
        string Name { get; }
        string Uri { get; }

        Task OverWriteAllLinesAsync(IEnumerable<string> lines);
        Task OverWriteAllTextAsync(string value);
        Task<IEnumerable<string>> ReadAllLinesAsync();
        Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination);
        Task<string> ReadTextAsync();
        Task WriteLineAsync(string value);
        Task WriteTextAsync(string value);
    }
}

