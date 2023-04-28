namespace MD.Journal.IO.Readers
{
    public interface IResourceReader
    {
        Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri);
        Task<ReadLinesResponse> ReadLinesAsync(ResourceUri uri);
        Task<ReadLinesResponse> ReadLinesAsync(PaginationToken paginationToken);

        Task<string> ReadTextAsync(ResourceUri uri);
        Task<string> ReadTextAsync(ResourceUri uri, int offset, int length);
    }
}

