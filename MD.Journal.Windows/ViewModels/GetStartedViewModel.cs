using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class GetStartedViewModel
    {
        public ObservableCollection<RecentJournalEntry> RecentJournals { get; }

        private readonly RecentJournals recentJournals;

        public GetStartedViewModel()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MDJournal");
            this.RecentJournals = new ObservableCollection<RecentJournalEntry>();
            this.recentJournals = new RecentJournals(path);
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

        public async Task<Journal> OpenJournalAsync(string path)
        {
            var journal = new Journal(path);
            await this.recentJournals.TouchAsync(journal);
            await this.FillRecentRepositoriesAsync();
            return journal;
        }
    }
}
