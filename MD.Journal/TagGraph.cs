namespace MD.Journal
{
    public sealed class TagGraph
    {
        private readonly string path;

        public TagGraph(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            path = Path.Combine(path, "tags");

            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.path = path;
        }

        public async Task<string[]> MapJournalEntryAsync(JournalEntry journalEntry)
        {
            if (journalEntry is null)
            {
                throw new ArgumentNullException(nameof(journalEntry));
            }

            var fileNames = new string[journalEntry.Tags.Length];
            var i = 0;
            foreach (var tag in journalEntry.Tags)
            {
                var fileName = Path.Combine(this.path, $"tag-map.{tag}.txt");
                using var file = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                using var writer = new StreamWriter(file);
                _ = writer.BaseStream.Seek(0, SeekOrigin.End);
                await writer.WriteLineAsync(journalEntry.Id);
                await writer.FlushAsync();
                fileNames[i++] = fileName;
            }

            return fileNames;
        }

        public async Task<string[]> JournalEntriesAsync(string tag)
        {
            if (String.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentException($"'{nameof(tag)}' cannot be null or whitespace.", nameof(tag));
            }

            var fileName = Path.Combine(this.path, $"tag-map.{tag}.txt");
            return !File.Exists(fileName)
                ? Array.Empty<string>()
                : await File.ReadAllLinesAsync(fileName);
        }

        public string[] Tags()
        {
            var files = Directory.GetFiles(this.path, "tag-map.*.txt");
            var tags = new string[files.Length];
            for (var i = 0; i < files.Length; ++i)
            {
                var file = Path.GetFileName(files[i]);
                tags[i] = file[8..^4];
            }

            return tags;
        }
    }
}
