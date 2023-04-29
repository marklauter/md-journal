using MD.Journal.IO.Pagination;

namespace MD.Journal.IO.Readers
{
    public interface IResourceReader
    {
        Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri);
        Task<ReadResponse> ReadLinesAsync(ResourceUri uri);
        Task<ReadResponse> ReadLinesAsync(PaginationToken paginationToken);

        Task<string> ReadTextAsync(ResourceUri uri);
        Task<string> ReadTextAsync(ResourceUri uri, int offset, int length);
    }
}

