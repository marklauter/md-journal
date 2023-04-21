using MD.Journal.Storage;
using Microsoft.Extensions.Options;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace MD.Journal.Recents
{
    public class RecentItems
        : IRecentItems
    {
        private readonly IStore store;
        private readonly int entryLimit;

        public RecentItems(
            IStore store,
            IOptions<RecentItemsOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.entryLimit = options.Value.EntryLimit;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<IEnumerable<RecentItem>> ReadAsync()
        {
            return (await this.store.ReadLinesAsync(Pagination.Default))
                .Where(line => !String.IsNullOrWhiteSpace(line))
                .Select(line => (RecentItem)line)
                .OrderByDescending(item => item.LastAccessUtc);
        }

        public async Task TouchAsync(RecentItem recentItem)
        {
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

            await this.store
                .OverwriteAllLinesAsync(items.Select(item => (string)item));
        }
    }
}
