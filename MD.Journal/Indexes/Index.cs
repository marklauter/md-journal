using MD.Journal.IO;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Indexes
{
    internal sealed class Index<TValue>
        : IIndex<TValue>
        where TValue : IComparable<TValue>
    {
        private readonly IDocument store;

        public Index(IDocument store)
        {
            this.store = store;
        }

        public async Task<IEnumerable<IndexEntry<TValue>>> FindAsync(string key)
        {
            return (await this.store.ReadLinesAsync(Pagination.Default))
                .Select(line => (IndexEntry<TValue>)line)
                .Where(entry => String.Compare(entry.Key, key, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public async Task PackAsync()
        {
            var lines = (await this.ReadAsync(Pagination.Default))
                .Order()
                .Select(item => (string)item);

            await this.store.OverwriteAllLinesAsync(lines);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<IndexEntry<TValue>>> ReadAsync(Pagination pagination)
        {
            return (await this.store.ReadLinesAsync(pagination))
                .Distinct()
                .Where(line => !String.IsNullOrEmpty(line))
                .Select(line => (IndexEntry<TValue>)line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteAsync(IndexEntry<TValue> entry)
        {
            return this.store.AppendLineAsync((string)entry);
        }
    }
}
