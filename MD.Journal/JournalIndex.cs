using Newtonsoft.Json;

namespace MD.Journal
{
    // todo: add binary search
    public sealed class JournalIndex
    {
        private const string JournalIndexFolder = "index";
        private readonly string indexFileName;

        public JournalIndex(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace.", nameof(path));
            }

            path = Path.Combine(path, JournalIndexFolder);
            if (!Directory.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            this.indexFileName = Path.Combine(path, "journal-index.txt");
        }

        public async Task InsertAsync(JournalEntry journalEntry)
        {
            var newIndexLine = Enumerable
                .Repeat(JsonConvert.SerializeObject(new JournalIndexEntry(journalEntry.Id, journalEntry.Date)), 1);
            await File.AppendAllLinesAsync(this.indexFileName, newIndexLine);
        }

        public async Task PackAsync()
        {
            var lines = (await File.ReadAllLinesAsync(this.indexFileName))
                .Distinct()
                .Select(JsonConvert.DeserializeObject<JournalIndexEntry>)
                .OrderByDescending(entry => entry.Date)
                .Select(entry => JsonConvert.SerializeObject(entry));

            await File.WriteAllLinesAsync(this.indexFileName, lines);
        }

        public async Task<JournalIndexEntry[]> ReadAsync(Pagination pagination)
        {
            return File.Exists(this.indexFileName)
                ? (await File.ReadAllLinesAsync(this.indexFileName))
                    .Distinct()
                    .Select(JsonConvert.DeserializeObject<JournalIndexEntry>)
                    .OrderByDescending(entry => entry.Date)
                    .Skip(pagination.Skip)
                    .Take(pagination.Take)
                    .ToArray()
                : Array.Empty<JournalIndexEntry>();
        }
    }
}
