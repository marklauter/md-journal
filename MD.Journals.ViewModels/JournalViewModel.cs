using MD.Journal.Journals;
using System.Collections.ObjectModel;

namespace MD.Journals.ViewModels
{
    internal sealed class JournalViewModel
        : ObservableViewModel
    {
        private readonly IJournal journal;

        public JournalViewModel(IJournal journal)
        {
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
            this.FillTagsAsync();
            this.FillJournalEntries();
        }

        public ObservableCollection<string> Tags { get; } = new();

        private async void FillTagsAsync()
        {
            var tags = await this.journal.ReadTagsAsync();
            this.Tags.Clear();
            foreach (var tag in tags)
            {
                this.Tags.Add(tag);
            }
        }

        private void FillJournalEntries()
        {

        }
    }
}
