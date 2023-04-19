using Newtonsoft.Json;

namespace MD.Journal
{
    public sealed class RecentJournals
    {
        private readonly string fileName;
        private const int HistoryLimit = 20;

        public RecentJournals(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            _ = Directory.CreateDirectory(path);
            this.fileName = Path.Combine(path, "recent-journals.txt");
        }

        public async Task TouchAsync(Journal journal)
        {
            var entries = File.Exists(this.fileName)
                ? (await File.ReadAllLinesAsync(this.fileName))
                    .Select(JsonConvert.DeserializeObject<RecentJournalEntry>)
                    .OrderByDescending(entry => entry.LastAccessUtc)
                    .ToArray()
                : Array.Empty<RecentJournalEntry>();

            var journalPath = journal.Path;
            var found = false;
            for (var i = 0; i < entries.Length; ++i)
            {
                var entry = entries[i];
                if (entry.Path.Equals(journalPath, StringComparison.OrdinalIgnoreCase))
                {
                    entries[i] = new RecentJournalEntry(journalPath, DateTime.UtcNow);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // replace the oldest entry if not found or append a new entry
                if (entries.Length == HistoryLimit)
                {
                    entries[^1] = new RecentJournalEntry(journalPath, DateTime.UtcNow);
                }
                else
                {
                    entries = entries
                        .Append(new RecentJournalEntry(journalPath, DateTime.UtcNow))
                        .ToArray();
                }
            }

            var lines = entries
                .Select(entry => JsonConvert.SerializeObject(entry))
                .ToArray();

            await File.WriteAllLinesAsync(this.fileName, lines);
        }

        public async Task<RecentJournalEntry[]> ReadAsync()
        {
            return File.Exists(this.fileName)
                ? (await File.ReadAllLinesAsync(this.fileName))
                    .Select(JsonConvert.DeserializeObject<RecentJournalEntry>)
                    .OrderByDescending(entry => entry.LastAccessUtc)
                    .ToArray()
                : Array.Empty<RecentJournalEntry>();
        }
    }
}
