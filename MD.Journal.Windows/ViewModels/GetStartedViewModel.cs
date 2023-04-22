using MD.Journal.Recents;
using MD.Journal.Storage;
using Microsoft.Extensions.Options;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class RecentJournal
    {
        public RecentJournal(RecentItem recentItem)
        {
            this.Name = System.IO.Path.GetFileName(recentItem.Key);
            this.Path = recentItem.Key;
            this.LastAccessLocal = recentItem.LastAccessUtc.ToLocalTime().ToShortDateString();
        }

        public string Name { get; }
        public string Path { get; }
        public string LastAccessLocal { get; }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RecentJournal(RecentItem recentItem)
        {
            return new RecentJournal(recentItem);
        }
    }

    public sealed class GetStartedViewModel
    {
        public ObservableCollection<RecentJournal> RecentJournals { get; } = new();

        private readonly IStoreSet stores;
        private readonly IRecentItems recentJournals;

        public GetStartedViewModel(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            this.stores = new StoreSet<FileStore>(path);
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
            var journal = Journals.Journal.Open<FileStore>(path);
            await this.recentJournals.TouchAsync(new RecentItem(journal.Path, DateTime.UtcNow));
            await this.FillRecentRepositoriesAsync();
            return journal;
        }
    }
}
