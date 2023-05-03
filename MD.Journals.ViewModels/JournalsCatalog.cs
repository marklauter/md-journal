using MD.Journal.IO;
using MD.Journal.IO.Recents;
using System.Collections.ObjectModel;

namespace MD.Journals.ViewModels
{
    internal sealed class JournalsCatalog
    {
        private readonly ResourceUri uri;
        private readonly IRecentItems recentJournalsRepository;

        public ObservableCollection<RecentItem> RecentJournals { get; } = new();

        //Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        public JournalsCatalog(
            IRecentItemsCatalog recentItemsCatalog,
            string path)
        {
            if (recentItemsCatalog is null)
            {
                throw new ArgumentNullException(nameof(recentItemsCatalog));
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));
            }

            this.uri = Path.Combine(path, "recent-journals");
            this.recentJournalsRepository = recentItemsCatalog.Open(this.uri);
            _ = this.FillRecentJournalsAsync();
        }

        private async Task FillRecentJournalsAsync()
        {
            var journals = await this.recentJournalsRepository.ReadAsync();
            this.RecentJournals.Clear();
            foreach (var journal in journals)
            {
                this.RecentJournals.Add(journal);
            }
        }

        public Task OpenJournalAsync(string path)
        {
            throw new NotImplementedException();
            //var journal = Journals.Journal.Open<FileResourceStore>(path);
            //await this.recentJournals.TouchAsync(new RecentItem(journal.Path, DateTime.UtcNow));
            //await this.FillRecentRepositoriesAsync();
            //return journal;
        }
    }
}
