using Newtonsoft.Json;

namespace MD.Journal
{
    public sealed class Journal
    {
        public string Path { get; }
        private readonly TagGraph tagGraph;

        public static Journal Open(string path)
        {
            return new Journal(path);
        }

        private Journal(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.Path = path;
            this.tagGraph = new TagGraph(System.IO.Path.Combine(path, "tags"));
        }

        public async Task<JournalEntry?> ReadAsync(string journalEntryId)
        {
            var fileName = System.IO.Path.Combine(this.Path, $"{journalEntryId}.json");
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
            var fileName = System.IO.Path.Combine(this.Path, $"{journalEntry.Id}.json");
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
            var fileName = System.IO.Path.Combine(this.Path, $"{journalEntry.Id}.md");
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
