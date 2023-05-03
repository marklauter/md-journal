using MD.Journal.IO.Pagination;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Readers
{
    internal sealed class ResourceReader
        : IResourceReader
    {
        private readonly int pageSize;
        private readonly ILogger<ResourceReader> logger;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ResourceReader(
            IOptions<PaginationOptions> options,
            ILogger<ResourceReader> logger)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.pageSize = options.Value.PageSize;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadAllLinesAsync), (string)uri);

            return !File.Exists(uri)
                ? Enumerable.Empty<string>()
                : await File.ReadAllLinesAsync(uri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<ReadResponse> ReadLinesAsync(ResourceUri uri)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadLinesAsync), (string)uri);

            return this.ReadLinesAsync(PaginationToken.Bof(uri));
        }

        [Pure]
        public async Task<ReadResponse> ReadLinesAsync(PaginationToken paginationToken)
        {
            this.logger.LogInformation("{MethodName}({@PaginationToken})", nameof(ReadLinesAsync), paginationToken);

            var uri = paginationToken.Uri;
            if (!File.Exists(uri))
            {
                return new ReadResponse(
                    Array.Empty<string>(),
                    PaginationToken.Eof(uri));
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.pageSize;
            var lines = new string[this.pageSize];

            using var stream = File.OpenRead(uri);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (linenumber + start >= start && line is not null)
                {
                    lines[linenumber] = line;
                }

                if (++linenumber + start == end)
                {
                    return new ReadResponse(
                        lines,
                        new PaginationToken(uri, end));
                }
            }

            return new ReadResponse(
                lines[..linenumber],
                PaginationToken.Eof(uri));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<string> ReadTextAsync(ResourceUri uri)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadTextAsync), (string)uri);

            return !File.Exists(uri)
                ? Task.FromResult(String.Empty)
                : File.ReadAllTextAsync(uri);
        }

        [Pure]
        public async Task<string> ReadTextAsync(ResourceUri uri, int offset, int length)
        {
            this.logger.LogInformation("{MethodName}({Uri}, {Offset}, {Length})", nameof(ReadTextAsync), (string)uri, offset, length);

            if (!File.Exists(uri))
            {
                return String.Empty;
            }

            using var stream = File.OpenRead(uri);
            using var reader = new StreamReader(stream);
            var buffer = new char[length];
            var blockSize = await reader.ReadBlockAsync(buffer, offset, length);

            return new string(buffer[..blockSize]);
        }
    }
}

