using MD.Journal.Journals;
using MD.Journal.Markdown;
using MD.Journal.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MD.Journal.Windows.ViewModels
{
    public sealed class JournalViewModel
        : INotifyPropertyChanged
    {
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
                    this.CurrentJournalEntryMarkdown = value?.ToMarkdownString();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentJournalEntry)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.CurrentJournalEntryMarkdown)));
                }
            }
        }

        public ObservableCollection<JournalEntry> JournalEntries { get; } = new();
        public ObservableCollection<string> Tags { get; } = new();
        public ObservableCollection<string> SearchText { get; } = new();

        public JournalViewModel(Journals.Journal journal)
        {
            this.Journal = journal ?? throw new ArgumentNullException(nameof(journal));

            this.FillJournalEntriesAsync();
            this.FillTagsAsync();
        }

        private async void FillJournalEntriesAsync()
        {
            this.CurrentJournalEntry = null;

            var entries = (await this.Journal.ReadAsync(Pagination.Default))
                .Where(entry => entry is not null)
                .OrderByDescending(entry => entry?.Date);

            this.JournalEntries.Clear();
            foreach (var entry in entries)
            {
                if (entry is not null)
                {
                    this.JournalEntries.Add(entry);
                }
            }

            var searchText = entries
                .SelectMany(entry => entry is null ? Array.Empty<string>() : entry.Title.Split(" "))
                .Union(entries.SelectMany(entry => entry is null ? Array.Empty<string>() : entry.Tags))
                .Union(entries.Select(entry => entry is null ? String.Empty : entry.Author))
                .Where(entry => !String.IsNullOrWhiteSpace(entry))
                .Distinct();

            this.SearchText.Clear();
            foreach (var entry in searchText)
            {
                if (entry is not null)
                {
                    this.SearchText.Add(entry);
                }
            }
        }

        private async void FillTagsAsync()
        {
            var tags = await this.Journal.ReadTagsAsync();

            this.Tags.Clear();
            this.Tags.Add(String.Empty);
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
            if (String.IsNullOrEmpty(tag))
            {
                this.FillJournalEntriesAsync();
                return;
            }

            this.CurrentJournalEntry = null;

            var entries = (await this.Journal
                .FindAsync(tag, Pagination.Default))
                .Where(entry => entry is not null)
                .OrderByDescending(entry => entry?.Date);

            this.JournalEntries.Clear();
            foreach (var entry in entries)
            {
                if (entry is not null)
                {
                    this.JournalEntries.Add(entry);
                }
            }
        }
    }
}
