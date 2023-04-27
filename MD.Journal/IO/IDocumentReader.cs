using MD.Journal.Pagination;

namespace MD.Journal.IO
{
    public interface IDocumentReader
    {
        DocumentUri Uri { get; }

        Task<IEnumerable<string>> ReadAllLinesAsync();
        Task<ReadLinesResponse> ReadLinesAsync();
        Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken);

        Task<string> ReadTextAsync();
        Task<string> ReadTextAsync(int offset, int length);
    }
}

