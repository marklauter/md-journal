namespace MD.Journal.IO.Readers
{
    public readonly record struct ReadLinesResponse(
        string[] Lines,
        PaginationToken PaginationToken)
    {
    }
}

