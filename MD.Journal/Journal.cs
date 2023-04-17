using Newtonsoft.Json;

namespace MD.Journal
{
    public sealed class Journal
    {
        private readonly string path;
        private readonly TagGraph tagGraph;

        public Journal(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.path = path;
            this.tagGraph = new TagGraph(Path.Combine(path, "tags"));
        }

        public async Task<string> AuthorAsync()
        {
            var fileName = Path.Combine(this.path, "author.txt");
            return !File.Exists(fileName)
                ? String.Empty
                : await File.ReadAllTextAsync(fileName);
        }

        public async Task AuthorAsync(string author)
        {
            if (String.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException($"'{nameof(author)}' cannot be null or whitespace.", nameof(author));
            }

            var fileName = Path.Combine(this.path, "author.txt");
            await File.WriteAllTextAsync(fileName, author);
        }

        public async Task<JournalEntry?> ReadAsync(string journalEntryId)
        {
            var fileName = Path.Combine(this.path, $"{journalEntryId}.json");
            if (!File.Exists(fileName))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(fileName);
            return JsonConvert.DeserializeObject<JournalEntry>(json);
        }

        public async Task<JournalEntry[]> ReadAsync(string[] journalEntryIds)
        {
            var tasks = new Task<JournalEntry?>[journalEntryIds.Length];
            for (var i = 0; i < tasks.Length; ++i)
            {
                tasks[i] = this.ReadAsync(journalEntryIds[i]);
            }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return (await Task.WhenAll(tasks)).Where(entry => entry is not null).ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public async Task<JournalEntry[]> FindAsync(string tag)
        {
            var entries = await this.tagGraph.JournalEntriesAsync(tag);
            return await this.ReadAsync(entries);
        }

        public async Task WriteAsync(
            JournalEntry journalEntry,
            CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                this.WriteAsJsonAsync(journalEntry, cancellationToken),
                this.WriteAsMarkdownAsync(journalEntry, cancellationToken),
                this.tagGraph.MapJournalEntryAsync(journalEntry));
        }

        private async Task WriteAsJsonAsync(
            JournalEntry journalEntry,
            CancellationToken cancellationToken)
        {
            var fileName = Path.Combine(this.path, $"{journalEntry.Id}.json");
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            using var streamWriter = new StreamWriter(file);
            using var jsonWriter = new JsonTextWriter(streamWriter);
            JsonSerializer
                .Create(new JsonSerializerSettings { Formatting = Formatting.Indented })
                .Serialize(jsonWriter, journalEntry);
            await file.FlushAsync(cancellationToken);
        }

        private async Task WriteAsMarkdownAsync(
            JournalEntry journalEntry,
            CancellationToken cancellationToken)
        {
            var fileName = Path.Combine(this.path, $"{journalEntry.Id}.md");
            using var markdown = journalEntry.ToMarkdownStream();
            using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            await markdown.CopyToAsync(file, cancellationToken);
            await file.FlushAsync(cancellationToken);
        }

        public Task<string[]> TagsAsync()
        {
            return this.tagGraph.TagsAsync();
        }
    }
}
