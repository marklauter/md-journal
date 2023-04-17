using Newtonsoft.Json;

namespace MD.Journal
{
    public sealed class RepositoryHistory
    {
        private const string RepositoryHistoryFileName = "repositories.txt";
        private const int HistoryLimit = 20;

        public static async Task AddRecentRepositoryAsync(string path)
        {
            var lines = await File.ReadAllLinesAsync(RepositoryHistoryFileName);
            var entries = lines
                .Select(JsonConvert.DeserializeObject<RepositoryHistoryEntry>)
                .OrderBy(entry => entry.LastAccessUtc)
                .ToArray();

            var found = false;
            for (var i = 0; i < entries.Length; ++i)
            {
                var entry = entries[i];
                if (entry.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    entries[i] = new RepositoryHistoryEntry(path, DateTime.UtcNow);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                // replace the oldest entry if not found or append a new entry
                if (entries.Length == HistoryLimit)
                {
                    entries[^1] = new RepositoryHistoryEntry(path, DateTime.UtcNow);
                }
                else
                {
                    entries = entries
                        .Append(new RepositoryHistoryEntry(path, DateTime.UtcNow))
                        .ToArray();
                }
            }

            lines = entries
                .Select(entry => JsonConvert.SerializeObject(entry))
                .ToArray();

            await File.WriteAllLinesAsync(RepositoryHistoryFileName, lines);
        }

        public static async Task<RepositoryHistoryEntry[]> RecentRepositoriesAsync()
        {
            var lines = await File.ReadAllLinesAsync(RepositoryHistoryFileName);
            return lines
                .Select(JsonConvert.DeserializeObject<RepositoryHistoryEntry>)
                .OrderByDescending(entry => entry.LastAccessUtc)
                .ToArray();
        }
    }
}
