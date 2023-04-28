using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Readers
{
    internal sealed class FileResourceReader
        : ResourceReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FileResourceReader(IOptions<ResourceReaderOptions> options)
            : base(options)
        {
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override async Task<IEnumerable<string>> ReadAllLinesAsync(ResourceUri uri)
        {
            return !File.Exists(uri)
                ? Enumerable.Empty<string>()
                : await File.ReadAllLinesAsync(uri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<ReadLinesResponse> ReadLinesAsync(ResourceUri uri)
        {
            return this.ReadLinesAsync(PaginationToken.Bof(uri));
        }

        [Pure]
        public override async Task<ReadLinesResponse> ReadLinesAsync(PaginationToken paginationToken)
        {
            var uri = paginationToken.Uri;
            if (!File.Exists(uri))
            {
                return new ReadLinesResponse(
                    Array.Empty<string>(),
                    PaginationToken.Eof(uri));
            }

            var linenumber = 0;
            var start = paginationToken.NextPageStart;
            var end = start + this.PageSize;
            var lines = new string[this.PageSize];

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
                    return new ReadLinesResponse(
                        lines,
                        new PaginationToken(uri, end));
                }
            }

            return new ReadLinesResponse(
                lines[..linenumber],
                PaginationToken.Eof(uri));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Task<string> ReadTextAsync(ResourceUri uri)
        {
            return !File.Exists(uri)
                ? Task.FromResult(String.Empty)
                : File.ReadAllTextAsync(uri);
        }

        [Pure]
        public override async Task<string> ReadTextAsync(ResourceUri uri, int offset, int length)
        {
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

