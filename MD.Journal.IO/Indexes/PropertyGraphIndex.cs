using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Indexes
{
    internal sealed class PropertyGraphIndex
        : IPropertyGraphIndex
    {
        private readonly IResourceReader reader;
        private readonly IResourceWriter writer;
        private readonly ILogger<PropertyGraphIndex> logger;
        private readonly IndexOptions options;
        private readonly ResourceUri rootUri;
        private readonly ResourceUri indexUri;

        public PropertyGraphIndex(
            IResourceReader reader,
            IResourceWriter writer,
            IOptions<IndexOptions> options,
            ILogger<PropertyGraphIndex> logger)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.logger = logger;
            this.options = options.Value;
            this.rootUri = ResourceUri.Empty.WithPath(this.options.Path);
            this.indexUri = this.rootUri.WithPath(this.options.Name);
            this.logger.LogInformation("{MethodName}({IndexUri})", "ctor", (string)this.indexUri);
        }

        public async Task MapAsync(string key, string value)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
            }

            this.logger.LogInformation("{MethodName}({Key}, {Value}, {IndexUri})", nameof(MapAsync), key, value, (string)this.indexUri);

            if (!(await this.ReadPropertiesAsync()).Contains(key))
            {
                await this.writer.AppendLineAsync(this.indexUri, key);
            }

            var valuesUri = this.rootUri.WithPath($"{(PropertyId)key}.values");
            await this.writer.AppendLineAsync(valuesUri, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task MapAsync(IEnumerable<string> keys, string value)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"'{nameof(value)}' cannot be null or empty.", nameof(value));
            }

            this.logger.LogInformation("{MethodName}({Keys}, {Value})", nameof(MapAsync), String.Join(',', keys), value);

            await Task.WhenAll(keys.Select(key => this.MapAsync(key, value)));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<IEnumerable<string>> ReadValuesAsync(string key)
        {
            this.logger.LogInformation("{MethodName}({Key})", nameof(ReadValuesAsync), key);
            var valuesUri = this.rootUri.WithPath($"{(PropertyId)key}.values");
            return this.reader.ReadAllLinesAsync(valuesUri);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<IEnumerable<string>> ReadPropertiesAsync()
        {
            this.logger.LogInformation("{MethodName}({IndexUri})", nameof(ReadPropertiesAsync), (string)this.indexUri);
            return this.reader.ReadAllLinesAsync(this.indexUri);
        }
    }
}
