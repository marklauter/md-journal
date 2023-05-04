using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Indexes
{
    internal sealed class Index<TValue>
        : IIndex<TValue>
        where TValue : IComparable<TValue>
    {
        private readonly IResourceReader reader;
        private readonly IResourceWriter writer;
        private readonly IndexOptions options;
        private readonly ResourceUri rootUri;
        private readonly ResourceUri indexUri;
        private readonly ILogger<Index> logger;

        public Index(
            IResourceReader reader,
            IResourceWriter writer,
            IOptions<IndexOptions> options,
            ILogger<Index> logger)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.options = options.Value;
            this.rootUri = ResourceUri.Empty.WithPath(this.options.Path);
            this.indexUri = this.rootUri.WithPath(this.options.Name);
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.logger.LogInformation("{MethodName}({IndexUri})", "ctor", (string)this.indexUri);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task PackAsync()
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(PackAsync), (string)this.indexUri);

            var lines = (await this.ReadAsync())
                .Order()
                .Select(item => (string)item);

            await this.writer.OverwriteAllLinesAsync(this.indexUri, lines);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<IndexEntry<TValue>>> ReadAsync()
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadAsync), (string)this.indexUri);

            return (await this.reader.ReadAllLinesAsync(this.indexUri))
                .Where(line => !String.IsNullOrWhiteSpace(line))
                .Distinct()
                .Select(line => (IndexEntry<TValue>)line);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<IndexEntry<TValue>>> ReadAsync(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            this.logger.LogInformation("{MethodName}({Key}, {Uri})", nameof(ReadAsync), key, (string)this.indexUri);

            return (await this.ReadAsync())
                .Where(entry => String.Compare(entry.Key, key, StringComparison.OrdinalIgnoreCase) == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteAsync(IndexEntry<TValue> entry)
        {
            this.logger.LogInformation("{MethodName}({Uri}, {@Entry})", nameof(WriteAsync), (string)this.indexUri, entry);

            return this.writer.AppendLineAsync(this.indexUri, (string)entry);
        }
    }
}
