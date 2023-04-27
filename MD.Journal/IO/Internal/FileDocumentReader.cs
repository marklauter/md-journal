using MD.Journal.Pagination;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Internal
{
    internal sealed class FileDocumentReader
        : DocumentReader
    {
        public FileDocumentReader(DocumentUri uri, int pageSize)
            : base(uri, pageSize)
        {
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<IEnumerable<string>> ReadAllLinesAsync()
        {
            return !File.Exists(this.Uri)
                ? Enumerable.Empty<string>()
                : await File.ReadAllLinesAsync(this.Uri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<ReadLinesResponse> ReadLinesAsync()
        {
            return this.ReadNextLinesAsync(PaginationToken.Bof);
        }

        [Pure]
        public override async Task<ReadLinesResponse> ReadNextLinesAsync(PaginationToken paginationToken)
        {
            if (!File.Exists(this.Uri))
            {
                return new ReadLinesResponse(
                    Enumerable.Empty<string>(),
                    PaginationToken.Eof);
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.PageSize;
            var lines = new List<string>();

            using var stream = File.OpenRead(this.Uri);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (linenumber >= start && line is not null)
                {
                    lines.Add(line);
                }

                if (++linenumber == end)
                {
                    return new ReadLinesResponse(
                        lines,
                        new PaginationToken(end));
                }
            }

            return new ReadLinesResponse(
                lines,
                PaginationToken.Eof);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<string> ReadTextAsync()
        {
            return !File.Exists(this.Uri)
                ? Task.FromResult(String.Empty)
                : File.ReadAllTextAsync(this.Uri);
        }

        [Pure]
        public override async Task<string> ReadTextAsync(int offset, int length)
        {
            if (!File.Exists(this.Uri))
            {
                return String.Empty;
            }

            using var stream = File.OpenRead(this.Uri);
            using var reader = new StreamReader(stream);
            var buffer = new char[length];
            var blockSize = await reader.ReadBlockAsync(buffer, offset, length);

            return new string(buffer[..blockSize]);
        }
    }
}

