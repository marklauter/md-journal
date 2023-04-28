namespace MD.Journal.IO.Readers
{
    public readonly struct PaginationToken
    {
        internal static PaginationToken Bof(ResourceUri uri)
        {
            return new(uri, 0, false);
        }

        internal static PaginationToken Eof(ResourceUri uri)
        {
            return new(uri, 0, true);
        }

        internal PaginationToken(ResourceUri uri, int nextPageStart, bool endOfFile = false)
        {
            this.Uri = uri;
            this.NextPageStart = nextPageStart;
            this.EndOfFile = endOfFile;
        }

        public ResourceUri Uri { get; }
        public int NextPageStart { get; }
        public bool EndOfFile { get; }
    }
}

