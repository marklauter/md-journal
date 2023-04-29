﻿using MD.Journal.IO.Pagination;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MD.Journal.IO.Readers
{
    internal abstract class ResourceReader
        : IResourceReader
    {
        protected ResourceReader(
            IOptions<PaginationOptions> options,
            ILogger<ResourceReader> logger)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.PageSize = options.Value.PageSize;
            this.Logger = logger;
        }

        public int PageSize { get; }
        public ILogger<ResourceReader> Logger { get; }

        public abstract Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri);
        public abstract Task<ReadResponse> ReadLinesAsync(ResourceUri uri);
        public abstract Task<ReadResponse> ReadLinesAsync(PaginationToken paginationToken);
        public abstract Task<string> ReadTextAsync(ResourceUri uri);
        public abstract Task<string> ReadTextAsync(ResourceUri uri, int offset, int length);
    }
}

