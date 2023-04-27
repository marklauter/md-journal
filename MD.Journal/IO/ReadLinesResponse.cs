using MD.Journal.Pagination;

namespace MD.Journal.IO
{
    public record ReadLinesResponse(
        IEnumerable<string> Lines,
        PaginationToken PaginationToken)
    {
    }
}

