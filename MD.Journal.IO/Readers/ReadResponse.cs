using MD.Journal.IO.Pagination;

namespace MD.Journal.IO.Readers
{
    public readonly record struct ReadResponse(
        string[] Lines,
        PaginationToken PaginationToken)
    {
    }
}

