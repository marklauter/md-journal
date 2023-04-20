using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class GetStartedViewModel
    {
        public ObservableCollection<RecentJournalEntry> RecentJournals { get; } = new();

        private readonly RecentJournals recentJournals;

        public GetStartedViewModel(string path)
        {
            if (System.String.IsNullOrWhiteSpace(path))
            {
                throw new System.ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

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
            var journal = Journal.Open(path);
            await this.recentJournals.TouchAsync(journal);
            await this.FillRecentRepositoriesAsync();
            return journal;
        }
    }
}
