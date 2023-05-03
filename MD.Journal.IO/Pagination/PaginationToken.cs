using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Pagination
{
    public readonly struct PaginationToken
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static PaginationToken Bof(ResourceUri uri)
        {
            return new(uri, 0, false);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static PaginationToken Eof(ResourceUri uri)
        {
            return new(uri, 0, true);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

