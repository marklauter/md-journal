namespace MD.Journal.Pagination
{
    public readonly struct PaginationToken
    {
        internal static PaginationToken Bof { get; } = new PaginationToken(0, false);
        internal static PaginationToken Eof { get; } = new PaginationToken(0, true);

        internal PaginationToken(int nextPageStart, bool endOfFile = false)
        {
            this.NextPageStart = nextPageStart;
            this.EndOfFile = endOfFile;
        }

        public int NextPageStart { get; }
        public bool EndOfFile { get; }
    }
}

