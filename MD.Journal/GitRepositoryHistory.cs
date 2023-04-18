using Newtonsoft.Json;

namespace MD.Journal
{
    public sealed class GitRepositoryHistory
    {
        private readonly string repositoryHistoryFileName;
        private readonly string dataPath;
        private const int HistoryLimit = 20;

        public GitRepositoryHistory(string dataPath)
        {
            if (String.IsNullOrWhiteSpace(dataPath))
            {
                throw new ArgumentException($"'{nameof(dataPath)}' cannot be null or whitespace.", nameof(dataPath));
            }

            _ = Directory.CreateDirectory(dataPath);
            this.repositoryHistoryFileName = Path.Combine(dataPath, "repositories.txt");
            this.dataPath = dataPath;
        }

        public async Task<string> ReadRecentAuthorAsync()
        {
            var fileName = System.IO.Path.Combine(this.dataPath, "author.txt");
            return !File.Exists(fileName)
                ? String.Empty
                : await File.ReadAllTextAsync(fileName);
        }

        public async Task WriteRecentAuthorAsync(string author)
        {
            if (String.IsNullOrWhiteSpace(author))
            {
                throw new ArgumentException($"'{nameof(author)}' cannot be null or whitespace.", nameof(author));
            }

            var fileName = System.IO.Path.Combine(this.dataPath, "author.txt");
            await File.WriteAllTextAsync(fileName, author);
        }

        public async Task AddRecentRepositoryAsync(string path)
        {
            var entries = File.Exists(this.repositoryHistoryFileName)
                ? (await File.ReadAllLinesAsync(this.repositoryHistoryFileName))
                    .Select(JsonConvert.DeserializeObject<RepositoryHistoryEntry>)
                    .OrderByDescending(entry => entry.LastAccessUtc)
                    .ToArray()
                : Array.Empty<RepositoryHistoryEntry>();

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

            var lines = entries
                .Select(entry => JsonConvert.SerializeObject(entry))
                .ToArray();

            await File.WriteAllLinesAsync(this.repositoryHistoryFileName, lines);
        }

        public async Task<RepositoryHistoryEntry[]> RecentRepositoriesAsync()
        {
            return File.Exists(this.repositoryHistoryFileName)
                ? (await File.ReadAllLinesAsync(this.repositoryHistoryFileName))
                    .Select(JsonConvert.DeserializeObject<RepositoryHistoryEntry>)
                    .OrderByDescending(entry => entry.LastAccessUtc)
                    .ToArray()
                : Array.Empty<RepositoryHistoryEntry>();
        }
    }
}
