using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Readers
{
    internal abstract class ResourceReader
        : IResourceReader
    {
        protected ResourceReader(IOptions<ResourceReaderOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.PageSize = options.Value.PageSize;
        }

        public int PageSize { get; }

        public abstract Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri);
        public abstract Task<ReadLinesResponse> ReadLinesAsync(ResourceUri uri);
        public abstract Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken);
        public abstract Task<string> ReadTextAsync(ResourceUri uri);
        public abstract Task<string> ReadTextAsync(ResourceUri uri, int offset, int length);
    }
}

