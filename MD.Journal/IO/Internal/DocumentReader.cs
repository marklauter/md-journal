using MD.Journal.Pagination;

namespace MD.Journal.IO.Internal
{
    internal abstract class DocumentReader
        : IDocumentReader
    {
        protected DocumentReader(DocumentUri uri, int pageSize)
        {
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this.PageSize = pageSize;
        }

        public DocumentUri Uri { get; }
        public int PageSize { get; }

        public abstract Task<IEnumerable<string>> ReadAllLinesAsync();
        public abstract Task<ReadLinesResponse> ReadLinesAsync();
        public abstract Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken);

        public abstract Task<string> ReadTextAsync();
        public abstract Task<string> ReadTextAsync(int offset, int length);
    }
}

