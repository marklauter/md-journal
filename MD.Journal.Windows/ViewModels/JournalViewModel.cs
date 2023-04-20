using System;
using System.Collections.ObjectModel;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class JournalViewModel
    {
        private readonly Journal journal;

        public JournalViewModel(Journal journal)
        {
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));
        }

        public ObservableCollection<JournalEntry> JournalEntries { get; } = new();
    }
}
