namespace MD.Journal.IO.Readers
{
    public readonly record struct ReadLinesResponse(
        IEnumerable<string> Lines,
        PaginationToken PaginationToken)
    {
    }
}

