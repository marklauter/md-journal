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
            IOptions<ResourceReaderOptions> options)
            : base(options)
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
            return this.ReadNextLinesAsync(PaginationToken.Bof(uri));
        }

        [Pure]
        public override Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken)
        {
            var uri = paginationToken.Uri;
            if (!this.Exists(uri))
            {
                return Task.FromResult(new ReadLinesResponse(
                    Enumerable.Empty<string>(),
                    PaginationToken.Eof(uri)));
            }

            if (!this.resourceStore.Resources.TryGetValue(uri, out var Resources))
            {
                return Task.FromResult(new ReadLinesResponse(
                    Enumerable.Empty<string>(),
                    PaginationToken.Eof(uri)));
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.PageSize;
            var lines = new string[this.PageSize];
            foreach (var line in Resources)
            {
                if (linenumber >= start && line is not null)
                {
                    lines[linenumber] = line;
                }

                if (++linenumber == end)
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
            return (await this.ReadTextAsync(uri))[offset..(offset + length)];
        }
    }
}

