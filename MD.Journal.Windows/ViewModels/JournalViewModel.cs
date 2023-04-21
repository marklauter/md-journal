using MD.Journal.Journals;
using MD.Journal.Storage;
using System;
using System.Collections.ObjectModel;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class JournalViewModel
    {
        private readonly Journals.Journal journal;

        public JournalViewModel(Journals.Journal journal)
        {
            this.journal = journal ?? throw new ArgumentNullException(nameof(journal));

            this.WriteEntryAsync();
            this.FillJournalEntriesAsync();
            this.FillTagsAsync();
        }

        private async void FillJournalEntriesAsync()
        {
            var entries = await this.journal.ReadAsync(new Pagination(0, Int32.MaxValue));

            this.JournalEntries.Clear();
            foreach (var entry in entries)
            {
                if (entry is not null)
                {
                    this.JournalEntries.Add(entry);
                }
            }
        }

        private async void FillTagsAsync()
        {
            var tags = await this.journal.ReadTagsAsync();

            this.Tags.Clear();
            foreach (var tag in tags)
            {
                this.Tags.Add(tag);
            }
        }

        public ObservableCollection<JournalEntry> JournalEntries { get; } = new();
        public ObservableCollection<string> Tags { get; } = new();
        public string Path => this.journal.Path;

        public void NewJournalEntry()
        {
            // todo: navigate to edit page
            throw new NotImplementedException();
        }

        public async void SearchAsync(string tag)
        {
            var entries = await this.journal
                .FindAsync(tag, new Pagination(0, Int32.MaxValue));

            this.JournalEntries.Clear();
            foreach (var entry in entries)
            {
                if (entry is not null)
                {
                    this.JournalEntries.Add(entry);
                }
            }
        }

        private async void WriteEntryAsync()
        {
            var lines = new string[]
            {
                "## heading 2",
                "line 1",
                "line 2",
                String.Empty,
                "line 3",
                "- bullet 1",
                "- bullet 2",
                String.Empty,
                "line 1",
                "line 2",
                String.Empty,
                "line 3",
            };

            var entry = JournalEntryBuilder.Create()
                .WithTitle("title")
                .WithAuthor("author")
                .WithSummary("summary")
                .WithBody(String.Join(Environment.NewLine, lines))
                .WithTags(new string[] { "tag1", "tag2" })
                .Build();

            await this.journal.WriteAsync(entry);
        }
    }
}
