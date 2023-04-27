using MD.Journal.Recents;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MD.Journal.ResourceStorage;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class GetStartedViewModel
    {
        public ObservableCollection<RecentItem> RecentJournals { get; } = new();

        private readonly IResourceStoreGroup stores;
        private readonly IRecentItems recentJournals;

        public GetStartedViewModel(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            this.stores = new StoreSet<FileResourceStore>(path);
            this.recentJournals = new RecentItems(this.stores["recent-journals.json"], Options.Create(new RecentItemsOptions()));

            _ = this.FillRecentRepositoriesAsync();
        }

        private async Task FillRecentRepositoriesAsync()
        {
            var journals = await this.recentJournals.ReadAsync();
            this.RecentJournals.Clear();
            foreach (var journal in journals)
            {
                this.RecentJournals.Add(journal);
            }
        }

        public async Task<Journals.Journal> OpenJournalAsync(string path)
        {
            var journal = Journals.Journal.Open<FileResourceStore>(path);
            await this.recentJournals.TouchAsync(new RecentItem(journal.Path, DateTime.UtcNow));
            await this.FillRecentRepositoriesAsync();
            return journal;
        }
    }
}
