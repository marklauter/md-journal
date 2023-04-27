using MD.Journal.Pagination;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Internal
{
    internal sealed class MemoryDocumentReader
        : DocumentReader
    {
        public MemoryDocumentReader(DocumentUri uri, int pageSize)
            : base(uri, pageSize)
        {
        }

        private bool Exists()
        {
            return DocumentStore.Documents.ContainsKey(this.Uri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<IEnumerable<string>> ReadAllLinesAsync()
        {
            return Task.FromResult(!this.Exists()
                ? Enumerable.Empty<string>()
                : !DocumentStore.Documents.TryGetValue(this.Uri, out var lines)
                    ? Enumerable.Empty<string>()
                    : lines);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<ReadLinesResponse> ReadLinesAsync()
        {
            return this.ReadNextLinesAsync(PaginationToken.Bof);
        }

        [Pure]
        public override Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken)
        {
            if (!this.Exists())
            {
                return Task.FromResult(new ReadLinesResponse(
                    Enumerable.Empty<string>(),
                    PaginationToken.Eof));
            }

            if (!DocumentStore.Documents.TryGetValue(this.Uri, out var documents))
            {
                return Task.FromResult(new ReadLinesResponse(
                    Enumerable.Empty<string>(),
                    PaginationToken.Eof));
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.PageSize;
            var lines = new List<string>();
            foreach (var line in documents)
            {
                if (linenumber >= start && line is not null)
                {
                    lines.Add(line);
                }

                if (++linenumber == end)
                {
                    return Task.FromResult(new ReadLinesResponse(
                        lines,
                        new PaginationToken(end)));
                }
            }

            return Task.FromResult(new ReadLinesResponse(
                lines,
                PaginationToken.Eof));
        }

        [Pure]
        public override Task<string> ReadTextAsync()
        {
            return !this.Exists()
                ? Task.FromResult(String.Empty)
                : !DocumentStore.Documents.TryGetValue(this.Uri, out var lines)
                    ? Task.FromResult(String.Empty)
                    : Task.FromResult(String.Join(Environment.NewLine, lines));
        }

        [Pure]
        public override async Task<string> ReadTextAsync(int offset, int length)
        {
            return (await this.ReadTextAsync())[offset..(offset + length)];
        }
    }
}

