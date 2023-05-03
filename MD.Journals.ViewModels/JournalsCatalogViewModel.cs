using MD.Journal.IO;
using MD.Journal.IO.Recents;
using MD.Journal.Journals;
using System.Collections.ObjectModel;

namespace MD.Journals.ViewModels
{
    internal sealed class JournalsCatalogViewModel
    {
        private readonly IJournalCatalog journalCatalog;

        public ObservableCollection<RecentItem> RecentJournals { get; } = new();

        public JournalsCatalogViewModel(IJournalCatalog journalsCatalog)
        {
            _ = this.FillRecentJournalsAsync();
            this.journalCatalog = journalsCatalog;
        }

        private async Task FillRecentJournalsAsync()
        {
            var journals = await this.journalCatalog.ReadAsync();
            this.RecentJournals.Clear();
            foreach (var journal in journals)
            {
                this.RecentJournals.Add(journal);
            }
        }

        public async Task<IJournal> OpenJournalAsync(ResourceUri uri)
        {
            var journal = await this.journalCatalog.OpenJournalAsync(uri);
            await this.FillRecentJournalsAsync();
            return journal;
        }
    }
}
