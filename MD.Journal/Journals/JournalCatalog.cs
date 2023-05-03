using MD.Journal.IO;
using MD.Journal.IO.Recents;
using Microsoft.Extensions.DependencyInjection;

namespace MD.Journal.Journals
{
    internal sealed class JournalCatalog
        : IJournalCatalog
    {
        private readonly IRecentItems recentItems;
        private readonly IServiceProvider serviceProvider;

        public JournalCatalog(
            IRecentItems recentItems,
            IServiceProvider serviceProvider)
        {
            this.recentItems = recentItems;
            this.serviceProvider = serviceProvider;
        }

        public Task<IEnumerable<RecentItem>> ReadAsync()
        {
            return this.recentItems.ReadAsync();
        }

        public async Task<IJournal> OpenJournalAsync(ResourceUri uri)
        {
            var journal = this.serviceProvider.GetRequiredService<Func<ResourceUri, IJournal>>()(uri);
            await this.recentItems.TouchAsync(new RecentItem(journal.Uri, DateTime.UtcNow));
            return journal;
        }
    }
}
