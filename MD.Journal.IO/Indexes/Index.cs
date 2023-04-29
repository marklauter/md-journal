using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Indexes
{
    internal sealed class Index<TValue>
        : IIndex<TValue>
        where TValue : IComparable<TValue>
    {
        private readonly ResourceUri uri;
        private readonly IResourceReader reader;
        private readonly IResourceWriter writer;
        private readonly ILogger<Index> logger;

        public Index(
            ResourceUri uri,
            IResourceReader reader,
            IResourceWriter writer,
            ILogger<Index> logger)
        {
            this.uri = uri;
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async Task<IEnumerable<IndexEntry<TValue>>> ReadEntriesAsync()
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadEntriesAsync), (string)this.uri);

            return (await this.reader.ReadAllLinesAsync(this.uri))
                .Where(line => !String.IsNullOrWhiteSpace(line))
                .Distinct()
                .Select(line => (IndexEntry<TValue>)line);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task PackAsync()
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(PackAsync), (string)this.uri);

            var lines = (await this.ReadEntriesAsync())
                .Order()
                .Select(item => (string)item);

            await this.writer.OverwriteAllLinesAsync(this.uri, lines);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<IndexEntry<TValue>>> ReadAsync(string key)
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadAsync), (string)this.uri);

            return (await this.ReadEntriesAsync())
                .Where(entry => String.Compare(entry.Key, key, StringComparison.OrdinalIgnoreCase) == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteAsync(IndexEntry<TValue> entry)
        {
            this.logger.LogInformation("{MethodName}({Uri}, {@Entry})", nameof(WriteAsync), (string)this.uri, entry);

            return this.writer.AppendLineAsync(this.uri, (string)entry);
        }
    }
}
