using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Readers
{
    internal sealed class MemoryResourceReader
        : ResourceReader
    {
        private readonly IResourceStore resourceStore;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MemoryResourceReader(
            IResourceStore resourceStore,
            IOptions<ResourceReaderOptions> options,
            ILogger<MemoryResourceReader> logger)
            : base(options, logger)
        {
            this.resourceStore = resourceStore;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(ResourceUri uri)
        {
            return this.resourceStore.Resources.ContainsKey(uri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri)
        {
            this.Logger.LogInformation("{MethodName}({Uri})", nameof(ReadAllLinesAsync), (string)uri);

            return Task.FromResult(!this.Exists(uri)
                ? Enumerable.Empty<string>()
                : !this.resourceStore.Resources.TryGetValue(uri, out var lines)
                    ? Enumerable.Empty<string>()
                    : lines);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<ReadLinesResponse> ReadLinesAsync(ResourceUri uri)
        {
            this.Logger.LogInformation("{MethodName}({Uri})", nameof(ReadLinesAsync), (string)uri);

            return this.ReadLinesAsync(PaginationToken.Bof(uri));
        }

        [Pure]
        public override Task<ReadLinesResponse> ReadLinesAsync(PaginationToken paginationToken)
        {
            this.Logger.LogInformation("{MethodName}({@PaginationToken})", nameof(ReadLinesAsync), paginationToken);

            var uri = paginationToken.Uri;
            if (!this.Exists(uri))
            {
                return Task.FromResult(new ReadLinesResponse(
                    Array.Empty<string>(),
                    PaginationToken.Eof(uri)));
            }

            if (!this.resourceStore.Resources.TryGetValue(uri, out var resources))
            {
                return Task.FromResult(new ReadLinesResponse(
                    Array.Empty<string>(),
                    PaginationToken.Eof(uri)));
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.PageSize;
            var lines = new string[this.PageSize];
            foreach (var line in resources)
            {
                if (linenumber + start >= start && line is not null)
                {
                    lines[linenumber] = line;
                }

                if (++linenumber + start == end)
                {
                    return Task.FromResult(new ReadLinesResponse(
                        lines,
                        new PaginationToken(uri, end)));
                }
            }

            return Task.FromResult(new ReadLinesResponse(
                lines[..linenumber],
                PaginationToken.Eof(uri)));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<string> ReadTextAsync(ResourceUri uri)
        {
            this.Logger.LogInformation("{MethodName}({Uri})", nameof(ReadTextAsync), (string)uri);

            return !this.Exists(uri)
                ? Task.FromResult(String.Empty)
                : !this.resourceStore.Resources.TryGetValue(uri, out var lines)
                    ? Task.FromResult(String.Empty)
                    : Task.FromResult(String.Join(Environment.NewLine, lines));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<string> ReadTextAsync(ResourceUri uri, int offset, int length)
        {
            this.Logger.LogInformation("{MethodName}({Uri}, {Offset}, {Length})", nameof(ReadTextAsync), (string)uri, offset, length);

            return !this.Exists(uri)
                ? String.Empty
                : (await this.ReadTextAsync(uri))[offset..(offset + length)];
        }
    }
}

