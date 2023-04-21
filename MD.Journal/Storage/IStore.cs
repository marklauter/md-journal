namespace MD.Journal.Storage
{
    public interface IStore
    {
        string Path { get; }
        string ResourceName { get; }
        string Uri { get; }

        Task AppendLineAsync(string value);
        Task AppendTextAsync(string value);
        Task OverwriteAllLinesAsync(IEnumerable<string> lines);
        Task OverwriteAllTextAsync(string value);
        Task<IEnumerable<string>> ReadAllLinesAsync();
        Task<IEnumerable<string>> ReadLinesAsync(Pagination pagination);
        Task<string> ReadTextAsync();
    }
}

