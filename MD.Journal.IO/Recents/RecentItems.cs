using MD.Journal.IO.Readers;
using MD.Journal.IO.Writers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.IO.Recents
{
    public class RecentItems
        : IRecentItems
    {
        private readonly int entryLimit;
        private readonly ResourceUri uri;
        private readonly IResourceReader reader;
        private readonly IResourceWriter writer;
        private readonly ILogger<RecentItems> logger;

        public RecentItems(
            IResourceReader reader,
            IResourceWriter writer,
            IOptions<RecentItemsOptions> options,
            ILogger<RecentItems> logger)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.entryLimit = options.Value.EntryLimit;
            this.uri = ResourceUri.Empty.Combine(options.Value.Path, options.Value.Name);
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<RecentItem>> ReadAsync()
        {
            this.logger.LogInformation("{MethodName}({Uri})", nameof(ReadAsync), (string)this.uri);

            return (await this.reader.ReadAllLinesAsync(this.uri))
                .Where(line => !String.IsNullOrWhiteSpace(line))
                .Select(line => (RecentItem)line)
                .OrderByDescending(item => item.LastAccessUtc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task TouchAsync(RecentItem recentItem)
        {
            this.logger.LogInformation("{MethodName}({Uri}, {@RecentItem})", nameof(TouchAsync), (string)this.uri, recentItem);

            var items = (await this.ReadAsync()).ToArray();

            var found = false;
            for (var i = 0; i < items.Length; ++i)
            {
                if (items[i].CompareTo(recentItem) == 0)
                {
                    items[i] = recentItem;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // replace the oldest entry if not found or append a new entry
                if (items.Length == this.entryLimit)
                {
                    items[^1] = recentItem;
                }
                else
                {
                    items = items
                        .Append(recentItem)
                        .ToArray();
                }
            }

            await this.writer
                .OverwriteAllLinesAsync(this.uri, items.Select(item => (string)item));
        }
    }
}
