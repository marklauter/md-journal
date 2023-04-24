using MD.Journal.Journals;
using MD.Journal.Markdown;
using MD.Journal.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class JournalViewModel
        : INotifyPropertyChanged
    {
        private const string ClearTag = "[clear]";

        public event PropertyChangedEventHandler? PropertyChanged;

        public Journals.Journal Journal { get; private set; }

        public string? CurrentJournalEntryMarkdown { get; private set; }

        private JournalEntry? currentJournalEntry;
        public JournalEntry? CurrentJournalEntry
        {
            get => this.currentJournalEntry;
            set
            {
                if (this.currentJournalEntry != value)
                {
                    this.currentJournalEntry = value;
                    this.CurrentJournalEntryMarkdown = value.ToMarkdownString();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentJournalEntry)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentJournalEntryMarkdown)));
                }
            }
        }

        public ObservableCollection<JournalEntry> JournalEntries { get; } = new();
        public ObservableCollection<string> Tags { get; } = new();

        public JournalViewModel(Journals.Journal journal)
        {
            this.Journal = journal ?? throw new ArgumentNullException(nameof(journal));

            //this.WriteEntryAsync();
            this.FillJournalEntriesAsync();
            this.FillTagsAsync();
        }

        private async void FillJournalEntriesAsync()
        {
            var entries = await this.Journal.ReadAsync(new Pagination(0, Int32.MaxValue));

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
            var tags = await this.Journal.ReadTagsAsync();

            this.Tags.Clear();
            this.Tags.Add(ClearTag);
            foreach (var tag in tags)
            {
                this.Tags.Add(tag);
            }
        }

        public void NewJournalEntry()
        {
            // todo: navigate to edit page
            throw new NotImplementedException();
        }

        public async void SearchAsync(string tag)
        {
            if (tag.Equals(ClearTag, StringComparison.OrdinalIgnoreCase) || String.IsNullOrWhiteSpace(tag))
            {
                this.FillJournalEntriesAsync();
                return;
            }

            var entries = await this.Journal
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

        //private async void WriteEntryAsync()
        //{
        //    var lines = new string[]
        //    {
        //        "## heading 2",
        //        "line 1",
        //        "line 2",
        //        String.Empty,
        //        "line 3",
        //        "- bullet 1",
        //        "- bullet 2",
        //        String.Empty,
        //        "line 1",
        //        "line 2",
        //        String.Empty,
        //        "line 3",
        //    };

        //    var entry = JournalEntryBuilder.Create()
        //        .WithTitle("title")
        //        .WithAuthor("author")
        //        .WithSummary("summary")
        //        .WithBody(String.Join(Environment.NewLine, lines))
        //        .WithTags(new string[] { "tag1", "tag2" })
        //        .Build();

        //    await this.Journal.WriteAsync(entry);
        //}
    }
}
