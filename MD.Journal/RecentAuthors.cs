namespace MD.Journal
{
    public sealed class RecentAuthors
    {
        private readonly string fileName;
        private const int HistoryLimit = 20;

        public RecentAuthors(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            _ = Directory.CreateDirectory(path);
            this.fileName = Path.Combine(path, "recent-authors.txt");
        }

        public async Task TouchAsync(JournalEntry journalEntry)
        {
            var authors = (await this.ReadAsync())
                .Append(journalEntry.Author)
                .Distinct()
                .Order()
                .Take(HistoryLimit)
                .ToArray();

            await File.WriteAllLinesAsync(this.fileName, authors);
        }

        public async Task<string[]> ReadAsync()
        {
            return File.Exists(this.fileName)
                ? await File.ReadAllLinesAsync(this.fileName)
                : Array.Empty<string>();
        }
    }
}
